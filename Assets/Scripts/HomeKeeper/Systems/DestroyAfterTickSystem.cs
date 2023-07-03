using HomeKeeper.Components;
using Unity.Collections;
using Unity.Entities;

namespace HomeKeeper.Systems
{
    // run just after SimulationSystemGroup
    
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct DestroyAfterTickSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (destroyAfterTick, entity) in SystemAPI.Query<DestroyAfterTick>().WithEntityAccess())
            {
                commandBuffer.DestroyEntity(entity);
            }
        }
    }
}