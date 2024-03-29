﻿using System;
using _OnlyOneGame.Scripts.Components;
using _OnlyOneGame.Scripts.Components.Data;
using Components;
using DefaultNamespace;
using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.Systems;
using HomeKeeper.Components;
using Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using ValueVariant;

namespace _OnlyOneGame.Scripts.Systems
{
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(CharacterMovementSystem))]
    [UpdateAfter(typeof(HealthSystem))]
    public partial struct OnPlayerCharacterSystem : ISystem
    {
        private NativeList<(float3, Entity)> m_OverlapSphereResultBuffer;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<OnPrefabs>();
            state.RequireForUpdate<BuildPhysicsWorldData>();
            state.RequireForUpdate<OnPlayerCharacter>();
            state.RequireForUpdate<NetworkTime>();
            m_OverlapSphereResultBuffer = new NativeList<(float3, Entity)>(Allocator.Persistent);
        }
        
        
        
        public void OnUpdate(ref SystemState state)
        {
            state.CompleteDependency();
            
            var groundItemLookup = SystemAPI.GetComponentLookup<GroundItem>();
            var healthLookup = SystemAPI.GetComponentLookup<Health>();
            var factionLookup = SystemAPI.GetComponentLookup<Faction>();
            var ghostDestroyedLookup = SystemAPI.GetComponentLookup<DestroyableGhost>();
            var ghostOwnerLookup = SystemAPI.GetComponentLookup<GhostOwner>();
            var physicsVelocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>();
            
            var buildPhysicsWorld = SystemAPI.GetSingleton<BuildPhysicsWorldData>();
            
            var prefabs = SystemAPI.GetSingleton<OnPrefabs>();
            
            var interactionRadius = 1f;

            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            
            // todo: just use the "networkTime.ServerTick"
            var deltaTime = 0.02f;

            var tick = networkTime.ServerTick;
            var tickInt = tick.TickIndexForValidTick;
            var time = (float)tickInt * deltaTime;
            
            //if(networkTime.IsPartialTick) return;
            


            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            // update logic
            foreach (var (playerCharacterRw, localTransform, characterMovementRw, faction, health, entity) 
                     in SystemAPI.Query<
                             RefRW<OnPlayerCharacter>, 
                             LocalTransform,
                             RefRW<CharacterMovement>,
                             Faction,
                             Health
                         >()
                         .WithEntityAccess().WithAny<GhostOwner>().WithAll<Simulate>())
            {
                var playerCharacter = playerCharacterRw.ValueRO;
                var characterMovement = characterMovementRw.ValueRO;
                ghostOwnerLookup.TryGetComponent(entity, out var ghostOwner);
                
                
                if (health.IsDead)
                {
                    characterMovement.MovementInput = float2.zero;
                    continue;
                }
                
                characterMovement.MovementInput = playerCharacter.Input.Movement;
                
                if(math.lengthsq(playerCharacter.LookDirection) < 0.5f)
                {
                    playerCharacter.LookDirection = new float2(1, 0); // init
                }

                //Debug.Log("look direction input: " + playerCharacter.LookInput);
                playerCharacter.LookDirection = math.normalizesafe(math.lerp(playerCharacter.LookDirection,  playerCharacter.Input.Look, 0.1f));


                var playerCharacterEvents = playerCharacter.Events;
                var inventoryStack = playerCharacter.InventoryStack.Get();
                
                playerCharacterEvents.Clear();
                

                var playerPosition = localTransform.Position;
                
                var silenced = playerCharacter.CommandsBlockedDuration > 0;
                
                // handle action
                bool itemNearby = false;
                var characterPosition = localTransform.Position;
                        
                m_OverlapSphereResultBuffer.Clear();
                buildPhysicsWorld.GetAllOverlapSphereNoAlloc(characterPosition, 1.5f, ref m_OverlapSphereResultBuffer);
                foreach (var (qPos, qEntity) in m_OverlapSphereResultBuffer)
                {
                    if (SystemAPI.HasComponent<GroundItem>(qEntity))
                    {
                        itemNearby = true;
                    }
                }
                
                // figure out action:
                //if()
                

                // process action
                if (!silenced)
                {
                    if (playerCharacter.Input.PickupButtonTap)
                    {
                        if (itemNearby)
                        {
                            ProcessPickupItemCommand(
                                ref buildPhysicsWorld,
                                playerPosition,
                                interactionRadius,
                                ref groundItemLookup,
                                ref inventoryStack,
                                playerCharacter.InventoryCapacity,
                                ref playerCharacter.CommandsBlockedDuration,
                                ref playerCharacterEvents,
                                ref ghostDestroyedLookup,
                                tick
                            );
                        }
                    }
                    else if (playerCharacter.Input.DropButtonTap)
                    {

                        // drop Item
                        if (inventoryStack.Length != 0)
                        {
                            var item = inventoryStack[^1];
                            inventoryStack.RemoveAt(inventoryStack.Length - 1);

                            var dropPosition = playerPosition + Utility.Up * 0.5f + localTransform.Forward();
                            playerCharacterEvents.Add(new PlayerEvent(new EventDroppedItem(item)));

                            ThrowItem(dropPosition, Utility.Up * 2f, item, in prefabs, ref ecb, false, tick,
                                ghostOwner);
                        }
                    }
                    else if (playerCharacter.Input.ActionButton0Tap)
                    {
                        playerCharacter.OnGoingActionOpt.Set(new OnGoingAction(time,
                            0.35f,
                            new ActionMeleeAttacking(playerCharacter.LookDirection.X0Y())));
                        playerCharacter.CommandsBlockedDuration += (int)(2f / deltaTime);
                        playerCharacterEvents.Add(new PlayerEvent(new EventMeleeAttackStarted()));
                    }
                    else if (playerCharacter.Input.DropButtonReleasedFromHold && inventoryStack.Length != 0)
                    {
                        var item = inventoryStack[^1];
                        var throwVelocity = playerCharacter.Input.Look.X0Y() * 5 + Utility.Up * 5;
                        var throwDirection = throwVelocity / (math.length(throwVelocity) + 0.001f);
                        var throwPosition = playerPosition + throwDirection * 0.5f + Utility.Up;

                        inventoryStack.RemoveAt(inventoryStack.Length - 1);
                        playerCharacterEvents.Add(new PlayerEvent(new EventThrownItem(item, throwVelocity)));

                        ThrowItem(throwPosition, throwVelocity, item, in prefabs, ref ecb, true, tick,
                            ghostOwner);
                            
                        playerCharacter.CommandsBlockedDuration += (int)(0.5f / deltaTime);                    }
                }


                if (playerCharacter.OnGoingActionOpt.TryGet(out var onGoingAction))
                {
                    if (onGoingAction.Data.TryGet(out ActionDismantling dismantling))
                    {
                        
                    }
                    else if (onGoingAction.Data.TryGet(out ActionMeleeAttacking meleeAttacking))
                    {
                        if (time >= onGoingAction.StartTime + onGoingAction.Duration)
                        {
                            playerCharacter.OnGoingActionOpt = Option<OnGoingAction>.None();
                            
                            
                            Utility.DamageNearby(
                                playerPosition + meleeAttacking.Direction * 0.5f, 
                                1.5f, 
                                0.5f,
                                Option<Faction>.Some(faction), 
                                Option<Entity>.Some(entity),
                                ref buildPhysicsWorld,
                                ref healthLookup,
                                ref factionLookup,
                                ref physicsVelocityLookup
                            );
                        }
                    }
                }
                
                playerCharacter.CommandsBlockedDuration -= 1;
                if (playerCharacter.CommandsBlockedDuration < 0) playerCharacter.CommandsBlockedDuration = 0;
                playerCharacter.MovementBlockedDuration -= 1;
                if (playerCharacter.MovementBlockedDuration < 0) playerCharacter.MovementBlockedDuration = 0;

                // write back
                playerCharacter.Events = playerCharacterEvents;
                playerCharacter.InventoryStack.Set(inventoryStack);
                

                playerCharacterRw.ValueRW = playerCharacter;
                characterMovementRw.ValueRW = characterMovement;
            }
            
            // update view
            foreach (var (characterViewRw, onPlayerCharacterRo, localTransform, characterMovementRo, healthRo, physicsVelocityRo) in
                     SystemAPI.Query<
                         RefRW<CharacterView>,
                         RefRO<OnPlayerCharacter>,
                         LocalTransform,
                         RefRO<CharacterMovement>,
                         RefRO<Health>,
                         RefRO<PhysicsVelocity>
                     >().WithAll<Simulate>()
                    )
            {
                var characterView = characterViewRw.ValueRO;
                
                //characterView.LastAttacked = networkTime.ServerTick;
                //characterView.LastItemThrown = networkTime.ServerTick;

                var events = onPlayerCharacterRo.ValueRO.Events;
                foreach (var playerEvent in events)
                {
                    playerEvent.Switch(
                        meleeAttackStarted => characterView.LastAttacked = tick,
                        itemPickedUp => { },
                        crafted => { },
                        unbuilt => { },
                        resourceGathered => { },
                        itemStackChanged => { },
                        droppedItem => characterView.LastItemThrown = tick,
                        thrownItem => characterView.LastItemThrown = tick
                    );
                }
                
                characterView.MovementVelocity = physicsVelocityRo.ValueRO.Linear.xz;
                characterView.LookDirection = onPlayerCharacterRo.ValueRO.LookDirection.X0Y();

                characterView.Dead = healthRo.ValueRO.IsDead;
                if (healthRo.ValueRO.DiedNow)
                {
                    characterView.RagdollForce = 400;
                    characterView.RagdollForcePoint = healthRo.ValueRO.BiggestDamageNormal + localTransform.Position;
                    Debug.Log("baran- ragdoll force: " + characterView.RagdollForcePoint);
                }

                characterView.IsGrounded = characterMovementRo.ValueRO.IsGrounded;

                characterViewRw.ValueRW = characterView;
            }

            if (!ecb.IsEmpty && networkTime.IsFirstTimeFullyPredictingTick)
            {
                ecb.Playback(state.EntityManager);
            }
            ecb.Dispose();
        }

        
        private static Entity ThrowItem(float3 position, float3 velocity, Item item, in OnPrefabs prefabs, ref EntityCommandBuffer ecb, bool activated, NetworkTick currentTick, GhostOwner ghostOwner)
        {
            var instance = CreateAndThrow(position, velocity, prefabs.GroundItemPrefab.Entity, ref ecb, ghostOwner);
            ecb.SetComponent(instance, new GroundItem(item));
            //var turnInterpolated = currentTick;
            //turnInterpolated.Add(50);
            //ecb.SetComponent(instance, new TurnInterpolatedAfterDelay(currentTick,turnInterpolated));
            
            
            if (activated)
            {
                var deploymentDirection = velocity;
                deploymentDirection.y = 0;
                deploymentDirection = math.normalizesafe(deploymentDirection, Utility.Forward);
                //ecb.AddComponent(instance, new ActivatedItem(deploymentDirection, 10, currentTick));
            }
            
            return instance;
        }
        
        private static Entity CreateAndThrow(float3 position, float3 velocity, Entity prefab, ref EntityCommandBuffer ecb, GhostOwner ghostOwner)
        {
            var instance = ecb.Instantiate(prefab);
            
            ecb.SetLocalPositionRotation(instance, position, quaternion.identity);
            ecb.SetVelocity(instance, velocity);
            ecb.AddComponent(instance, ghostOwner);
            return instance;
        }

        private static void ProcessPickupItemCommand(ref BuildPhysicsWorldData buildPhysicsWorld, float3 playerPosition,
            float interactionRadius, ref ComponentLookup<GroundItem> groundItemLookup, ref FixedList128Bytes<Item> inventoryStack, int inventoryCapacity,
            ref int commandBlockedDuration,
            ref FixedList128Bytes<PlayerEvent> playerEvents,
            ref ComponentLookup<DestroyableGhost> ghostDestroyedLookup, NetworkTick tick)
        {
            if (buildPhysicsWorld.TryGetFirstOverlapSphere(
                    playerPosition,
                    interactionRadius,
                    ref groundItemLookup,
                    out var groundItemEntity,
                    out var groundItem
                ))
            {
                var item = groundItem.Item;
                if (inventoryCapacity > inventoryStack.Length)
                {
                    
                    inventoryStack.Add(item);
                    
                    if(ghostDestroyedLookup.TryGetRw(groundItemEntity, out var ghostDestroyedRw))
                    {
                        var ghostDestroyed = ghostDestroyedRw.ValueRO;
                        ghostDestroyed.SetDestroyed(tick);
                        ghostDestroyedRw.ValueRW = ghostDestroyed;
                    }
                    else
                    {
                        Debug.LogError("GhostDestroyed not found on GroundItem entity");
                    }
                    
                    
                    playerEvents.Add(new PlayerEvent(new EventItemPickedUp(item)));
                    
                    commandBlockedDuration += (int)(0.5f / 0.02f);
                }
            }
        }

        private static void ProcessDismantleCommand(ref OnPlayerCharacter playerCharacter, ComponentLookup<LocalTransform> localTransformLookup,
            float3 playerPosition, float interactionRadius, ref BuildPhysicsWorldData buildPhysicsWorld, float time, ref ComponentLookup<DeployedItem> deployedItemLookup)
        {
            if (playerCharacter.OnGoingActionOpt.TryGet(out var onGoingAction) &&
                onGoingAction.Data.TryGet(out ActionDismantling dismantling))
            {
                // still unbuilding
                if (localTransformLookup.TryGetComponent(dismantling.Target, out var targetTransform) &&
                    math.distance(playerPosition, targetTransform.Position) < interactionRadius)
                {
                    return;
                }

                // try start dismantling something else
                if (
                    buildPhysicsWorld.TryGetFirstOverlapSphere(
                        playerPosition,
                        interactionRadius,
                        ref deployedItemLookup,
                        out var deployedItemEntity,
                        out var deployedItem
                    ))
                {
                    playerCharacter.OnGoingActionOpt = Option<OnGoingAction>.Some(new OnGoingAction(
                        time, 2, new ActionDismantling(deployedItemEntity)
                    ));
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }

    public static class OnCharacterSystemUtils
    {
        public static ActionCommand GetActionCommand(
            OnPlayerCharacter character, 
            float3 characterPosition,
            ref ComponentLookup<GroundItem> groundItemLookup
        )
        {
            return default;
        }
    }
}

/*
                
                // process action
                if (!silenced && playerCharacter.ActionCommandOpt.Get().TryGet(out var actionCommand))
                {
                    actionCommand.Switch(
                        dismantle => ProcessDismantleCommand(
                            ref playerCharacter,
                            localTransformLookup,
                            playerPosition,
                            interactionRadius,
                            ref buildPhysicsWorld,
                            time,
                            ref deployedItemLookup
                        ),
                        pickupItem => ProcessPickupItemCommand(
                            ref buildPhysicsWorld,
                            playerPosition,
                            interactionRadius,
                            ref groundItemLookup,
                            ref inventoryStack,
                            playerCharacter.InventoryCapacity,
                            ref playerCharacter.CommandsBlockedDuration,
                            ref playerCharacterEvents,
                            ref ghostDestroyedLookup
                        ),
                        craftItem => { },
                        mineResource => { },
                        cycleStack => { },
                        meleeAttack =>
                        {
                            playerCharacter.OnGoingActionOpt.Set(new OnGoingAction(time,
                                2, new OnGoingActionData(new ActionMeleeAttacking(meleeAttack.Direction))));
                            playerCharacter.CommandsBlockedDuration += (int)(2f / deltaTime);
                            playerCharacterEvents.Add(new PlayerEvent(new EventMeleeAttackStarted()));
                        },
                        throwItem =>
                        {
                            if(inventoryStack.Length == 0)
                                return;
                            
                            var item = inventoryStack[^1];
                            var throwVelocity = throwItem.ThrowVelocity;
                            var throwDirection = throwVelocity / (math.length(throwVelocity) + 0.001f);
                            var throwPosition = playerPosition + throwDirection * 0.5f + Utility.Up;
                            
                            inventoryStack.RemoveAt(inventoryStack.Length - 1);
                            playerCharacterEvents.Add(new PlayerEvent(new EventThrownItem(item, throwItem.ThrowVelocity)));

                            ThrowItem(throwPosition, throwVelocity, item, in prefabs, ref ecb, true, tick, ghostOwner);
                        },
                        dropItem =>
                        {
                            if(inventoryStack.Length == 0)
                                return;
                            
                            var item = inventoryStack[^1];
                            inventoryStack.RemoveAt(inventoryStack.Length - 1);
                            
                            var dropPosition = playerPosition + Utility.Up * 0.5f + localTransform.Forward();
                            playerCharacterEvents.Add(new PlayerEvent(new EventDroppedItem(item)));
                            
                            ThrowItem(dropPosition, Utility.Up * 2f, item, in prefabs, ref ecb, false, tick, ghostOwner);
                        } 
                    );
                }
                
                */