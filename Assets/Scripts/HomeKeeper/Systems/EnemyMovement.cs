using DefaultNamespace;
using HomeKeeper.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace HomeKeeper.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct EnemyMovement : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            return;
            foreach (var (characterMovement, localToWorld, physicsVelocityRw, entity) in SystemAPI.Query<CharacterMovement2, LocalToWorld, RefRW<PhysicsVelocity>>().WithEntityAccess())
            {
                var physicsVelocity = physicsVelocityRw.ValueRO;
                
                
                // lerp it
                var desiredVelocity =
                    math.lerp(physicsVelocity.Linear,
                        characterMovement.DirectionInput * characterMovement.Stats.MaxSpeed,
                        characterMovement.Stats.AccelerationMultiplier * SystemAPI.Time.DeltaTime
                    );
                
                var desiredDelta = desiredVelocity - physicsVelocity.Linear;
                var delta = desiredDelta.ClampMagnitude(characterMovement.Stats.MaxAcceleration);
                
                physicsVelocity.Linear += delta;
                
                
                physicsVelocityRw.ValueRW = physicsVelocity;
            }
        }
    }
}