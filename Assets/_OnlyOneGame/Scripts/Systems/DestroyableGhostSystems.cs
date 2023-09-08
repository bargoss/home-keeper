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

    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct TurnInterpolatedAfterDelaySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GhostPredictionSwitchingQueues>();
            state.RequireForUpdate<NetworkTime>();
        }

        public void OnDestroy(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {
            var time = SystemAPI.GetSingleton<NetworkTime>();
            if (!time.IsFirstTimeFullyPredictingTick) return;

            var tick = time.ServerTick;
            var ghostPredictionSwitchingQueuesRw = SystemAPI.GetSingletonRW<GhostPredictionSwitchingQueues>();
            
            foreach (var (turnInterpolatedAfterDelay, predictedGhost, entity) in SystemAPI.Query<TurnInterpolatedAfterDelay, PredictedGhost>().WithEntityAccess())
            {
                if (turnInterpolatedAfterDelay.PredictionStart.IsValid && turnInterpolatedAfterDelay.PredictionEnd.IsValid)
                {
                    var ticksSincePredictionStart = tick.TicksSince(turnInterpolatedAfterDelay.PredictionStart);
                    var ticksSincePredictionEnd = tick.TicksSince(turnInterpolatedAfterDelay.PredictionEnd);
                    
                    if(ticksSincePredictionStart < 0 || ticksSincePredictionEnd >= 0)
                    {
                        Debug.Log("turned interpolated");
                        ghostPredictionSwitchingQueuesRw.ValueRW.ConvertToInterpolatedQueue.Enqueue(new ConvertPredictionEntry()
                        {
                            TargetEntity = entity,
                            TransitionDurationSeconds = 0f,
                        });
                    }
                }
            }
            foreach (var (turnInterpolatedAfterDelay, entity) in SystemAPI.Query<TurnInterpolatedAfterDelay>().WithNone<PredictedGhost>().WithEntityAccess())
            {
                if (turnInterpolatedAfterDelay.PredictionStart.IsValid && turnInterpolatedAfterDelay.PredictionEnd.IsValid)
                {
                    var ticksSincePredictionStart = tick.TicksSince(turnInterpolatedAfterDelay.PredictionStart);
                    var ticksSincePredictionEnd = tick.TicksSince(turnInterpolatedAfterDelay.PredictionEnd);
                    
                    if(ticksSincePredictionStart >= 0 && ticksSincePredictionEnd < 0)
                    {
                        Debug.Log("turned predicted");
                        ghostPredictionSwitchingQueuesRw.ValueRW.ConvertToPredictedQueue.Enqueue(new ConvertPredictionEntry()
                        {
                            TargetEntity = entity,
                            TransitionDurationSeconds = 0.0f,
                        });
                    }
                }
            }
        }
    }
    
    public struct TurnInterpolatedAfterDelay : IComponentData
    {
        public NetworkTick PredictionStart;
        public NetworkTick PredictionEnd;
        
        public TurnInterpolatedAfterDelay(NetworkTick predictionStart, NetworkTick predictionEnd)
        {
            PredictionStart = predictionStart;
            PredictionEnd = predictionEnd;
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