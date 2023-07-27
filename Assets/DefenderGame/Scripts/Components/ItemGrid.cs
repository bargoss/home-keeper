﻿using System;
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
        public ItemGrid ItemGrid { get; }
        public List<OngoingAction> OngoingActions { get; } = new(); 
    }

    public abstract class OngoingAction
    {
        public float StartTime { get; }
        public float Duration { get; }
        public float GetProgress(float time) => math.unlerp(StartTime, StartTime + Duration, time);
    }

    public class TurretLoadingMagazine : OngoingAction
    {
        public int2 NewMagazinePositionBeforeLoad { get; } 
        
        public Magazine NewMagazine { get; }
        [CanBeNull] public Magazine PreviousMagazine { get; }
        public Turret Turret { get; }
    }

    public class AmmoBoxFillingMagazine : OngoingAction
    {
        public AmmoBox AmmoBox { get; }
        public Magazine Magazine { get; }
    }
    
    
    
    public class ItemGrid
    {
        [ItemCanBeNull] private readonly GridItem[] m_Occupations;
        private readonly int m_Width;
        private readonly int m_Height;
        private readonly HashSet<GridItem> m_Items;
        private readonly Dictionary<GridItem, int2> m_ItemPivots;
        
        public void ForEachItem(Action<GridItem, int2> action)
        {
            foreach (var item in m_Items)
            {
                action(item, m_ItemPivots[item]);
            }
        }

        public int2[] GetOccupyingGrids(GridItem gridItem)
        {
            var result = new List<int2>();
            for (var i = 0; i < m_Occupations.Length; i++)
            {
                if (m_Occupations[i] == gridItem)
                {
                    result.Add(new int2(i % m_Width, i / m_Width));
                }
            }

            return result.ToArray();
        } 
        
        // with bounds check
        public bool TryGetGridItem(int2 position, out GridItem gridItem)
        {
            if (position.x < 0 || position.x >= m_Width || position.y < 0 || position.y >= m_Height)
            {
                gridItem = default;
                return false;
            }
            
            gridItem = m_Occupations[position.x + position.y * m_Width];
            return gridItem != null;
        }
        
        public void RemoveSocketOccupier(GridItem gridItem)
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
                if (position.x < 0 || position.x >= m_Width || position.y < 0 || position.y >= m_Height)
                {
                    return false;
                }

                var gridItem = m_Occupations[position.x + position.y * m_Width];
                if (gridItem != null)
                {
                    return false;
                }
            }

            return true;
        }
        
        public bool TryPlaceSocketOccupier(int2 pivot, GridItem gridItem)
        {
            if (!IsSpaceAvailable(pivot, gridItem.Occupations))
            {
                return false;
            }
            
            foreach (var occupation in gridItem.Occupations)
            {
                var position = pivot + occupation;
                m_Occupations[position.x + position.y * m_Width] = gridItem;
            }
            m_Items.Add(gridItem);
            m_ItemPivots.Add(gridItem, pivot);
            return true;
        } 
        
        

        public ItemGrid(int width, int height)
        {
            m_Occupations = new GridItem[width * height];
            m_Items = new HashSet<GridItem>();
            m_ItemPivots = new Dictionary<GridItem, int2>();
            m_Width = width;
            m_Height = height;
        }
    }

    public abstract class GridItem
    {
        public virtual int2[] Occupations => new int2[1]{new int2(0,0)};
    }

    public class AmmoBox : GridItem
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

    public class Magazine : GridItem
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

    public class Turret : GridItem
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