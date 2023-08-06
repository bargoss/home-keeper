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
            
            Entities.ForEach((Entity entity, ref DeEnemy enemy, ref CharacterMovement characterMovement, in Health health, in LocalTransform localTransform) =>
            {
                if (health.IsDead)
                {
                    ecb.DestroyEntity(entity);
                }
                
                var movementDirection = math.normalize(targetPosition - localTransform.Position);
                
                characterMovement.MovementInput = movementDirection;

            }).Run();

            ecb.Playback(EntityManager);
        }
    }
}