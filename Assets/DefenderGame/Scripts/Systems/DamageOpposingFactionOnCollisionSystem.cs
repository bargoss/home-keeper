using Unity.Burst;
using Unity.Entities;

namespace DefenderGame.Scripts.Systems
{
    public partial struct DamageOpposingFactionOnCollisionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // todo
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}