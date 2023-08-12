using System;
using _OnlyOneGame.Scripts.Components;
using DefaultNamespace;
using HomeKeeper.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using ValueVariant;

namespace _OnlyOneGame.Scripts.Systems
{
    public partial struct OnPlayerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<OnPlayerCharacter>();
        }
        
        
        
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (playerCharacter, localTransform, entity) in SystemAPI.Query<OnPlayerCharacter, LocalTransform>().WithEntityAccess())
            {
                if (playerCharacter.ActionCommandOpt.TryGet(out var playerAction))
                {
                    var visitor = new TestVariantVisitor();
                    
                }
                if(playerCharacter.ActionCommandOpt.TryGet(out var actionCommand))
                {
                    actionCommand.Switch(
                        command => {},
                        command => {},
                        command => {},
                        command => {},
                        command => {}
                    );

                    Action<UnBuildCommand> a = i => { };
                    
                    MyStruct<UnBuildCommand> myStruct = new MyStruct<UnBuildCommand>();
                    myStruct.TestAction(a);
                    

                    if (actionCommand.TryGetValue(out UnBuildCommand unBuildCommand))
                    {
                        
                    }
                    else if (actionCommand.TryGetValue(out PickupItemCommand pickupItemCommand))
                    {
                        
                    }
                    else if (actionCommand.TryGetValue(out CraftItemCommand craftItemCommand))
                    {
                        
                    }
                    else if (actionCommand.TryGetValue(out MineResourceCommand mineResourceCommand))
                    {
                        
                    }
                    //else if (actionCommand.TryGetValue(out CycleStackCommand cycleStackCommand2))
                    //{
                    //    
                    //}
                    
                    else
                    {
                        throw new System.Exception("Unknown action command");
                    }
                    
                    //actionCommand.Switch(
                    //    unBuildCommand => {},
                    //    pickupItemCommand => {},
                    //    craftItemCommand => {},
                    //    mineResourceCommand => {},
                    //    cycleStackCommand => {}
                    //);
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        public struct MyStruct<T> where T: struct
        {
            public int A;
            public double B;
            public T C;
            
            public void TestAction(Action<T> action)
            {
                action(C);
            }
        }
        
        public readonly struct PlayerEventFuncVisitor : PlayerEvent.IFuncVisitor<int>
        {
            public int Visit(in MeleeAttackStartedEvent value)
            {
                throw new NotImplementedException();
            }

            public int Visit(in ItemPickedUpEvent value)
            {
                throw new NotImplementedException();
            }

            public int Visit(in ItemCraftedEvent value)
            {
                throw new NotImplementedException();
            }

            public int Visit(in UnBuiltEvent value)
            {
                throw new NotImplementedException();
            }

            public int Visit(in ResourceGatheredEvent value)
            {
                throw new NotImplementedException();
            }

            public int Visit(in ItemStackChangedEvent value)
            {
                throw new NotImplementedException();
            }

            public int Visit(in DroppedItemEvent value)
            {
                throw new NotImplementedException();
            }

            public int Visit(in ThrownItemEvent value)
            {
                throw new NotImplementedException();
            }
        }
        public readonly struct PlayerEventActionVisitor : PlayerEvent.IActionVisitor
        {
            public void Visit(in MeleeAttackStartedEvent value)
            {
                throw new NotImplementedException();
            }

            public void Visit(in ItemPickedUpEvent value)
            {
                throw new NotImplementedException();
            }

            public void Visit(in ItemCraftedEvent value)
            {
                throw new NotImplementedException();
            }

            public void Visit(in UnBuiltEvent value)
            {
                throw new NotImplementedException();
            }

            public void Visit(in ResourceGatheredEvent value)
            {
                throw new NotImplementedException();
            }

            public void Visit(in ItemStackChangedEvent value)
            {
                throw new NotImplementedException();
            }

            public void Visit(in DroppedItemEvent value)
            {
                throw new NotImplementedException();
            }

            public void Visit(in ThrownItemEvent value)
            {
                throw new NotImplementedException();
            }
        }
        public readonly struct TestVariantVisitor : PlayerEvent.IFuncVisitor<int>
        {
            public int Visit(in MeleeAttackStartedEvent value)
            {
                throw new NotImplementedException();
            }

            public int Visit(in ItemPickedUpEvent value)
            {
                throw new NotImplementedException();
            }

            public int Visit(in ItemCraftedEvent value)
            {
                throw new NotImplementedException();
            }

            public int Visit(in UnBuiltEvent value)
            {
                throw new NotImplementedException();
            }

            public int Visit(in ResourceGatheredEvent value)
            {
                throw new NotImplementedException();
            }

            public int Visit(in ItemStackChangedEvent value)
            {
                throw new NotImplementedException();
            }

            public int Visit(in DroppedItemEvent value)
            {
                throw new NotImplementedException();
            }

            public int Visit(in ThrownItemEvent value)
            {
                throw new NotImplementedException();
            }
        }

    }
}