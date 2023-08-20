﻿using System;
using _OnlyOneGame.Scripts.Components;
using Components;
using DefaultNamespace;
using DefenderGame.Scripts.Components;
using HomeKeeper.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;
using ValueVariant;

namespace _OnlyOneGame.Scripts.Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct OnPlayerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<OnPrefabs>();
            state.RequireForUpdate<BuildPhysicsWorldData>();
            state.RequireForUpdate<OnPlayerCharacter>();
        }
        
        
        
        public void OnUpdate(ref SystemState state)
        {
            var buildPhysicsWorld = SystemAPI.GetSingleton<BuildPhysicsWorldData>();
            var prefabs = SystemAPI.GetSingleton<OnPrefabs>();
            
            var deployedItemLookup = SystemAPI.GetComponentLookup<DeployedItem>();
            var groundItemLookup = SystemAPI.GetComponentLookup<GroundItem>();
            var localTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
            var healthLookup = SystemAPI.GetComponentLookup<Health>();
            var factionLookup = SystemAPI.GetComponentLookup<Faction>();
            
            var interactionRadius = 1f;

            var time = (float)SystemAPI.Time.ElapsedTime;
            var deltaTime = SystemAPI.Time.DeltaTime;
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (playerCharacterRw, localTransform, characterMovementRw, faction, entity) 
                     in SystemAPI.Query<
                             RefRW<OnPlayerCharacter>, 
                             LocalTransform,
                             RefRW<CharacterMovement>,
                             Faction
                         >()
                         .WithEntityAccess())
            {
                var playerCharacter = playerCharacterRw.ValueRO;
                var characterMovement = characterMovementRw.ValueRO;
                
                
                playerCharacter.Events.Edit((ref FixedList128Bytes<PlayerEvent> value) => value.Clear());
                
                var playerPosition = localTransform.Position;
                
                var silenced = playerCharacter.CommandsBlockedDuration > 0;
                
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
                            buildPhysicsWorld,
                            playerPosition,
                            interactionRadius,
                            ref groundItemLookup,
                            ref playerCharacter,
                            ecb
                        ),
                        craftItem => { },
                        mineResource => { },
                        cycleStack => { },
                        meleeAttack =>
                        {
                            playerCharacter.OnGoingActionOpt.Set(new OnGoingAction(time,
                                2, new OnGoingActionData(new ActionMeleeAttacking(meleeAttack.Direction))));
                            playerCharacter.CommandsBlockedDuration += 2;
                        },
                        throwItem =>
                        {
                            if(playerCharacter.InventoryStack.Get().Length == 0)
                                return;
                            
                            var item = playerCharacter.InventoryStack.Get()[^1];
                            var throwVelocity = throwItem.ThrowVelocity;
                            var throwDirection = throwVelocity / (math.length(throwVelocity) + 0.001f);
                            var throwPosition = playerPosition + throwDirection * 0.5f + Utility.Up;

                            //playerCharacter.InventoryStack.Value.RemoveAt(playerCharacter.InventoryStack.Value.Length - 1);
                            playerCharacter.InventoryStack.Edit((ref FixedList128Bytes<Item> value) => value.RemoveAt(value.Length - 1));
                            //playerCharacter.Events.Value.Add(new PlayerEvent(new EventThrownItem(item, throwItem.ThrowVelocity)));
                            playerCharacter.Events.Edit((ref FixedList128Bytes<PlayerEvent> value) => value.Add(new PlayerEvent(new EventThrownItem(item, throwItem.ThrowVelocity))));

                            if(item.TryGetValue(out DeployableItemType deployableItemType))
                            {
                                ThrowActivatedDeployable(throwPosition, throwVelocity, deployableItemType, in prefabs,
                                    ref ecb, time);
                            }
                            else
                            {
                                ThrowItem(throwPosition, throwVelocity, item, in prefabs, ref ecb, time);
                            }
                        },
                        dropItem =>
                        {
                            if(playerCharacter.InventoryStack.Get().Length == 0)
                                return;
                            
                            //var item = playerCharacter.InventoryStack.Value[^1];
                            var item = playerCharacter.InventoryStack.Get()[^1];
                            //playerCharacter.InventoryStack.Value.RemoveAt(playerCharacter.InventoryStack.Value.Length - 1);
                            playerCharacter.InventoryStack.Edit((ref FixedList128Bytes<Item> value) => value.RemoveAt(value.Length - 1));
                            
                            var dropPosition = playerPosition + Utility.Up * 0.5f + localTransform.Forward();
                            //playerCharacter.Events.Value.Add(new PlayerEvent(new EventDroppedItem(item)));
                            playerCharacter.Events.Edit((ref FixedList128Bytes<PlayerEvent> value) => value.Add(new PlayerEvent(new EventDroppedItem(item))));
                            
                            ThrowItem(dropPosition, Utility.Up * 2f, item, in prefabs, ref ecb, time);
                        } 
                    );
                }

                if (playerCharacter.OnGoingActionOpt.Get().TryGet(out var onGoingAction))
                {
                    onGoingAction.Data.Switch(
                        meleeAttacking =>
                        {
                            if (time >= onGoingAction.StartTime + onGoingAction.Duration)
                            {
                                playerCharacter.OnGoingActionOpt = Option<OnGoingAction>.None();
                                Utility.DamageNearby(
                                    playerPosition + meleeAttacking.Direction * 0.5f, 
                                    1.5f, 
                                    0.3f,
                                    Option<Faction>.Some(faction), 
                                    Option<Entity>.Some(entity),
                                    ref buildPhysicsWorld,
                                    ref healthLookup,
                                    ref factionLookup
                                );
                            }
                        },
                        dismantling =>
                        {
                            
                        }
                    );
                }
                
                playerCharacter.CommandsBlockedDuration -= deltaTime;
                if (playerCharacter.CommandsBlockedDuration < 0) playerCharacter.CommandsBlockedDuration = 0;
                playerCharacter.MovementBlockedDuration -= deltaTime;
                if (playerCharacter.MovementBlockedDuration < 0) playerCharacter.MovementBlockedDuration = 0;

                playerCharacterRw.ValueRW = playerCharacter;
                characterMovementRw.ValueRW = characterMovement;
            }
        }

        private static Entity ThrowActivatedDeployable(float3 position, float3 velocity, DeployableItemType deployableItemType, in OnPrefabs prefabs, ref EntityCommandBuffer ecb, float time)
        {
            var instance = CreateAndThrow(position, velocity, prefabs.DeployingItemPrefab.Entity, ref ecb);
            var deployDuration = deployableItemType switch
            {
                DeployableItemType.Wall => 10,
                DeployableItemType.Turret => 10,
                DeployableItemType.AutoRepairModule => 10,
                DeployableItemType.BubbleShieldModule => 10,
                DeployableItemType.MiningModule => 10,
                DeployableItemType.SpawnPoint => 10,
                DeployableItemType.Landmine => 2,
                DeployableItemType.BarbedWire => 5,
                _ => 0
            };
            if(deployDuration == 0) Debug.LogError("Deployable item type not supported: " + deployableItemType);

            ecb.SetComponent(instance,
                new DeployingItem(
                    deployableItemType,
                    math.normalizesafe(velocity, Utility.Forward), 3, time
                )
            );
            return instance;
        }
        
        private static Entity ThrowItem(float3 position, float3 velocity, Item item, in OnPrefabs prefabs, ref EntityCommandBuffer ecb, float time)
        {
            var instance = CreateAndThrow(position, velocity, prefabs.GroundItemPrefab.Entity, ref ecb);
            ecb.SetComponent(instance, new GroundItem(item, time));
            return instance;
        }
        
        private static Entity CreateAndThrow(float3 position, float3 velocity, Entity prefab, ref EntityCommandBuffer ecb)
        {
            var instance = ecb.Instantiate(prefab);
            ecb.SetLocalPositionRotation(instance, position, quaternion.identity);
            ecb.SetVelocity(instance, velocity);
            return instance;
        }

        private static void ProcessPickupItemCommand(BuildPhysicsWorldData buildPhysicsWorld, float3 playerPosition,
            float interactionRadius, ref ComponentLookup<GroundItem> groundItemLookup, ref OnPlayerCharacter playerCharacter,
            EntityCommandBuffer ecb)
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
                if (playerCharacter.InventoryCapacity > playerCharacter.InventoryStack.Get().Length)
                {
                    //playerCharacter.InventoryStack.Value.Add(item);
                    playerCharacter.InventoryStack.Edit((ref FixedList128Bytes<Item> value) => value.Add(item));
                    ecb.DestroyEntity(groundItemEntity);

                    // todo: not too sure if this will modify the player character
                    //playerCharacter.Events.Value.Add(new PlayerEvent(new EventItemPickedUp(item)));
                    playerCharacter.Events.Edit((ref FixedList128Bytes<PlayerEvent> value) => value.Add(new PlayerEvent(new EventItemPickedUp(item))));
                    
                    playerCharacter.CommandsBlockedDuration += 0.5f;
                }
            }
        }

        private static void ProcessDismantleCommand(ref OnPlayerCharacter playerCharacter, ComponentLookup<LocalTransform> localTransformLookup,
            float3 playerPosition, float interactionRadius, ref BuildPhysicsWorldData buildPhysicsWorld, float time, ref ComponentLookup<DeployedItem> deployedItemLookup)
        {
            if (playerCharacter.OnGoingActionOpt.Get().TryGet(out var onGoingAction) &&
                onGoingAction.Data.TryGetValue(out ActionDismantling dismantling))
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
                    //playerCharacter.OnGoingActionOpt.Value = new OnGoingAction(
                    //    time, 2, new OnGoingActionData(new ActionDismantling(deployedItemEntity))
                    //);
                    playerCharacter.OnGoingActionOpt = Option<OnGoingAction>.Some(new OnGoingAction(
                        time, 2, new OnGoingActionData(new ActionDismantling(deployedItemEntity))
                    ));
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}