using _OnlyOneGame.Scripts.Components;
using DefenderGame.Scripts.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial class LocalPlayerCharacterMovementControlSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<CubeInput>();
        }
        
        protected override void OnUpdate()
        {
            foreach (var (localPlayerCharacterMovementControl, characterMovementRw, entity) 
                     in SystemAPI.Query<CubeInput, RefRW<CharacterMovement>>().WithAll<Simulate>().WithEntityAccess())
            {
                var characterMovement = characterMovementRw.ValueRW;
                characterMovement.MovementInput = localPlayerCharacterMovementControl.Value;
                characterMovementRw.ValueRW = characterMovement;
            }
        }
        
    }
}