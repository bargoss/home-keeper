using _OnlyOneGame.Scripts.Components;
using DefaultNamespace;
using DefenderGame.Scripts.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateBefore(typeof(OnPlayerSystem))]
    public partial class LocalPlayerCharacterMovementControlSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<OnPlayerInput>();
            RequireForUpdate<SyncedIdToEntityMap>();
        }

        protected override void OnUpdate()
        {
            var syncedIdToEntityMap = SystemAPI.ManagedAPI.GetSingleton<SyncedIdToEntityMap>();

            foreach (var (playerInput, playerRo, entity)
                     in SystemAPI.Query<OnPlayerInput, RefRO<OnPlayer>>().WithAll<Simulate>().WithEntityAccess())
            {
                if (syncedIdToEntityMap.TryGet(playerRo.ValueRO.ControlledCharacterSyncedId, out var controlledCharacterEntity))
                {
                    if (SystemAPI.GetComponentLookup<OnPlayerCharacter>().TryGetRw(controlledCharacterEntity, out var controlledCharacterRw))
                    {
                        //input is not correct here, its just the default input
                        controlledCharacterRw.ValueRW.SetMovementInput(playerInput.MovementInput);
                        controlledCharacterRw.ValueRW.SetLookInput(playerInput.LookInput);
                        controlledCharacterRw.ValueRW.SetActionCommandOpt(playerInput.ActionCommandOpt);
                    }
                }
            }
        }
    }
}