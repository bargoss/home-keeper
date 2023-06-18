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
        private VectorField m_VectorField;

        public void OnCreate(ref SystemState state)
        {
            m_VectorField = new VectorField(0.25f);
        }

        public void OnUpdate(ref SystemState state)
        {
            var domePosition = new float3(0, 0, 0);

            foreach (var (dome, localTransform, entity) in SystemAPI.Query<Dome, LocalTransform>().WithEntityAccess())
            {
                domePosition = localTransform.Position;
            }


            foreach (var (flyingMeleeEnemyRw, enemyStateRw, localTransform, physicsVelocityRw, entity) in SystemAPI
                         .Query<RefRW<FlyingMeleeEnemy>, RefRW<EnemyState>, LocalTransform, RefRW<PhysicsVelocity>>()
                         .WithEntityAccess())
            {
                var flyingMeleeEnemy = flyingMeleeEnemyRw.ValueRO;
                var enemyState = enemyStateRw.ValueRO;
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
                    desiredLinearVelocity = math.normalizesafe(m_VectorField.Sample(localTransform.Position)) *
                                            flyingMeleeEnemy.MaxSpeed;
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