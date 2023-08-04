using System;
using System.Collections.Generic;
using System.Linq;
using DefenderGame.Scripts.Systems;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Serialization;

namespace DefenderGame.Scripts.Components
{
    public class DeItemGrid : IComponentData
    {
        public DeItemGrid<DeGridObject> ItemGrid { get; }

        public HashSet<OngoingAction> OngoingActions { get; } = new();
        public float GridLength { get; }
        
        public HashSet<int2> GetBlockedGrids()
        {
            var blockedGrids = new HashSet<int2>();
            foreach (var ongoingAction in OngoingActions)
            {
                blockedGrids.UnionWith(ongoingAction.BlockingGrids);
            }
            return blockedGrids;
        }

        // doesnt work
        //public DeItemGrid(int width, int height, float gridLength)
        //{
        //    ItemGrid = new DeItemGrid<DeGridObject>(width, height);
        //    GridLength = gridLength;
        //}

        public DeItemGrid()
        {
            ItemGrid = new DeItemGrid<DeGridObject>(4, 4);
            GridLength = 2;
        }

        public void HandleMove(int2 startPos, int2 endPos, float time)
        {
            if(
                ItemGrid.TryGetGridItem(startPos, out var startItem)
            )
            {
                if (ItemGrid.TryGetGridItem(endPos, out var endItem))
                {
                    if (startItem is Magazine magSource && endItem is Magazine magDest)
                    {
                        OngoingActions.Add(new AmmoTransfer(time, 0.5f, startPos, endPos));
                    }
                    else if(startItem is AmmoBox && endItem is Magazine)
                    {
                        OngoingActions.Add(new AmmoTransfer(time, 0.5f, startPos, endPos));
                    }
                    else if(startItem is Magazine magazine1 && endItem is Turret turret1)
                    {
                        ItemGrid.RemoveItem(startItem);
                        OngoingActions.Add(new TurretLoadingMagazine(time, 1.5f, startPos, magazine1, turret1.Magazine, endPos));
                        turret1.SetMagazine(null, time);
                    }
                    else if (startItem is Turret turret2A && endItem is Turret turret2B && turret2A.Magazine != null && turret2B.Magazine == null)
                    {
                        // magswap
                        OngoingActions.Add(new TurretLoadingMagazine(time, 1.5f, startPos, turret2A.Magazine, null, endPos));
                        turret2A.SetMagazine(null, time);
                    }
                    else // just swap
                    {
                        ItemGrid.RemoveItem(startItem);
                        ItemGrid.RemoveItem(endItem);
                        OngoingActions.Add(new Moving(time, startItem, startPos, endPos, 0.5f));
                        OngoingActions.Add(new Moving(time, endItem, endPos, startPos, 0.5f));
                    }
                }
                else
                {
                    if (startItem is Turret turret0 && turret0.Magazine != null)
                    {
                        OngoingActions.Add(new Moving(time, turret0.Magazine, startPos, endPos, 0.5f));
                        turret0.SetMagazine(null, time);
                    }
                    else
                    {
                        ItemGrid.RemoveItem(startItem);
                        OngoingActions.Add(new Moving(time, startItem, startPos, endPos, 0.5f));
                    }
                }
            }
        }

        public bool IsPositionOccupied(int2 position)
        {
            return TryGetGridObject<DeGridObject>(position, out _);
        }
        public bool TryGetGridObject<T>(int2 position, out T gridObject) where T : DeGridObject
        {
            if (ItemGrid.TryGetGridItem(position, out var item))
            {
                // if its of type
                if (item is T t)
                {
                    gridObject = t;
                    return true;
                }
            }

            gridObject = null;
            return false;
        }
    }

    public abstract class OngoingAction
    {
        protected int2[] m_BlockingGrids = {};

        public IEnumerable<int2> BlockingGrids => m_BlockingGrids;

        protected OngoingAction(float startTime)
        {
            StartTime = startTime;
        }

        public float StartTime { get; }
    }
    
    public class Selection : OngoingAction
    {
        public int2 SelectedObjectPos { get; }

        public Selection(float startTime, int2 selectedObjectPos) : base(startTime)
        {
            SelectedObjectPos = selectedObjectPos;
        }
    }

    public class Moving : OngoingAction
    {
        public DeGridObject MovingObject { get; }
        public int2 OriginalPosition { get; }
        public int2 TargetPosition { get; }
        
        public float Duration { get; }

        public Moving(float startTime, DeGridObject movingObject, int2 originalPosition, int2 targetPosition, float duration) : base(startTime)
        {
            MovingObject = movingObject;
            OriginalPosition = originalPosition;
            TargetPosition = targetPosition;
            Duration = duration;
            
            var destinationCoordinates =
                ItemGridUtils.GetGridsFromPivotAndOffsets(originalPosition, movingObject.GetOccupations());
            var sourceCoordinates =
                ItemGridUtils.GetGridsFromPivotAndOffsets(targetPosition, movingObject.GetOccupations());
            var blockingGrids = destinationCoordinates.Union(sourceCoordinates);
            m_BlockingGrids = blockingGrids.ToArray();
        }
    }
    
