using _OnlyOneGame.Scripts.Components;
using _OnlyOneGame.Scripts.Components.Tank;
using _OnlyOneGame.Scripts.Components.Tank.Actions;
using DefenderGame.Scripts.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Systems
{
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(CharacterMovement))]
    public partial struct OnTankSkillSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<OnPrefabs>();
            state.RequireForUpdate<BuildPhysicsWorldData>();
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<ClientServerTickRate>();
            state.RequireForUpdate<OnTank>();
            state.RequireForUpdate<OnTankSkills>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsVelocityLookup = new ComponentLookup<PhysicsVelocity>();
            var clientServerTickRate = SystemAPI.GetSingleton<ClientServerTickRate>();
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            var physics = SystemAPI.GetSingleton<BuildPhysicsWorldData>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var prefabs = SystemAPI.GetSingleton<OnPrefabs>();
            
            foreach (var (onTankSkillsRw, localTransform, physicsVelocity, characterMovementRw, entity) 
                     in SystemAPI.Query<RefRW<OnTankSkills>, LocalTransform, PhysicsVelocity, RefRW<CharacterMovement>>()
                         .WithAll<Simulate>().WithEntityAccess())
            {
                if (onTankSkillsRw.ValueRO.OnGoingAction.TryGet(out var onGoingTankAction))
                {
                    var actionHandlerContext =  new ActionHandlerContext(
                        onGoingTankAction.ActionData,
                        characterMovementRw.ValueRO,
                        ecb,
                        prefabs,
                        physics,
                        localTransform,
                        networkTime,
                        clientServerTickRate,
                        physicsVelocityLookup
                    );
                    switch (onGoingTankAction.ActionCard)
                    {
                        case TankActionCard.Move:
                            ActionHandlers.Update_Move(ref actionHandlerContext);
                            break;
                        default:
                            Debug.LogError("action not implemented: " + onGoingTankAction.ActionCard);
                            break;
                    }
                    
                    actionHandlerContext.ApplyBack(ref onGoingTankAction.ActionData, ref characterMovementRw.ValueRW, ref ecb);
                    
                    if (actionHandlerContext.IsFinished)
                    {
                        onTankSkillsRw.ValueRW.OnGoingAction = Option<OnGoingTankAction>.None;
                    }
                }
            }

            if (!ecb.IsEmpty)
            {
                ecb.Playback(state.EntityManager);
            }
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}