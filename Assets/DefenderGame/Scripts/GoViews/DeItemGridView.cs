using System.Collections.Generic;
using BulletCircle.GoViews;
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

        public void Restore(int width, int height)
        {
            
        }

    }
    
}