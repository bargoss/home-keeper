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
        private ItemGrid m_ItemGrid;
        private DeItemGridView m_View;

        public DeItemGrid(int width, int height, DeItemGridView view)
        {
            m_ItemGrid = new ItemGrid(width, height);
            m_View = view;
        }
    }
    
    public class ItemGrid
    {
        [ItemCanBeNull] private readonly GridItem[] m_Occupations;
        private readonly int m_Width;
        private readonly int m_Height;
        private readonly HashSet<GridItem> m_Items;
        private readonly Dictionary<GridItem, int2> m_ItemPivots;
        
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
    

    /*
    Requirements:
        Item Entity should be able access other items in SocketGrid  
     */
}