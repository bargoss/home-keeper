using DefaultNamespace;
using RunnerGame.Scripts.ECS.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Stateful;
using Unity.Transforms;

namespace RunnerGame.Scripts.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ParticleAcceleratorSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //state.CompleteDependency(); // we get errors without this
            
            //foreach (var (particleAccelerator, localToWorld, entity) in SystemAPI.Query<ParticleAccelerator, LocalToWorld>().WithEntityAccess())
            //foreach (var (particleAccelerator, localToWorld, statefulTriggerEvents, entity) in SystemAPI.Query<ParticleAccelerator, LocalToWorld, DynamicBuffer<StatefulTriggerEvent>>().WithEntityAccess())
            //{
            //    foreach (var statefulTriggerEvent in statefulTriggerEvents)
            //    {
            //        var other = statefulTriggerEvent.GetOtherEntity(entity);
            //        if (SystemAPI.GetComponentLookup<PhysicsVelocity>().TryGetRw(other, out var physicsVelocityRw))
            //        {
            //            var physicsVelocity = physicsVelocityRw.ValueRO;
            //            
            //            physicsVelocity.Linear += math.mul(localToWorld.Rotation, particleAccelerator.Acceleration) * SystemAPI.Time.DeltaTime;
            //            
            //            physicsVelocityRw.ValueRW = physicsVelocity;
            //        }
            //    }
            //}
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}