using RunnerGame.Scripts.ECS.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace RunnerGame.Scripts.ECS.Systems
{
    public partial struct PlayerControlSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (rgPlayer, physicsVelocityRw, localToWorld, entity) in SystemAPI.Query<RgPlayer, RefRW<PhysicsVelocity>, LocalToWorld>().WithEntityAccess())
            {
                var physicsVelocity = physicsVelocityRw.ValueRO;
                
                // todo
                
                physicsVelocityRw.ValueRW = physicsVelocity;
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}