using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefenderGame.Scripts.GoViews;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;

namespace DefenderGame.Scripts.Components
{
    public struct Socket : IComponentData
    {
        public readonly int SocketIndex;
        
        public Socket(int socketIndex)
        {
            SocketIndex = socketIndex;
        }
    }

    public class DeItemGrid : IComponentData
    {
        public DeItemGrid(ItemGrid<DeGridObject> itemGrid)
        {
            ItemGrid = itemGrid;
        }

        public ItemGrid<DeGridObject> ItemGrid { get; }
        public HashSet<OngoingAction> OngoingActions { get; } = new();
        
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
        }
    }

    public class AmmoBoxFillingMagazine : OngoingAction
    {
        public float TimePerAmmoLoad { get; }
        public float LastAmmoLoadedTime { get; set; }
        public int2 AmmoBoxPos { get; }
        public int2 MagazinePos { get; }
        
        public float GetProgress(float time, int ammoBoxAmmoLeft, int magazineAmmoCount, int magazineAmmoCapacity)
        {
            var ammoLeft = ammoBoxAmmoLeft;
            var magazineCapacityLeft = magazineAmmoCapacity - magazineAmmoCount;
            
            var ammoLoadsLeft = math.min(ammoLeft, magazineCapacityLeft);
            var loadFinishTime = LastAmmoLoadedTime + ammoLoadsLeft * TimePerAmmoLoad;
            var loadStartTime = StartTime;
            
            return math.unlerp(loadStartTime, loadFinishTime, time);
        }

        public AmmoBoxFillingMagazine(float startTime, float timePerAmmoLoad, int2 ammoBoxPos, int2 magazinePos) : base(startTime)
        {
            TimePerAmmoLoad = timePerAmmoLoad;
            AmmoBoxPos = ammoBoxPos;
            MagazinePos = magazinePos;
            LastAmmoLoadedTime = startTime;
        }
    }
    
    
    
    public class ItemGrid<T> where T : class, IGridItem
    {
        [ItemCanBeNull] private readonly T[] m_Occupations;
        public int Width{get;}
        public int Height{get;}
        private readonly HashSet<T> m_Items;
        private readonly Dictionary<T, int2> m_ItemPivots;
        
        public void ForEachItem(Action<T, int2> action)
        {
            foreach (var item in m_Items)
            {
                action(item, m_ItemPivots[item]);
            }
        }

        public int2[] GetOccupyingGrids(T gridItem)
        {
            var result = new List<int2>();
            for (var i = 0; i < m_Occupations.Length; i++)
            {
                if (m_Occupations[i].Equals(gridItem))
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
        
        public void RemoveSocketOccupier(T gridItem)
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
        
        public bool TryPlaceItem(int2 pivot, T gridItem)
        {
            if (!IsSpaceAvailable(pivot, gridItem.Occupations))
            {
                return false;
            }
            
            foreach (var occupation in gridItem.Occupations)
            {
                var position = pivot + occupation;
                m_Occupations[position.x + position.y * Width] = gridItem;
            }
            m_Items.Add(gridItem);
            m_ItemPivots.Add(gridItem, pivot);
            return true;
        } 
        
        

        public ItemGrid(int width, int height)
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
        public int2[] Occupations { get; }
    }
    public abstract class DeGridObject : IGridItem
    {
        public virtual int2[] Occupations { get; } = new int2[1];
    }

    public class AmmoBox : DeGridObject
    {
        public int AmmoCount { get; private set; }
        public int AmmoCapacity { get; }
        public int AmmoTier { get; }
        public int BoxTier { get; }
        
        public float AmmoCountChangedTime { get; private set; }
        
        public void SetAmmoCount(int ammoCount, float time)
        {
            AmmoCount = ammoCount;
            AmmoCountChangedTime = time;
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
    }

    public class Turret : DeGridObject
    {
        public float LastShotTime { get; private set; }
        public Magazine Magazine { get; private set; }
        public float3 AimDirection { get; set; }
        public float LastMagazineChangedTime { get; private set; }
        
        public float FireRate { get; }
        
        public void SetMagazine(Magazine magazine, float time)
        {
            Magazine = magazine;
            LastMagazineChangedTime = time;
        }
        
        public bool TryShoot(float time)
        {
            if(time > LastShotTime + FireRate && Magazine.AmmoCount > 0)
            {
                Magazine.SetAmmoCount(Magazine.AmmoCount - 1, time);
                LastShotTime = time;
                return true;
            }
            
            return false;
        }
    }
    

    /*
    Requirements:
        Item Entity should be able access other items in SocketGrid  
     */
}