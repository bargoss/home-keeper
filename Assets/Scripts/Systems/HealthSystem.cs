using Components;
using HomeKeeper.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct HealthSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Health>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (healthRw, entity) in SystemAPI.Query<RefRW<Health>>().WithEntityAccess())
            {
                var health = healthRw.ValueRO;
                health.Update((float)SystemAPI.Time.ElapsedTime);
                
                
                if (health is { HitPoints: <= 0, DestroyOnDeath: true })
                {
                    commandBuffer.DestroyEntity(entity);
                }
                
                healthRw.ValueRW = health;
            }
            
            if (!commandBuffer.IsEmpty)
            {
                commandBuffer.Playback(state.EntityManager);
            }
        }
        
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}