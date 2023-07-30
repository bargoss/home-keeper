using DefaultNamespace;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace DefenderGame.Scripts.GoViews
{
    public class MagazineGOView : MonoBehaviour
    {
        [SerializeField] private Transform m_BulletFeed;
        [SerializeField] private Transform m_BulletBottom;
        public Transform BulletFeed => m_BulletFeed;
        public Transform BulletBottom => m_BulletBottom;

        [SerializeField] private Transform m_TopParent;
        [SerializeField] private Transform m_BottomParent;
        
        [SerializeField] private TextMeshPro m_AmmoCountText;
        
        private int m_AmmoCapacity;
        
        public void Restore(int ammoCount, int ammoCapacity, int ammoTier)
        {
            m_AmmoCapacity = ammoCapacity;
            SetAmmoCount(ammoCount);
            
            m_TopParent.ResetLocal();            
            m_BottomParent.ResetLocal();
        }
        
        public void SetAmmoCount(int ammoCount)
        {
            m_AmmoCountText.text = ammoCount + "/" + m_AmmoCapacity;
        }

        public void ShakeFromTop(float duration = 0.2f)
        {
            if (!DOTween.IsTweening(m_TopParent))
            {
                m_TopParent.DOPunchScale(new Vector3(0.1f,-0.2f, 0.1f), duration, 1, 0.5f);
            }
        }
        
        public void ShakeFromBottom(float duration = 0.2f)
        {
            if (!DOTween.IsTweening(m_BottomParent))
            {
                m_BottomParent.DOPunchScale(new Vector3(0.1f,0.2f, 0.1f), duration, 1, 0.5f);
            }
        }

        public void HandleDestroy()
        {
            PoolManager.Instance.MagazineViewPool.Release(this);
        }
    }
}