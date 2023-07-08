using DefaultNamespace;
using HomeKeeper.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace HomeKeeper.Systems
{
    public partial struct GameManagerSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var mousePositionInWorldSpace = Utility.GetMousePositionInWorldSpace();
            var shootInput = Input.GetMouseButton(0);

            int shooterCount = 0;
            
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
            
            // control all shooters
            foreach (var (shooterRw, localToWorld) in SystemAPI.Query<RefRW<Shooter>, LocalToWorld>())
            {
                var shooter = shooterRw.ValueRO;
                var shooterPosition = localToWorld.Position;
                var direction = math.normalizesafe(mousePositionInWorldSpace - shooterPosition);
                
                shooter.LookInput = direction;
                shooter.ShootInput = shootInput;
                
                shooterRw.ValueRW = shooter;
                
                shooterCount++;
            }

            if (shooterCount == 0)
            {
                var shooterPrefab = SystemAPI.GetSingleton<GameResourcesUnmanaged>().ShooterPrefab;
                var shooter = entityCommandBuffer.Instantiate(shooterPrefab);
                entityCommandBuffer.SetLocalPositionRotation(shooter, float3.zero, quaternion.identity);
            }
            
            
            // todo: handle enemy spawning
            
            
            entityCommandBuffer.Playback(state.EntityManager);
        }
    }
}