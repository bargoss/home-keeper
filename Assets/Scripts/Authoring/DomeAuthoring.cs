using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace.Authoring
{
    public class DomeAuthoring : MonoBehaviour
    {
        public float MaxHitPoints = 100;
        public float ProjectileBaseDamage = 1;
        public float ProjectileSpeed = 10;
        public float BestAccuracyDegrees = 0;
        public float WorstAccuracyDegrees = 15;
        public float RecoilPerShot = 0.1f;
        public float RecoilPerSecond = 0.5f;
        public float FireRate = 0.5f;
        public GameObject ProjectilePrefab;
    }
    
    public class DomeBaker : Baker<DomeAuthoring>
    {
        public override void Bake(DomeAuthoring authoring)
        {
            var projectilePrefabEntity = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic);
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent(entity, new Dome()
            {
                MaxHitPoints = authoring.MaxHitPoints,
                ProjectileBaseDamage = authoring.ProjectileBaseDamage,
                ProjectileSpeed = authoring.ProjectileSpeed,
                BestAccuracyDegrees = authoring.BestAccuracyDegrees,
                WorstAccuracyDegrees = authoring.WorstAccuracyDegrees,
                RecoilPerShot = authoring.RecoilPerShot,
                RecoilPerSecond = authoring.RecoilPerSecond,
                FireRate = authoring.FireRate,
                ProjectilePrefab = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.None),
                
                Recoil = 0,
                AimDirection = float2.zero,
                HitPoints = authoring.MaxHitPoints,
                ShootInput = false,
                LastShootTime = 0
            });
        }
    }
}