    public class GridEffect : OngoingAction
    {
        public int2[] GridPositions { get; }
        public EnMsg Msg { get; }
        public float Duration { get; }

        public enum EnMsg
        {
            Positive,
            Negative,
            Warning,
            Neutral
        }


        public GridEffect(float startTime, int2[] gridPositions, EnMsg msg, float duration) : base(startTime)
        {
            GridPositions = gridPositions;
            Msg = msg;
            Duration = duration;
        }
    }
    
    public class TurretLoadingMagazine : OngoingAction
    {
        public float ActionDuration { get; }
        
        public int2 NewMagazinePositionBeforeLoad { get; } 
        
        public Magazine NewMagazine { get; } // thats being loaded to turret
        [CanBeNull] public Magazine PreviousMagazine { get; } // thats being unloaded back to NewMagazinePositionBeforeLoad 
        public int2 TurretPos { get; }
        
        public float GetProgress(float time)
        {
            return math.unlerp(StartTime, StartTime + ActionDuration, time);
        }

        public TurretLoadingMagazine(float startTime, float actionDuration, int2 newMagazinePositionBeforeLoad, Magazine newMagazine, [CanBeNull] Magazine previousMagazine, int2 turretPos) : base(startTime)
        {
            ActionDuration = actionDuration;
            NewMagazinePositionBeforeLoad = newMagazinePositionBeforeLoad;
            NewMagazine = newMagazine;
            PreviousMagazine = previousMagazine;
            TurretPos = turretPos;
            
            var blockedGrids0 = ItemGridUtils.GetGridsFromPivotAndOffsets(newMagazinePositionBeforeLoad, newMagazine.GetOccupations());
            var blockedGrids1 = ItemGridUtils.GetGridsFromPivotAndOffsets(turretPos, newMagazine.GetOccupations()); // todo what if turret is not 1x1 size?
            m_BlockingGrids = blockedGrids0.Union(blockedGrids1).ToArray();
        }
    }

    public class AmmoTransfer : OngoingAction
    {
        public float TimePerAmmoLoad { get; }
        public float LastAmmoLoadedTime { get; set; }
        public int2 AmmoSourcePos { get; }
        public int2 AmmoDestinationPos { get; }
        
        public float GetProgress(float time, int ammoBoxAmmoLeft, int magazineAmmoCount, int magazineAmmoCapacity)
        {
            var ammoLeft = ammoBoxAmmoLeft;
            var magazineCapacityLeft = magazineAmmoCapacity - magazineAmmoCount;
            
            var ammoLoadsLeft = math.min(ammoLeft, magazineCapacityLeft);
            var loadFinishTime = LastAmmoLoadedTime + ammoLoadsLeft * TimePerAmmoLoad;
            var loadStartTime = StartTime;
            
            return math.unlerp(loadStartTime, loadFinishTime, time);
        }

        public AmmoTransfer(float startTime, float timePerAmmoLoad, int2 ammoSourcePos, int2 ammoDestinationPos) : base(startTime)
        {
            TimePerAmmoLoad = timePerAmmoLoad;
            AmmoSourcePos = ammoSourcePos;
            AmmoDestinationPos = ammoDestinationPos;
            LastAmmoLoadedTime = startTime;
        }
    }
    
    
    
    public class DeItemGrid<T> where T : class, IGridItem
    {
        [ItemCanBeNull] private readonly T[] m_Occupations;
        public int Width{get;}
        public int Height{get;}
        private readonly HashSet<T> m_Items;
        private readonly Dictionary<T, int2> m_ItemPivots;
        
        
        //** lambda in dots is problematic with Entities.ForEach **
        //public void ForEachItem(Action<T, int2> action)
        //{
        //    foreach (var item in m_Items)
        //    {
        //        action(item, m_ItemPivots[item]);
        //    }
        //}
        
        // Ienumerable version
        public IEnumerable<(T item, int2 pivot)> Items
        {
            get
            {
                foreach (var item in m_Items)
                {
                    yield return (item, m_ItemPivots[item]);
                }
            }
        }

        public int2[] GetOccupyingGrids(T gridItem)
        {
            var result = new List<int2>();
            for (var i = 0; i < m_Occupations.Length; i++)
            {
                if (m_Occupations[i] != null && m_Occupations[i].Equals(gridItem))
                {
                    result.Add(new int2(i % Width, i / Width));
                }
            }

            return result.ToArray();
        } 
        
        // with bounds check
        public bool TryGetGridItem(int2 position, out T gridItem)
        {
            if (position.x < 0 || position.x >= Width || position.y < 0 || position.y >= Height)
            {
                gridItem = default;
                return false;
            }
            
            gridItem = m_Occupations[position.x + position.y * Width];
            return gridItem != null;
        }
        
