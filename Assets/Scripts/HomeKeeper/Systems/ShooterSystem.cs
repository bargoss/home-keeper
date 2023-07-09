using System.Numerics;
using DefaultNamespace;
using HomeKeeper.Components;
using Unity.Collections;
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
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (shooterRw, localToWorld, localTransformRw,entity) in SystemAPI.Query<RefRW<Shooter>, LocalToWorld, RefRW<LocalTransform>>().WithEntityAccess())
            {
                var shooter = shooterRw.ValueRO;
                var localTransform = localTransformRw.ValueRO;
                
                shooter.ShotThisFrame = false; // reset
                
                // look
                HandleLook(ref shooter, localToWorld, SystemAPI.Time.DeltaTime);

                // shoot
                if (shooter.ShootInput)
                {
                    // todo
                    /*
                    if (TryGetMagazineRw(grabObjectSocket.HeldItemOpt, out var magazineRw))
                    {
                        var magazine = magazineRw.ValueRO;
                        
                        HandleShoot(magazine, ref shooter, localToWorld, ref commandBuffer);
                        
                        magazineRw.ValueRW = magazine;
                    }
                    */
                    var placeHolderProjectilePrefab = SystemAPI.GetSingleton<GameResourcesUnmanaged>().ProjectilePrefab;
                    HandleShoot(
                        new Magazine { Capacity = 99, Current = 99, ProjectilePrefab = placeHolderProjectilePrefab },
                        ref shooter,
                        localToWorld,
                        ref commandBuffer,
                        (float)SystemAPI.Time.ElapsedTime,
                        placeHolderProjectilePrefab,
                        SystemAPI.GetComponent<LocalToWorld>(shooter.Stats.ShootPositionEntity).Position
                    );
                }

                shooterRw.ValueRW = shooter;
                localTransformRw.ValueRW = localTransform;
            }
            
            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }

        private void HandleShoot(Magazine magazine, ref Shooter shooter, LocalToWorld localToWorld,
            ref EntityCommandBuffer commandBuffer, float elapsedTime, Entity projectilePrefab, float3 shootPosition)
        {
            if (magazine.Current > 0)
            {
                var cooldown = 1.0f / shooter.Stats.FireRate;
                var cooldownFinished = elapsedTime > shooter.LastShotTime + cooldown;

                if (cooldownFinished)
                {
                    //var shootPosition = SystemAPI.GetComponent<LocalToWorld>(shooter.Stats.ShootPositionEntity).Position
                    var shootVelocity = shooter.Look * shooter.Stats.MuzzleVelocity;
                    CreateProjectile(
                        shootPosition,
                        shootVelocity,
                        projectilePrefab,
                        ref commandBuffer
                    );
                    shooter.LastShotTime = elapsedTime;
                    shooter.ShotThisFrame = true;
                }
            }
        }

        private void HandleLook(ref Shooter shooter, LocalToWorld localToWorld, float deltaTime)
        {
            var lookInput = shooter.LookInput;
            if (math.lengthsq(lookInput) < 0.0001f) // if its zero, make it forward
            {
                lookInput = localToWorld.Forward;
            }
            
            shooter.Look = math.normalizesafe(math.lerp(shooter.Look, shooter.LookInput, 1.0f * deltaTime));
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
        /*
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
        */
    }
}