using Components;
using HomeKeeper.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Systems
{
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
    public partial struct LifeSpanSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LifeSpan>();
        }
        
        [BurstCompile]
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

            if (!commandBuffer.IsEmpty)
            {
                commandBuffer.Playback(state.EntityManager);
            }
            commandBuffer.Dispose();
        }
        
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}