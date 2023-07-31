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
        
        public void HighlightGrids(IEnumerable<int2> grids, Color color)
        {
            // set materials of grids
            foreach (var grid in grids)
            {
                var tileRenderer = m_TileRenderers[GetGridIndex(grid)];
                // set color
                tileRenderer.material.color = color;
            }
        }
        public void HighlightGrid(int2 grid, Color color)
        {
            var tileRenderer = m_TileRenderers[GetGridIndex(grid)];
            // set color
            tileRenderer.material.color = color;
        }
        
        public void ResetHighlights()
        {
            foreach (var tileRenderer in m_TileRenderers)
            {
                tileRenderer.material.color = Color.white;
            }
        }
        
        private int GetGridIndex(int2 grid)
        {
            return grid.x + grid.y * m_Width;
        }
    }
    
}