using Components;
using HomeKeeper.Components;
using Unity.Collections;
using Unity.Entities;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct LifeSpanSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (lifeSpanRw, entity) in SystemAPI.Query<RefRW<LifeSpan>>().WithEntityAccess())
            {
                var lifeSpan = lifeSpanRw.ValueRO;
                lifeSpan.SecondsToLive -= SystemAPI.Time.fixedDeltaTime;
                
                if(lifeSpan.SecondsToLive <= 0)
                {
                    //state.EntityManager.DestroyEntity(entity);
                    commandBuffer.DestroyEntity(entity);
                }
                
                lifeSpanRw.ValueRW = lifeSpan;
            }
            
            commandBuffer.Playback(state.EntityManager);
        }        
    }
}