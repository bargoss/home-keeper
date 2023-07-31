using System;
using DefaultNamespace;
using DefenderGame.Scripts.Components;
using DG.Tweening;
using JetBrains.Annotations;
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
        [CanBeNull] private MagazineGOView m_LoadedMagazineView; 
        
        
        private Vector3 m_AimDirection;

        private void Update()
        {
            if (m_AimDirection.sqrMagnitude > 0.1f)
            {
                var targetRotation = Quaternion.LookRotation(m_AimDirection, Vector3.up);
                m_BarrelAimParent.transform.rotation = Quaternion.Lerp(m_BarrelAimParent.transform.rotation, targetRotation, 0.5f * 50 * Time.deltaTime);
            }
        }

        public void SetMagazineView([CanBeNull] Magazine magazine)
        {
            if(m_LoadedMagazineView != null){
                m_LoadedMagazineView.HandleDestroy();
                m_LoadedMagazineView = null;
            }

            if (magazine != null)
            {
                m_LoadedMagazineView = PoolManager.Instance.MagazineViewPool.Get();
                m_LoadedMagazineView.Restore(magazine.AmmoCount, magazine.AmmoCapacity, magazine.AmmoTier);
                var magazineTr = m_LoadedMagazineView.transform;
                magazineTr.SetParent(m_MagazineSlot);
                var magazineSlotTr = m_MagazineSlot.transform;
                magazineTr.position = magazineSlotTr.position;
                magazineTr.rotation = magazineSlotTr.rotation;
                //magazineTr.localScale = magazineSlotTr.localScale;
            }
        }
        
        public void UpdateAimDirection(float3 aimDirection)
        {
            m_AimDirection = aimDirection;
        }

        public void Restore(Vector3 aimDirection, Magazine magazine)
        {
            m_GunShakeParent.ResetLocal();
            m_BarrelRecoilTweenParent.ResetLocal();
            m_BarrelAimParent.ResetLocal();

            SetMagazineView(magazine);
            
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

        public void AnimateShoot(int newAmmoCount)
        {
            PoolManager.Instance.PlayShootEffect(m_Muzzle.position, m_Muzzle.forward);
            
            m_BarrelRecoilTweenParent.ResetLocal();
            m_BarrelRecoilTweenParent.DOLocalMoveZ(-0.1f, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                // todo, empty case eject here
                m_BarrelRecoilTweenParent.DOLocalMoveZ(0f, 0.1f).SetEase(Ease.OutQuad);
            });
            
            if (m_LoadedMagazineView != null)
            {
                m_LoadedMagazineView.SetAmmoCount(newAmmoCount);
            }
        }
    }
}