using System.Collections.Generic;
using Unity.Mathematics;
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
            
            var tileCount = width * height;
            for (var i = 0; i < tileCount; i++)
            {
                var tile = Instantiate(m_TilePrefab, transform);
                tile.localScale = Vector3.one * gridLength;
                m_Tiles.Add(tile);
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