using HomeKeeper.Authoring;
using HomeKeeper.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace HomeKeeper.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct ChildForOneFrameSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var physicsVelocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>();
            
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            foreach (var (childForOneFrame, entity) in SystemAPI.Query<ChildForOneFrame>().WithEntityAccess())
            {
                var parentLocalToWorld = SystemAPI.GetComponent<LocalToWorld>(childForOneFrame.Parent);
                var parentTransform = parentLocalToWorld.Value;
                var childLocalTransform = childForOneFrame.LocalTransform;
                var childWorldTransform = math.mul(parentTransform, childLocalTransform);
                
                ecb.RemoveComponent<ChildForOneFrame>(entity);
                ecb.SetComponent(entity, LocalTransform.FromMatrix(childWorldTransform));
                ecb.SetComponent(entity, new LocalToWorld { Value = childWorldTransform });
                
                if(physicsVelocityLookup.TryGetComponent(childForOneFrame.Parent, out var parentPhysicsVelocity) &&
                   physicsVelocityLookup.HasComponent(entity)
                )
                {
                    ecb.SetComponent(entity, new PhysicsVelocity()
                    {
                        Angular = float3.zero,
                        Linear = parentPhysicsVelocity.Linear
                    });
                }
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

}