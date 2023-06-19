using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct DomeSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // get the command buffer or whatever we use to spawn entities here
            
            foreach (var (domeRw, localTransform, entity) in SystemAPI.Query<RefRW<Dome>, LocalTransform>().WithEntityAccess())
            {
                var dome = domeRw.ValueRO;

                var domeResult = DomeTurretBehaviour.HandleShooting(
                    dome,
                    dome.ShootInput,
                    dome.AimDirection,
                    (float)SystemAPI.Time.ElapsedTime,
                    SystemAPI.Time.fixedDeltaTime
                );

                domeRw.ValueRW = domeResult.Dome;
                if (domeResult.ShotProjectile)
                {
                    var projectileSpeed = domeResult.ShotProjectileVelocity;
                    var projectilePrefab = dome.ProjectilePrefab;
                    var projectileDamage = dome.ProjectileBaseDamage;

                    var projectileEntity = state.EntityManager.Instantiate(projectilePrefab);
                    var projectileRw = SystemAPI.GetComponentRW<Projectile>(projectileEntity);
                    var projectile = projectileRw.ValueRO;
                    
                    projectile.BaseDamage = projectileDamage;
                    
                    projectileRw.ValueRW = projectile;
                }
            }
        }
    }
    
    public static class DomeTurretBehaviour
    {
        public struct Result
        {
            public Dome Dome;
            public float3 ShotProjectileVelocity;
            public bool ShotProjectile;
        }
        
        public static Result HandleShooting(Dome dome, bool shootInput, float2 aimDirection, float time, float deltaTime)
        {
            if (math.lengthsq(aimDirection) == 0)
            {
                Debug.LogError("aimDirection is zero");
                aimDirection = new float2(0, 1);
                shootInput = false;
            }
            
            var shotProjectile = false;
            var shotProjectileVelocity = float3.zero;

            // recoil recovery
            var recoilRecovery = dome.RecoilPerSecond * deltaTime;
            dome.Recoil = math.max(0, dome.Recoil - recoilRecovery);

            // taking shot
            var fireCooldown = 1f / dome.FireRate;
            var nextShot = dome.LastShootTime + fireCooldown;
            var takingShot = time > nextShot && shootInput; 
            if(takingShot)
            {
                var accuracyAngleRange = math.lerp(dome.BestAccuracyDegrees, dome.WorstAccuracyDegrees, dome.Recoil);
                var randomAngle = UnityEngine.Random.Range(-accuracyAngleRange, accuracyAngleRange);
                var shootDirection = math.normalizesafe(aimDirection);
                var aimDirectionWithRecoil = math.mul(quaternion.Euler(0, 0, randomAngle), new float3(shootDirection.x, shootDirection.y, 0));
                
                dome.Recoil += dome.RecoilPerShot;
                dome.LastShootTime = time;
                
                shotProjectile = true;
                shotProjectileVelocity = aimDirectionWithRecoil * dome.ProjectileSpeed;
            }
            
            
            return new Result()
            {
                Dome = dome,
                ShotProjectileVelocity = shotProjectileVelocity,
                ShotProjectile = shotProjectile
            };
        }
    }


    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ProjectileSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            //SystemAPI.GetSingleton<>
            //var collisionWorld = SystemAPI.GetSingleton<BuildPhysicsWorldData>().PhysicsData.PhysicsWorld.CollisionWorld;
            //var collisions = new NativeList<DistanceHit>(Allocator.Temp);
            //
            //SystemAPI.GetSingleton<>().
            //foreach (
            //    var (projectileRw, localTransform, physicsVelocityRw, entity)
            //    in SystemAPI.Query<RefRW<Projectile>, LocalTransform, RefRW<PhysicsVelocity>>().WithEntityAccess()
            //)
            //{
            //    
            //}
        }
    }
}