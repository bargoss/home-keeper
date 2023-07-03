using System.Numerics;
using DefaultNamespace;
using HomeKeeper.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace HomeKeeper.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ShooterSystem: ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer();
            foreach (var (shooterRw, localToWorld, localTransformRw,entity) in SystemAPI.Query<RefRW<Shooter>, LocalToWorld, RefRW<LocalTransform>>().WithEntityAccess())
            {
                var shooter = shooterRw.ValueRO;
                var localTransform = localTransformRw.ValueRO;
                
                // look
                HandleLook(shooter, localToWorld, ref localTransform);

                // shoot
                if (shooter.ShootInput)
                {
                    if (TryGetMagazineRw(shooter.AttachedMagazine, out var magazineRw))
                    {
                        var magazine = magazineRw.ValueRO;
                        
                        HandleShoot(magazine, ref shooter, localToWorld, ref commandBuffer);
                        
                        magazineRw.ValueRW = magazine;
                    }
                }
                
                
                shooterRw.ValueRW = shooter;
                localTransformRw.ValueRW = localTransform;
            }
            
            commandBuffer.Playback(state.EntityManager);
        }

        private void HandleShoot(Magazine magazine, ref Shooter shooter, LocalToWorld localToWorld,
            ref EntityCommandBuffer commandBuffer)
        {
            if (magazine.Current > 0)
            {
                var cooldown = 1.0f / shooter.Stats.FireRate;
                var cooldownFinished = (float)SystemAPI.Time.ElapsedTime > shooter.LastShotTime + cooldown;

                if (cooldownFinished)
                {
                    var shootPosition = SystemAPI.GetComponent<LocalToWorld>(shooter.Stats.ShootPositionEntity).Position;
                    var shootVelocity = localToWorld.Forward * shooter.Stats.MuzzleVelocity;
                    CreateProjectile(
                        shootPosition,
                        shootVelocity,
                        SystemAPI.GetSingleton<GameResources>().ProjectilePrefab,
                        ref commandBuffer
                    );
                    shooter.LastShotTime = (float)SystemAPI.Time.ElapsedTime;
                }
            }
        }

        private void HandleLook(Shooter shooter, LocalToWorld localToWorld, ref LocalTransform localTransform)
        {
            var lookInput = shooter.LookInput;
            if (math.lengthsq(lookInput) < 0.0001f) // if its zero, make it forward
            {
                lookInput = localToWorld.Forward;
            }

            var targetRotation = quaternion.LookRotationSafe(
                shooter.LookInput + localToWorld.Forward * 0.001f,
                new float3(0, 1, 0)
            );
            localTransform.Rotation = math.slerp(localTransform.Rotation, targetRotation, 1.0f * SystemAPI.Time.DeltaTime);
        }

        private Entity CreateProjectile(float3 position, float3 velocity, Entity prefab, ref EntityCommandBuffer entityCommandBuffer)
        {
            var projectile = entityCommandBuffer.Instantiate(prefab);
            
            entityCommandBuffer.SetComponent(projectile, new PhysicsVelocity()
            {
                Linear =  velocity,
                Angular = float3.zero
            });
            entityCommandBuffer.SetLocalPositionRotation(
                projectile,
                position,
                quaternion.LookRotationSafe(math.normalizesafe(velocity), new float3(0,1,0))
            );
            
            return projectile;
        }
        private bool TryGetMagazineRw(Entity magazineEntity, out RefRW<Magazine> magazineRw)
        {
            magazineRw = default;
            if (!SystemAPI.Exists(magazineEntity))
            {
                return false;
            }

            if (!SystemAPI.HasComponent<Magazine>(magazineEntity))
            {
                Debug.LogError($"Entity {magazineEntity} does not have a magazine component but is being used as a magazine");
                return false;
            }

            magazineRw = SystemAPI.GetComponentRW<Magazine>(magazineEntity);
            return true;
        }
    }
}