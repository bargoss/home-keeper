using System.Collections.Generic;
using BulletCircle.GoViews;
using DefenderGame.Scripts.Components;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefenderGame.Scripts.GoViews
{
    public class DeItemGridView : MonoBehaviour
    {
        [SerializeField] private GameObject GridItemPrefab;
        
        public void Restore(int width, int height, float gridLength)
        {
            
        }

        public void HighlightGrids(int2[] grids)
        {
            
        }
        public void ResetHighlights()
        {
            
        }
    }
    
}