        public void RemoveItem(T gridItem)
        {
            m_Items.Remove(gridItem);
            m_ItemPivots.Remove(gridItem);
            for (var i = 0; i < m_Occupations.Length; i++)
            {
                if (m_Occupations[i] == gridItem)
                {
                    m_Occupations[i] = null;
                }
            }
        }

        public bool IsSpaceAvailable(int2 pivot, IEnumerable<int2> occupations)
        {
            foreach (var occupation in occupations)
            {
                var position = pivot + occupation;
                if (position.x < 0 || position.x >= Width || position.y < 0 || position.y >= Height)
                {
                    return false;
                }

                var gridItem = m_Occupations[position.x + position.y * Width];
                if (gridItem != null)
                {
                    return false;
                }
            }

            return true;
        }
        
        public bool TryPlaceItem(int2 pivot, [NotNull] T gridItem)
        {
            if (!IsSpaceAvailable(pivot, gridItem.GetOccupations()))
            {
                return false;
            }
            
            foreach (var occupation in gridItem.GetOccupations())
            {
                var position = pivot + occupation;
                m_Occupations[position.x + position.y * Width] = gridItem;
            }
            m_Items.Add(gridItem);
            m_ItemPivots.Add(gridItem, pivot);
            return true;
        } 
        
        

        public DeItemGrid(int width, int height)
        {
            m_Occupations = new T[width * height];
            m_Items = new HashSet<T>();
            m_ItemPivots = new Dictionary<T, int2>();
            Width = width;
            Height = height;
        }
    }

    public interface IGridItem
    {
        public int2[] GetOccupations();
    }
    public abstract class DeGridObject : IGridItem
    {
        public virtual int2[] GetOccupations() => new int2[1];
        public abstract DeGridObject Clone();
    }

    public class AmmoBox : DeGridObject
    {
        public int AmmoCount { get; private set; }

        public int AmmoCapacity { get; }

        public int AmmoTier { get; }

        public int BoxTier { get; }

        public float AmmoCountChangedTime { get; private set; }
        
        public AmmoBox(int ammoCount, int ammoCapacity, int ammoTier, int boxTier)
        {
            AmmoCount = ammoCount;
            AmmoCapacity = ammoCapacity;
            AmmoTier = ammoTier;
            BoxTier = boxTier;
        }

        public void SetAmmoCount(int ammoCount, float time)
        {
            AmmoCount = ammoCount;
            AmmoCountChangedTime = time;
        }
        
        public override DeGridObject Clone()
        {
            return new AmmoBox(AmmoCount, AmmoCapacity, AmmoTier, BoxTier)
            {
                AmmoCountChangedTime = AmmoCountChangedTime,
            };
        }
    }

    public class Magazine : DeGridObject
    {
        public int AmmoCount { get; private set; }
        public int AmmoCapacity { get; }
        public int AmmoTier { get; }
        public int MagazineTier { get; }
        
        public float AmmoCountChangedTime { get; private set; }
        
        public void SetAmmoCount(int ammoCount, float time)
        {
            AmmoCount = ammoCount;
            AmmoCountChangedTime = time;
        }
        
        public Magazine(int ammoCount, int ammoCapacity, int ammoTier, int magazineTier)
        {
            AmmoCount = ammoCount;
            AmmoCapacity = ammoCapacity;
            AmmoTier = ammoTier;
            MagazineTier = magazineTier;
        }

        public override DeGridObject Clone()
        {
            return new Magazine(AmmoCount, AmmoCapacity, AmmoTier, MagazineTier)
            {
                AmmoCountChangedTime = AmmoCountChangedTime,
            };
        }
    }

    public class Turret : DeGridObject
    {
        public float LastShotTime { get; private set; }
        [CanBeNull] public Magazine Magazine { get; private set; }
        public float3 AimDirection { get; set; }
        public float LastMagazineChangedTime { get; private set; }
        
        public float FireRate { get; }
        
        //ctor with all these field
        public Turret(float fireRate, float lastShotTime, [CanBeNull] Magazine magazine, float lastMagazineChangedTime)
        {
            FireRate = fireRate;
            LastShotTime = lastShotTime;
            Magazine = magazine;
            LastMagazineChangedTime = lastMagazineChangedTime;
        }
        
        public void SetMagazine(Magazine magazine, float time)
        {
            Magazine = magazine;
            LastMagazineChangedTime = time;
        }
        
        public bool TryShoot(float time)
        {
            if(time > LastShotTime + FireRate && Magazine is { AmmoCount: > 0 })
            {
                Magazine.SetAmmoCount(Magazine.AmmoCount - 1, time);
                LastShotTime = time;
                return true;
            }
            
            return false;
        }

        public override DeGridObject Clone()
        {
            return new Turret(FireRate, LastShotTime, Magazine, LastMagazineChangedTime);
        }
    }
}