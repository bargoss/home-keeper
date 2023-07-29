using TMPro;
using UnityEngine;

namespace DefenderGame.Scripts.GoViews
{
    public class AmmoBoxGOView : MonoBehaviour
    {
        [SerializeField] private Transform m_BulletFeed;
        [SerializeField] private TextMeshPro m_AmmoCountText;
        public Transform BulletFeed => m_BulletFeed;
        
        private int m_AmmoCapacity;
        
        public void Restore(int ammoCount, int ammoCapacity, int ammoTier)
        {
            m_AmmoCapacity = ammoCapacity;
            SetAmmoCount(ammoCount);
            //todo
        }

        public void SetAmmoCount(int ammoCount)
        {
            m_AmmoCountText.text = ammoCount + "/" + m_AmmoCapacity;
        }
    }
}