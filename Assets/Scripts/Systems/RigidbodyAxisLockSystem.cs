using Components;
using HomeKeeper.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct RigidbodyAxisLockSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (physicsVelocityRw, localTransformRw, rigidbodyAxisLock) in SystemAPI.Query<RefRW<PhysicsVelocity>, RefRW<LocalTransform>, RigidbodyAxisLock>())
            {
                var physicsVelocity = physicsVelocityRw.ValueRO;
                var localTransform = localTransformRw.ValueRO;


                var linearVelocity = physicsVelocity.Linear;
                var position = localTransform.Position;
                
                if (rigidbodyAxisLock.LockLinearX)
                {
                    linearVelocity.x = 0;
                    position.x = 0;
                }
                if (rigidbodyAxisLock.LockLinearY)
                {
                    linearVelocity.y = 0;
                    position.y = 0;
                }
                if (rigidbodyAxisLock.LockLinearZ)
                {
                    linearVelocity.z = 0;
                    position.z = 0;
                }
                
                physicsVelocity.Linear = linearVelocity;
                localTransform.Position = position;
                
                physicsVelocityRw.ValueRW = physicsVelocity;
                localTransformRw.ValueRW = localTransform;
            }
        }
    }
}