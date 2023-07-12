using Unity.Entities;

namespace HomeKeeper.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SpacialPartitioningSystem))]
    public partial struct ParticleCollisionSolvingSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var spacialPartitioning = state.WorldUnmanaged.GetExistingUnmanagedSystem<SpacialPartitioningSystem>();
        }
    }
}