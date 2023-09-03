using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.VisualScripting;

namespace _OnlyOneGame.Scripts.Components
{
    public struct LoseOwnerWithDelay : IComponentData
    {
        public int Ticks;
        public bool Converted;
    }

    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    //[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct SetInterpolatedWhenStationarySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            var ghostPredictionSwitchingQueues = SystemAPI.GetSingletonRW<GhostPredictionSwitchingQueues>().ValueRW;
            
            if(!networkTime.IsFirstTimeFullyPredictingTick) return;

            
            foreach (var (setInterpolatedAfterTicksRw, entity) in SystemAPI.Query<RefRW<LoseOwnerWithDelay>>().WithAll<Simulate>().WithEntityAccess())
            {
                var setInterpolatedAfterTicks = setInterpolatedAfterTicksRw.ValueRO;
                
                setInterpolatedAfterTicks.Ticks -= 1;

                if (setInterpolatedAfterTicks.Ticks <= 0 && !setInterpolatedAfterTicks.Converted)
                {
                    //ghostOwnerRw.ValueRW.NetworkId = 0;
                    setInterpolatedAfterTicks.Converted = true;
                    ghostPredictionSwitchingQueues.ConvertToInterpolatedQueue.Enqueue(new ConvertPredictionEntry
                        { TargetEntity = entity, TransitionDurationSeconds = 2 });
                    //ghostPredictionSwitchingQueues.ConvertToInterpolatedQueue.Enqueue(new ConvertPredictionEntry()
                    //    { TargetEntity = entity, TransitionDurationSeconds = 2 });
                }
                
                setInterpolatedAfterTicksRw.ValueRW = setInterpolatedAfterTicks;
            }
            
        }
    }
}