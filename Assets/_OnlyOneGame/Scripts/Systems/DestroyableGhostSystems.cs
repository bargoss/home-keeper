using _OnlyOneGame.Scripts.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct DestroyableGhostServerSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var destroyableGhostAspect in SystemAPI.Query<DestroyableGhostAspect>())
            {
                if (destroyableGhostAspect.GhostDestroyed)
                {
                    ecb.DestroyEntity(destroyableGhostAspect.Self);                    
                }                
            }
            
            if (!ecb.IsEmpty)
            {
                ecb.Playback(state.EntityManager);
            }
            ecb.Dispose();
        }
    }
    
    
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct DestroyableGhostClientSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var destroyableGhostAspect in SystemAPI.Query<DestroyableGhostAspect>())
            {
                if (destroyableGhostAspect is { GhostDestroyed: true, IsEnabled: true })
                {
                    ecb.SetEnabled(destroyableGhostAspect.Self, false);
                }
                else if(destroyableGhostAspect is { GhostDestroyed: false, IsEnabled: false })
                {
                    ecb.SetEnabled(destroyableGhostAspect.Self, true);
                }
            }
            
            if (!ecb.IsEmpty)
            {
                ecb.Playback(state.EntityManager);
            }
            ecb.Dispose();
        }
    }
    
    
    public readonly partial struct DestroyableGhostAspect : IAspect
    {
        public readonly Entity Self;
        private readonly RefRW<DestroyableGhost> m_DestroyableGhost;
        [Optional] private readonly RefRO<Disabled> m_Disabled;
        
        public bool IsEnabled => !m_Disabled.IsValid;

        public bool GhostDestroyed
        {
            get => m_DestroyableGhost.ValueRO.Destroyed;
            set => m_DestroyableGhost.ValueRW = new DestroyableGhost {Destroyed = value};
        }

    }
}