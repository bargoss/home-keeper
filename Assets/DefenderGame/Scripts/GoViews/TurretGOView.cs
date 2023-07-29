using System;
using DefaultNamespace;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefenderGame.Scripts.GoViews
{
    public class TurretGOView : MonoBehaviour
    {
        [SerializeField] private Transform m_GunShakeParent;
        [SerializeField] private Transform m_BarrelRecoilTweenParent;
        [SerializeField] private Transform m_BarrelAimParent;
        
        [SerializeField] private Transform m_Muzzle;
        [SerializeField] private Transform m_MagazineSlot;
        public Transform MagazineSlot => m_MagazineSlot;
        
        
        private Vector3 m_AimDirection;

        private void Update()
        {
            if (m_AimDirection.sqrMagnitude > 0.1f)
            {
                var targetRotation = Quaternion.LookRotation(m_AimDirection, Vector3.up);
                m_BarrelAimParent.transform.rotation = Quaternion.Lerp(m_BarrelAimParent.transform.rotation, targetRotation, 0.5f * 50 * Time.deltaTime);
            }
        }

        public void UpdateAimDirection(float3 aimDirection)
        {
            m_AimDirection = aimDirection;
        }

        public void Restore(Vector3 aimDirection)
        {
            m_GunShakeParent.ResetLocal();
            m_BarrelRecoilTweenParent.ResetLocal();
            m_BarrelAimParent.ResetLocal();
            
            m_AimDirection = aimDirection;
            if (m_AimDirection.sqrMagnitude > 0.1f)
            {
                m_BarrelAimParent.transform.rotation = Quaternion.LookRotation(m_AimDirection, Vector3.up);
            }
        }

        public void AnimateShake()
        {
            DOTween.Kill(m_GunShakeParent);
            m_GunShakeParent.ResetLocal();
            m_GunShakeParent.DOShakePosition(0.5f, 0.05f, 10, 90, false, true);
        }

        public void AnimateShoot()
        {
            PoolManager.Instance.PlayShootEffect(m_Muzzle.position, m_Muzzle.forward);
            
            m_BarrelRecoilTweenParent.ResetLocal();
            m_BarrelRecoilTweenParent.DOLocalMoveZ(-0.1f, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                // todo, empty case eject here
                m_BarrelRecoilTweenParent.DOLocalMoveZ(0f, 0.1f).SetEase(Ease.OutQuad);
            });
        }
    }
}