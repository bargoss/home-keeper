    using System.Collections.Generic;
    using DefenderGame.Scripts.GoViews;
    using DG.Tweening;
    using HomeKeeper.Components;
    using UnityEngine;

    public class PoolManager
    {
        private static PoolManager m_Instance;
        public static PoolManager Instance => m_Instance ??= new PoolManager();

        private readonly BasicPool<ParticleSystem> m_ImpactParticleEffectPool;
        private readonly BasicPool<ParticleSystem> m_SmallShockEffectPool;
        private readonly BasicPool<ParticleSystem> m_ShootEffectPool;
        private readonly BasicPool<ParticleSystem> m_BloodEffectPool;
            
        public readonly BasicPool<BulletGOView> BulletViewPool;
        public readonly BasicPool<MagazineGOView> MagazineViewPool;

        private PoolManager()
        {
            var poolsParent = new GameObject("PoolParent").transform;
            m_ImpactParticleEffectPool = new BasicPool<ParticleSystem>(GameResources.Instance.ImpactParticleEffectPrefab, poolsParent);
            m_SmallShockEffectPool = new BasicPool<ParticleSystem>(GameResources.Instance.SmallShockEffectPrefab, poolsParent);
            m_ShootEffectPool = new BasicPool<ParticleSystem>(GameResources.Instance.ShootEffectPrefab, poolsParent);
            m_BloodEffectPool = new BasicPool<ParticleSystem>(GameResources.Instance.BloodEffectPrefab, poolsParent);
            
            BulletViewPool = new BasicPool<BulletGOView>(GameResources.Instance.BulletGoView, poolsParent);
            MagazineViewPool = new BasicPool<MagazineGOView>(GameResources.Instance.MagazineGOViewPrefab, poolsParent);
        }
        
        public void PlayBloodEffect(Vector3 position, Vector3 forward)
        {
            PlayEffect(position, forward, m_BloodEffectPool);
        }
        
        public void PlayImpactEffect(Vector3 position, Vector3 forward)
        {
            PlayEffect(position, forward, m_ImpactParticleEffectPool);
        }
        
        public void PlaySmallShockEffect(Vector3 position, Vector3 forward)
        {
            PlayEffect(position, forward, m_SmallShockEffectPool);
        }
        
        public void PlayShootEffect(Vector3 position, Vector3 forward)
        {
            PlayEffect(position, forward, m_ShootEffectPool);
        }

        private void PlayEffect(Vector3 position, Vector3 forward, BasicPool<ParticleSystem> effectPool)
        {
            var particle = effectPool.Get();
            particle.transform.position = position;
            particle.transform.rotation = Quaternion.LookRotation(forward);
            particle.Stop(true);
            particle.Play(true);
            
            DOTween.Kill(particle.transform);
            DOTween.Sequence(particle.transform)
                .AppendInterval(0.3f)
                .AppendCallback(() => effectPool.Release(particle));
        }
    }

    public class BasicPool<T> : PoolerBase<T> where T : Component
    {
        public BasicPool(T prefab, Transform parent) : base(prefab, 10, 20, true, parent)
        {
        }
    }
    public class ParticleEffectPool : PoolerBase<ParticleSystem>
    {
        public ParticleEffectPool(ParticleSystem prefab, Transform parent) : base(prefab, 10, 20, true, parent)
        {
        }

        protected override void GetSetup(ParticleSystem particleSystem)
        {
            base.GetSetup(particleSystem);
            particleSystem.Stop();
            particleSystem.Play();
        }

        protected override void ReleaseSetup(ParticleSystem particleSystem)
        {
            base.ReleaseSetup(particleSystem);
            particleSystem.Stop();
        }
    }