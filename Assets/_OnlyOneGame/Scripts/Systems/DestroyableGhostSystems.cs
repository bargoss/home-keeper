using _OnlyOneGame.Scripts.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Systems
{
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct DestroyableGhostServerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var tick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var destroyableGhostAspect in SystemAPI.Query<DestroyableGhostAspect>())
            {
                if (destroyableGhostAspect.GetGhostDestroyed(tick))
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
    
    
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct DestroyableGhostClientSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var tick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var disabledLookup = SystemAPI.GetComponentLookup<Disabled>();
            
            foreach (var (destroyableGhost, entity) in SystemAPI.Query<DestroyableGhost>().WithEntityAccess().WithOptions(EntityQueryOptions.IncludeDisabledEntities))
            {
                var disabled = disabledLookup.HasComponent(entity);
                var ghostDestroyed = destroyableGhost.GetDestroyed(tick);
                if (disabled)
                {
                    Debug.Log("baran - a disabled entity : " + entity);
                }
                if (!disabled && ghostDestroyed)
                {
                    ecb.SetEnabled(entity, false);
                    Debug.Log("baran - disabled : " + entity); 
                }
                else if(disabled && !ghostDestroyed)
                {
                    ecb.SetEnabled(entity, true);
                    Debug.Log("baran - enabled : " + entity);
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

        public bool GetGhostDestroyed(NetworkTick tick)
        {
            return m_DestroyableGhost.ValueRO.GetDestroyed(tick);
        }

    }
}