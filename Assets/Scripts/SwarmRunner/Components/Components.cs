using HomeKeeper.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Aspects;
using Unity.Physics.Stateful;
using Unity.Transforms;

namespace DefaultNamespace.SwarmRunner
{
    /*
     * SoftJoint will be on a separate entity and it will have its own trigger collider
     */
    
    public struct Beam : IComponentData
    {
        public Entity BodyA;
        public Entity BodyB;
        public float FormedTime;
    }

    public readonly partial struct BeamAspect : IAspect
    {
        public readonly Entity Self;
        private readonly RefRW<Beam> m_SoftJoint;
        private readonly RefRO<LocalToWorld> m_LocalToWorld;
        
        private readonly RefRW<LocalTransform> m_LocalTransform;
        private readonly RefRW<PostTransformMatrix> m_PostTransformMatrix;
        
        public readonly DynamicBuffer<StatefulCollisionEvent> StatefulCollisionEvents;
        
        public bool TrySetScale(ref ComponentLookup<LocalToWorld> localToWorldLookup)
        {
            if (localToWorldLookup.TryGetComponent(m_SoftJoint.ValueRO.BodyA, out var localToWorldA) &&
                localToWorldLookup.TryGetComponent(m_SoftJoint.ValueRO.BodyB, out var localToWorldB))
            {
                var positionA = localToWorldA.Position;
                var positionB = localToWorldB.Position;
                
                var direction = positionB - positionA;
                var length = math.length(direction);
                var scale = new float3(0.2f, 0.2f, length);
                var rotation = quaternion.LookRotationSafe(direction, math.up());
                var translation = positionA + direction * 0.5f;
                var localTransform = new LocalTransform
                {
                    Position = translation,
                    Rotation = rotation,
                    Scale = 1
                };

                var postTransformMatrix = new PostTransformMatrix
                {
                    Value = float4x4.TRS(translation, rotation, scale),
                };

                m_LocalTransform.ValueRW = localTransform;
                m_PostTransformMatrix.ValueRW = postTransformMatrix;

                return true;
            }

            return false;
        }
    }
    
    /*
     * This will trigger the SoftJoint entities
     */
}