using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BulletCircle.GoViews
{
    public class ShooterView : MonoBehaviour
    {
        /*
        * Hierarchy:
         *   Parent
         *      Body
         *          Barrel
         *          BarrelBackPos
         *          BarrelFrontPos
         * 
         *          Slide
         *              Chamber
         *          SlideBackPos
         *          SlideFrontPos
         *
         *          BulletLoadPosition
         *          
        */
       
        [SerializeField] private Transform m_Parent;
        [SerializeField] private Transform m_Body;
        [SerializeField] private Transform m_Barrel;
        [SerializeField] private Transform m_Slide;
        [SerializeField] private Transform m_Chamber;
        
        
        [SerializeField] private Transform m_ExplosionVfxPoint;
        [SerializeField] private Transform m_BodyBasePos;
        [SerializeField] private Transform m_BulletLoadPosition;
        [SerializeField] private Transform m_BarrelBackPos;
        [SerializeField] private Transform m_BarrelFrontPos;
        [SerializeField] private Transform m_SlideBackPos;
        [SerializeField] private Transform m_SlideFrontPos;
        
        [SerializeField] private GameObject m_EmptyShellPrefab;
        [SerializeField] private GameObject m_ExplosionVfxPrefab;
        
        [SerializeField] private Transform m_Bullet0;
        [SerializeField] private Transform m_Bullet1;
        

        public float BodyRecoilDistance = 0.5f;
        public float EjectForce = 300;
        public float EjectForcePosition = 0.1f;

        private void Awake()
        {
            ResetAnimation();
        }

        public void UpdateLookDirection(Vector3 aimDirection)
        {
            m_Parent.transform.rotation = Quaternion.LookRotation(aimDirection, Vector3.up);
        }

        public void ShootAnimation(float animationDuration)
        {
            ResetAnimation();
            ShootVfx();
            var scaler = animationDuration / 0.8f;
            
            DOTween.Sequence(m_Barrel)
                .Append(m_Barrel.DOLocalMove(m_BarrelBackPos.localPosition, 0.075f).SetEase(Ease.OutCirc))
                .AppendInterval(0.1f * scaler)
                .Append(m_Barrel.DOLocalMove(m_BarrelFrontPos.localPosition, 0.55f * scaler));
            
            DOTween.Sequence(m_Slide)
                .AppendInterval(0.025f * scaler)
                .Append(m_Slide.DOLocalMove(m_SlideBackPos.localPosition, 0.15f * scaler).OnUpdate(() =>
                {
                    m_Bullet0.transform.position = m_Chamber.transform.position;
                    m_Bullet0.transform.rotation = m_Chamber.transform.rotation;
                }))
                .AppendCallback(() =>
                {
                    BulletEjectionAnimation();
                    m_Bullet0.gameObject.SetActive(false);

                    m_Bullet1.transform.SetParent(m_Chamber);
                })
                .AppendInterval(0.2f * scaler)
                .Append(m_Slide.DOLocalMove(m_SlideFrontPos.localPosition, 0.15f * scaler))
                .Join(m_Bullet1.DOLocalMove(Vector3.zero, 0.15f * scaler))
                .Join(m_Bullet1.DOLocalRotateQuaternion(Quaternion.identity, 0.15f * scaler));
            
            DOTween.Sequence(m_Body)
                .AppendInterval(0.2f)
                .Append(m_Body.DOLocalMove(m_BodyBasePos.localPosition - Vector3.forward * BodyRecoilDistance, 0.3f * scaler))
                .Append(m_Body.DOLocalMove(m_BodyBasePos.localPosition, 0.3f * scaler));
                
            
            /*
            DOTween.Sequence(gameObject)
                // shoot
                .Append(m_Barrel.DOLocalMove(m_BarrelBackPos.localPosition, 0.15f * scaler))
                .Append(m_Slide.DOLocalMove(m_SlideBackPos.localPosition, 0.15f * scaler).OnUpdate(() =>
                {
                    m_Bullet1.transform.position = m_Chamber.transform.position;
                    m_Bullet1.transform.rotation = m_Chamber.transform.rotation;
                }))
                .AppendCallback(() =>
                {
                    BulletEjectionAnimation();
                    m_Bullet1.gameObject.SetActive(false);
                })
                .Append(m_Body.DOLocalMove(m_BodyBasePos.localPosition - Vector3.forward * BodyRecoilDistance, 0.15f * scaler))

                // recover
                .Append(m_Body.DOLocalMove(m_BodyBasePos.localPosition, 0.15f * scaler))
                .AppendCallback(() => m_Bullet0.transform.SetParent(m_Chamber))
                .Join(m_Bullet0.DOLocalMove(Vector3.zero, 0.8f * scaler))
                .Join(m_Bullet0.DOLocalRotateQuaternion(Quaternion.identity, 0.8f * scaler))
                .Join(m_Slide.DOLocalMove(m_SlideFrontPos.localPosition, 0.15f * scaler))
                .Join(m_Barrel.DOLocalMove(m_BarrelFrontPos.localPosition, 0.15f * scaler));
                */
        }

        private void BulletEjectionAnimation()
        {
            var pos = m_Chamber.transform.position;
            var rot = m_Chamber.transform.rotation;
            
            var shell = Instantiate(m_EmptyShellPrefab, pos, rot);
            shell.transform.localScale = Vector3.one;
            shell.GetComponent<Rigidbody>().AddForceAtPosition((m_Body.transform.up * 0.8f - m_Body.transform.forward * 0.2f + Random.insideUnitSphere * 0.1f) * EjectForce, pos + shell.transform.forward * EjectForcePosition);
        }

        private void ResetAnimation()
        {
            DOTween.Kill(gameObject,true);
            DOTween.Kill(m_Barrel, true);
            DOTween.Kill(m_Slide, true);
            DOTween.Kill(m_Body, true);
            
            m_Bullet0.gameObject.SetActive(true);
            m_Bullet1.gameObject.SetActive(true);
            m_Bullet0.transform.SetParent(m_Body);
            m_Bullet1.transform.SetParent(m_Body);
            
            
            m_Bullet0.transform.localPosition = m_BulletLoadPosition.localPosition;
            m_Bullet1.transform.localPosition = m_BulletLoadPosition.localPosition;
            m_Bullet0.transform.localRotation = m_BulletLoadPosition.localRotation;
            m_Bullet1.transform.localRotation = m_BulletLoadPosition.localRotation;
            m_Bullet0.transform.localScale = Vector3.one;
            m_Bullet1.transform.localScale = Vector3.one;

            m_Barrel.transform.localPosition = m_BarrelFrontPos.localPosition;
            m_Slide.transform.localPosition = m_SlideFrontPos.localPosition;
            m_Body.transform.localPosition = m_BodyBasePos.localPosition;
            
            m_Barrel.transform.localRotation = m_BarrelFrontPos.localRotation;
            m_Slide.transform.localRotation = m_SlideFrontPos.localRotation;
            m_Body.transform.localRotation = m_BodyBasePos.localRotation;
            
            m_Barrel.localScale = Vector3.one;
            m_Slide.localScale = Vector3.one;
            m_Body.localScale = Vector3.one;
        }

        private void ShootVfx()
        {
            var explosion = Instantiate(m_ExplosionVfxPrefab, m_ExplosionVfxPoint.position, m_ExplosionVfxPoint.rotation);
            explosion.transform.localScale = m_ExplosionVfxPoint.localScale;
        } 
    }
}