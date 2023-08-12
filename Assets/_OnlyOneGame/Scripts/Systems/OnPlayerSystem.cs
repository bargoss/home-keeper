using System;
using _OnlyOneGame.Scripts.Components;
using DefaultNamespace;
using HomeKeeper.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using ValueVariant;

namespace _OnlyOneGame.Scripts.Systems
{
    public partial struct OnPlayerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildPhysicsWorldData>();
            state.RequireForUpdate<OnPlayerCharacter>();
        }
        
        
        
        public void OnUpdate(ref SystemState state)
        {
            var buildPhysicsWorld = SystemAPI.GetSingleton<BuildPhysicsWorldData>();
            var deployedItemLookup = SystemAPI.GetComponentLookup<DeployedItem>();
            var groundItemLookup = SystemAPI.GetComponentLookup<GroundItem>();
            
            var interactionRadius = 1f;
            
            foreach (var (playerCharacter, localTransform, entity) in SystemAPI.Query<OnPlayerCharacter, LocalTransform>().WithEntityAccess())
            {
                if(playerCharacter.ActionCommandOpt.TryGet(out var actionCommand))
                {
                    actionCommand.Switch(
                        unbuild =>
                        {
                            /*
                                public static bool TryGetFirstOverlapSphere<T0>(
                                this BuildPhysicsWorldData buildPhysicsWorldData,
                                float3 point,
                                float radius,
                                ref ComponentLookup<T0> lookUp0,
                                out Entity entity,
                                out T0 component0
                                )
                             */

                            if (buildPhysicsWorld.TryGetFirstOverlapSphere(
                                    localTransform.Position,
                                    interactionRadius,
                                    ref deployedItemLookup,
                                    out var deployedItemEntity,
                                    out var deployedItem
                                ))
                            {

                            }


                        },
                        pickupItem => {},
                        craftItem => {},
                        mineResource => {},
                        cycleStack => {}
                    );
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}