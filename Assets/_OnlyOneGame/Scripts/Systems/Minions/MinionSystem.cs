using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.Systems;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Systems
{
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(CharacterMovementSystem))]
    [UpdateBefore(typeof(CharacterViewSystem))]
    public partial struct MinionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}