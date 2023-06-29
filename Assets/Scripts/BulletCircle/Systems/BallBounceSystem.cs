using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Stateful;

namespace BulletCircle.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct BallBounceSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (ballBouncer, collisionEvents, entity) in SystemAPI.Query<BallBouncer, DynamicBuffer<StatefulCollisionEvent>>().WithEntityAccess())
            {
                foreach (var collisionEvent in collisionEvents)
                {
                    var otherEntity = collisionEvent.GetOtherEntity(entity);
                    
                    if (SystemAPI.GetComponentLookup<Ball>().GetRefRWOptional(otherEntity) is { IsValid: true } ballRw)
                    {
                        var ballPhysicsVelocityRw = SystemAPI.GetComponentRW<PhysicsVelocity>(otherEntity);
                        
                        var ball = ballRw.ValueRO;
                        var ballPhysicsVelocity = ballPhysicsVelocityRw.ValueRO;
                        
                        var normal = collisionEvent.Normal;
                        var forceMultiplier = ballBouncer.BounceForce;
                        var extraForce = forceMultiplier * normal;
                        ballPhysicsVelocity.Linear += extraForce;
                        ball.Bounces++;
                        
                        ballPhysicsVelocityRw.ValueRW = ballPhysicsVelocity;
                        ballRw.ValueRW = ball;
                    }
                }
            }
        }
    }
}