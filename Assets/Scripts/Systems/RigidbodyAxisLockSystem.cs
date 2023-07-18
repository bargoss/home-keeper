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
        public void OnCreate(ref SystemState state)
        {
            
        }
        
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (physicsVelocityRw, localTransformRw, localToWorldRw,rigidbodyAxisLock) in SystemAPI.Query<RefRW<PhysicsVelocity>, RefRW<LocalTransform>, RefRW<LocalToWorld>, RigidbodyAxisLock>())
            {
                var physicsVelocity = physicsVelocityRw.ValueRO;
                var localTransform = localTransformRw.ValueRO;


                var linearVelocity = physicsVelocity.Linear;
                var angularVelocity = physicsVelocity.Angular;
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

                if (rigidbodyAxisLock.LockRotation)
                {
                    angularVelocity = float3.zero;
                    localTransform.Rotation = quaternion.identity;
                    //localToWorldRw.ValueRW = new LocalToWorld()
                    //{
                    //    Value = float4x4.TRS(localToWorldRw.ValueRO.Position, quaternion.identity, localTransform.Scale)
                    //};
                }
                
                physicsVelocity.Linear = linearVelocity;
                physicsVelocity.Angular = angularVelocity;
                localTransform.Position = position;
                
                physicsVelocityRw.ValueRW = physicsVelocity;
                localTransformRw.ValueRW = localTransform;
            }
        }
    }
}