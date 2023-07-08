using HomeKeeper.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HomeKeeper.Authoring
{ 
    public class ShooterAuthoring : MonoBehaviour
    {
        public float FireRate = 1;
        public float MuzzleVelocity = 1;
        public float AccuracyAngles = 1;
        public GameObject ShootPosition;
    }
    
    public class ShooterBaker : Baker<ShooterAuthoring>
    {
        public override void Bake(ShooterAuthoring authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new Shooter()
            {
                Stats = new Shooter.ShooterStats()
                {
                    FireRate = authoring.FireRate,
                    MuzzleVelocity = authoring.MuzzleVelocity,
                    AccuracyAngles = authoring.AccuracyAngles,
                    ShootPositionEntity = GetEntity(authoring.ShootPosition, TransformUsageFlags.Dynamic)
                },
                ShootInput = false,
                LookInput = new float3(0,1,0),
                LastShotTime = 0,
                ShotThisFrame = false
            });
            
            AddComponent<ShooterView>(entity);
        }
    }
}