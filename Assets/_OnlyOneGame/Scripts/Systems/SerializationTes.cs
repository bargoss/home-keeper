using _OnlyOneGame.Scripts.Components;
using Unity.Burst;
using Unity.Entities;

namespace _OnlyOneGame.Scripts.Systems
{
    public partial struct SerializationTes : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }
        
        public void OnUpdate(ref SystemState state)
        {
            foreach (var character in SystemAPI.Query<OnPlayerCharacter>())
            {
                int a = 3;
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}