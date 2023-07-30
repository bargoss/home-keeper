using System.Collections.Generic;
using DefenderGame.Scripts.Systems;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

namespace DefenderGame.Scripts.GoViews
{
    public class DeItemGridView : MonoBehaviour
    {
        [SerializeField] private Transform m_TilePrefab;
        
        private readonly List<Transform> m_Tiles = new();
        private readonly List<MeshRenderer> m_TileRenderers = new();
        
        [SerializeField] private Material m_TileMaterial;
        [SerializeField] private Material m_TileSelectedMaterial;
        
        private readonly HashSet<int2> m_HighlightedGrids = new();
        
        private int m_Width;
        private int m_Height;
        private float m_GridLength;

        public void Restore(int width, int height, float gridLength)
        {
            m_Width = width;
            m_Height = height;
            m_GridLength = gridLength;
            m_Tiles.ForEach(tile => Destroy(tile.gameObject));
            m_Tiles.Clear();
            m_TileRenderers.Clear();
            m_HighlightedGrids.Clear();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var worldPos = ItemGridUtils.GridToWorldPos(new int2(x,y), new LocalToWorld()
                    {
                        Value = transform.localToWorldMatrix
                    },gridLength);
                    
                    var tile = Instantiate(m_TilePrefab, transform);
                    tile.position = worldPos;
                    tile.localScale = new Vector3(gridLength, 1, gridLength);
                    m_Tiles.Add(tile);
                    m_TileRenderers.Add(tile.GetComponentInChildren<MeshRenderer>());
                }
            }
        }
        
        public void HighlightGrids(int2[] grids)
        {
            // set materials of grids
            foreach (var grid in grids)
            {
                var tileRenderer = m_TileRenderers[GetGridIndex(grid)];
                tileRenderer.material = m_TileSelectedMaterial;
            }

            m_HighlightedGrids.AddRange(grids);
        }
        
        public void ResetHighlights()
        {
            foreach (var grid in m_HighlightedGrids)
            {
                var tileRenderer = m_TileRenderers[GetGridIndex(grid)];
                tileRenderer.material = m_TileMaterial;
            }
        }
        
        private int GetGridIndex(int2 grid)
        {
            return grid.x + grid.y * m_Width;
        }
    }
    
}