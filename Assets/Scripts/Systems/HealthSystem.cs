using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct HealthSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (healthRw, entity) in SystemAPI.Query<RefRW<Health>>().WithEntityAccess())
            {
                var health = healthRw.ValueRO;
                if (health.HitPoints > 0)
                {
                    health.HitPoints = math.clamp(health.HitPoints, 0, health.MaxHitPoints);
                    healthRw.ValueRW = health;
                }
                else
                {
                    if(health.DestroyOnDeath){
                        commandBuffer.DestroyEntity(entity);
                    }
                }
            }
            
            commandBuffer.Playback(state.EntityManager);
        }
    }
}