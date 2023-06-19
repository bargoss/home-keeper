using Components;
using Unity.Entities;
using Unity.Physics.Stateful;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ProjectileSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (statefulCollisionEvents, entity) in SystemAPI.Query<DynamicBuffer<StatefulCollisionEvent>>().WithEntityAccess())
            {
                Debug.Log("collision count: " + statefulCollisionEvents.Length + ",\nentity index: " + entity.Index);
            }
            
            //var collisionWorld = SystemAPI.GetSingleton<BuildPhysicsWorldData>().PhysicsData.PhysicsWorld.CollisionWorld;
            //var collisions = new NativeList<DistanceHit>(Allocator.Temp);
            //
            //SystemAPI.GetSingleton<>().
            //foreach (
            //    var (projectileRw, localTransform, physicsVelocityRw, entity)
            //    in SystemAPI.Query<RefRW<Projectile>, LocalTransform, RefRW<PhysicsVelocity>>().WithEntityAccess()
            //)
            //{
            //    
            //}
        }
    }
}