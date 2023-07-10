using Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Aspects;
using Unity.Physics.Stateful;
using Unity.Transforms;
using Unity.VisualScripting;

namespace DefaultNamespace.SwarmRunner.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct BrakeBeamsOnCollisionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var beamLookup = SystemAPI.GetComponentLookup<BrakeBeamsOnCollision>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (brakeBeamsOnCollision, statefulCollisionEvents, entity) in SystemAPI.Query<BrakeBeamsOnCollision, DynamicBuffer<StatefulCollisionEvent>>().WithEntityAccess())
            {
                foreach (var statefulCollisionEvent in statefulCollisionEvents)
                {
                    var other = statefulCollisionEvent.GetOtherEntity(entity);
                    if(beamLookup.TryGetComponent(other, out var beam))
                    {
                        ecb.DestroyEntity(other);
                    }
                }
            }   
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct FormBeamsOnCollisionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var beamLookup = SystemAPI.GetComponentLookup<BrakeBeamsOnCollision>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (brakeBeamsOnCollision, statefulCollisionEvents,colliderAspect , entity) in SystemAPI.Query<BrakeBeamsOnCollision, DynamicBuffer<StatefulCollisionEvent>, ColliderAspect>().WithEntityAccess())
            {
                colliderAspect.GetCollisionResponse(new ColliderKey());
                //public unsafe void GetColliderKeyToChildrenMapping(ref NativeHashMap<ColliderKey, ChildCollider> colliderKeyToChildrenMapping)
                var colliderKeyToChildrenMapping = new NativeHashMap<ColliderKey, ChildCollider>();
                colliderAspect.GetColliderKeyToChildrenMapping(ref colliderKeyToChildrenMapping);
                var childCollider = colliderKeyToChildrenMapping[new ColliderKey()];
                //childCollider.
                
                foreach (var statefulCollisionEvent in statefulCollisionEvents)
                {
                    var other = statefulCollisionEvent.GetOtherEntity(entity);
                    if(beamLookup.TryGetComponent(other, out var beam))
                    {
                        ecb.DestroyEntity(other);
                    }
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
    
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct SoftJointSystem : ISystem
    {
        // follow the closest opposing faction member and attack if in range
        public void OnUpdate(ref SystemState state)
        {
            var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
            var separateJointsOnCollisionLookup = SystemAPI.GetComponentLookup<BrakeBeamsOnCollision>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var softJointAspect in SystemAPI.Query<BeamAspect>())
            {
                var goingToBeDestroyed = false;
                
                // react to collisions
                foreach (var statefulCollisionEvent in softJointAspect.StatefulCollisionEvents)
                {
                    var otherEntity = statefulCollisionEvent.GetOtherEntity(softJointAspect.Self);
                    if (separateJointsOnCollisionLookup.HasComponent(otherEntity))
                    {
                        goingToBeDestroyed = true;
                    }
                }
                
                // set length of beam
                if (softJointAspect.TrySetScale(ref localToWorldLookup))
                {
                    goingToBeDestroyed = true;
                }
                
                
                if(goingToBeDestroyed) ecb.DestroyEntity(softJointAspect.Self);
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}