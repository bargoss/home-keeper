using _OnlyOneGame.Scripts.Components;
using DefaultNamespace;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace _OnlyOneGame.Scripts.Systems
{
    public partial struct OnPlayerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<OnPlayerCharacter>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (playerCharacter, localTransform, entity) in SystemAPI.Query<OnPlayerCharacter, LocalTransform>().WithEntityAccess())
            {
                if(playerCharacter.ActionCommand.TryGet(out var actionCommand))
                {
                    /*
                             UnBuildCommand, 
                            PickupItemCommand, 
                            CraftItemCommand, 
                            MineResourceCommand, 
                            CycleStackCommand
                     */
                     
                     //actionCommand.Switch5(
                    
                    
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}