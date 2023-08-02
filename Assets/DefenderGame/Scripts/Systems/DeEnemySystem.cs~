using DefenderGame.Scripts.Components;
using HomeKeeper.Components;
using Unity.Collections;
using Unity.Entities;

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
            
            Entities.ForEach((Entity entity, ref DeEnemy enemy, ref Health health) =>
            {
                if (health.IsDead)
                {
                    ecb.DestroyEntity(entity);
                }
            }).Schedule();

        }
    }
}