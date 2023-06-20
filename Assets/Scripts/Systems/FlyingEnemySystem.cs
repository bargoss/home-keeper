using System.Linq;
using Components;
using DefaultNamespace;
using DefaultNamespace.Utils;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct FlyingEnemySystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var domePosition = new float3(0, 0, 0);

            var vectorFieldConfig = VectorFieldConfig.Default;

            foreach (var (dome, localTransform, entity) in SystemAPI.Query<Dome, LocalTransform>().WithEntityAccess())
            {
                domePosition = localTransform.Position;
            }


            foreach (var (flyingMeleeEnemyRw, localTransform, physicsVelocityRw, entity) in SystemAPI
                         .Query<RefRW<FlyingMeleeEnemy>, LocalTransform, RefRW<PhysicsVelocity>>()
                         .WithEntityAccess())
            {
                var flyingMeleeEnemy = flyingMeleeEnemyRw.ValueRO;
                var physicsVelocity = physicsVelocityRw.ValueRO;

                float3 desiredLinearVelocity;

                var distanceToDome = math.distance(localTransform.Position, domePosition);
                var directionFromDome = math.normalizesafe(localTransform.Position - domePosition);
                if (distanceToDome < flyingMeleeEnemy.DesiredMinDistanceToDome)
                {
                    desiredLinearVelocity = directionFromDome * flyingMeleeEnemy.MaxSpeed;
                }
                else if (distanceToDome > flyingMeleeEnemy.DesiredMaxDistanceToDome)
                {
                    desiredLinearVelocity = -directionFromDome * flyingMeleeEnemy.MaxSpeed;
                }
                else
                {
                    desiredLinearVelocity = math.normalizesafe(
                        VectorField.Sample(localTransform.Position, vectorFieldConfig)
                    ) * flyingMeleeEnemy.MaxSpeed;

                    var clockwise =Mathf.PerlinNoise1D(((float)SystemAPI.Time.ElapsedTime * 0.1f + (float)entity.Index * 0.063f) % 1f) - 0.5f;
                    var clockwiseDirection = math.cross(directionFromDome, new float3(0, 0, 1)) * clockwise;
                    Debug.DrawRay(localTransform.Position,  math.normalize(clockwiseDirection) * 10, Color.red);
                    desiredLinearVelocity += clockwiseDirection * 30.2f;
                    
                    var up = new float3(0, 1, 0);
                    var myDir = math.normalizesafe(localTransform.Position - domePosition); 
                    var angle = math.acos(math.dot(myDir, up));
                    var angleAbs = math.abs(angle);

                    desiredLinearVelocity += angleAbs * new float3(0, 15, 0);
                }

                physicsVelocity.Linear = Utility.GoTowardsWithClampedMag(
                    physicsVelocity.Linear,
                    desiredLinearVelocity,
                    flyingMeleeEnemy.Acceleration * Time.fixedDeltaTime
                );

                physicsVelocity.Angular =
                    math.lerp(physicsVelocity.Angular, float3.zero, SystemAPI.Time.fixedDeltaTime);


                physicsVelocityRw.ValueRW = physicsVelocity;
            }
        }
    }
}