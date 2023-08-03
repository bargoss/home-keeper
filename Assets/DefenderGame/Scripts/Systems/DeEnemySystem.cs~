using DefenderGame.Scripts.Components;
using HomeKeeper.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace DefenderGame.Scripts.Systems
{
    public partial class DeEnemySystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<DeEnemy>();
            RequireForUpdate<Health>();
        }
        protected override void OnUpdate()
        {
            //var prefabs = SystemAPI.GetSingleton<DeGamePrefabs>();

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var targetEntity = SystemAPI.GetSingletonEntity<DeEnemyTarget>();
            var targetEntityLtw = SystemAPI.GetComponent<LocalToWorld>(targetEntity);
            var targetPosition = targetEntityLtw.Position;
            
            Entities.ForEach((Entity entity, ref DeEnemy enemy, ref Health health, ref LocalTransform localTransform, ref PhysicsVelocity physicsVelocity) =>
            {
                if (health.IsDead)
                {
                    ecb.DestroyEntity(entity);
                    return;
                }
                
                var movementDirection = math.normalize(targetPosition - localTransform.Position);
                var movementSpeed = enemy.MovementSpeed;
                var targetVelocity = movementDirection * movementSpeed;
                
                
                physicsVelocity.Linear = math.lerp(physicsVelocity.Linear, targetVelocity,  SystemAPI.Time.fixedDeltaTime * 10);
                physicsVelocity.Angular = math.lerp(physicsVelocity.Angular, float3.zero,  SystemAPI.Time.fixedDeltaTime * 10);
                
                var targetRotation = quaternion.LookRotationSafe(math.normalize(movementDirection + localTransform.Forward()*0.1f), math.up());

                localTransform.Rotation = math.slerp(localTransform.Rotation, targetRotation, SystemAPI.Time.fixedDeltaTime * 10);
                 
            }).Run();

            ecb.Playback(EntityManager);
        }
    }
}