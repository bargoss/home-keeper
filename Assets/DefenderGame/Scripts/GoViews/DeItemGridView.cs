using System.Collections.Generic;
using DefenderGame.Scripts.Systems;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DefenderGame.Scripts.GoViews
{
    public class DeItemGridView : MonoBehaviour
    {
        [SerializeField] private Transform m_TilePrefab;
        
        private readonly List<Transform> m_Tiles = new();

        public void Restore(int width, int height, float gridLength)
        {
            m_Tiles.ForEach(tile => Destroy(tile.gameObject));
            m_Tiles.Clear();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++) 
                {
                    var worldPos = ItemGridUtils.GridToWorldPos(new int2(x,y), new LocalToWorld()
                    {
                        Value = transform.localToWorldMatrix
                    },gridLength);
                    
                    var tile = Instantiate(m_TilePrefab, transform);
                    tile.position = worldPos;
                    tile.localScale = new Vector3(gridLength, 1, gridLength);
                    m_Tiles.Add(tile);
                }
            }
        }

        public void HighlightGrids(int2[] grids)
        {
            
        }
        public void ResetHighlights()
        {
            
        }
    }
    
}