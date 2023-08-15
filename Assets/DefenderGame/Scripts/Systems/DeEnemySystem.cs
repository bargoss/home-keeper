using DefenderGame.Scripts.Components;
using HomeKeeper.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Aspects;
using Unity.Physics.Authoring;
using Unity.Transforms;
using ValueVariant;

namespace DefenderGame.Scripts.Systems
{
    public partial class DeEnemySystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<DeEnemy>();
            //RequireForUpdate<Health>();
        }
        protected override void OnUpdate()
        {
            //var prefabs = SystemAPI.GetSingleton<DeGamePrefabs>();

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var targetPosition = float3.zero;
            if (SystemAPI.TryGetSingletonEntity<DeEnemyTarget>(out var targetEntity))
            {
                targetPosition = SystemAPI.GetComponent<LocalToWorld>(targetEntity).Position;
            }
            

            foreach (var (rb,
                         deEnemy,
                         characterMovementRw,
                         colliderAspect,
                         healthRo,
                         localTransformRo,
                         entity
                         )
                     in SystemAPI
                         .Query<RigidBodyAspect, DeEnemy, RefRW<CharacterMovement>, ColliderAspect,
                             RefRO<Health>, RefRO<LocalTransform>>().WithEntityAccess())
            {
                var health = healthRo.ValueRO;
                if (health.DiedNow)
                {
                    
                    //rb.LinearVelocity = float3.zero;
                    //rb.AngularVelocityLocalSpace = float3.zero;
                    //colliderAspect.SetCollisionResponse(CollisionResponsePolicy.None);
                    ecb.RemoveComponent<PhysicsWorldIndex>(entity);
                }
                //if (health.IsDead && (float)SystemAPI.Time.ElapsedTime > health.DeathTime + 2)
                if (health.Status.TryGetValue(out HealthStatus.Dead dead) && (float)SystemAPI.Time.ElapsedTime > dead.DeathTime + 2)
                {
                    ecb.DestroyEntity(entity);
                }
                
                var movementDirection = math.normalize(targetPosition - localTransformRo.ValueRO.Position);
                //movementDirection = new float3(1, 0, 0);
                var characterMovement = characterMovementRw.ValueRO;
                characterMovement.MovementInput = new float2(movementDirection.x, movementDirection.z);
                characterMovementRw.ValueRW = characterMovement;
            }

            /*
            Entities.ForEach((Entity entity, RigidBodyAspect rb, DeEnemy enemy, ref CharacterMovement characterMovement, ref PhysicsCollider collider, in Health health, in LocalTransform localTransform) =>
            {
                if (health.IsDead)
                {
                    rb.IsKinematic = true;
                    collider = new PhysicsCollider(); // disable the collider
                    //collider.Value.Value.Type
                    // deactivate physics velocity
                }
                if (health.IsDead && (float)SystemAPI.Time.ElapsedTime > health.DeathTime + 2)
                {
                    ecb.DestroyEntity(entity);
                }
                
                var movementDirection = math.normalize(targetPosition - localTransform.Position);
                
                characterMovement.MovementInput = movementDirection;

            }).Run();
            */

            ecb.Playback(EntityManager);
        }
    }
}