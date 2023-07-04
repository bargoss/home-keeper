using HomeKeeper.Components;
using Unity.Collections;
using Unity.Entities;

namespace HomeKeeper.Systems
{
    // run just after SimulationSystemGroup
    
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct EventCleanupSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entity) in SystemAPI.Query<EcsEvent>().WithEntityAccess())
            {
                commandBuffer.DestroyEntity(entity);
            }
        }
    }
}