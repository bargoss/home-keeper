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
        // view prefabs:
        [SerializeField] private MagazineGOView m_MagazinePrefab;
        [SerializeField] private AmmoCaseGOView m_AmmoCasePrefab;
        [SerializeField] private ShooterGOView m_ShooterPrefab;
        
        private ItemGrid m_ItemGrid;

        public void Restore(int width, int height)
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