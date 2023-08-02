using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DefenderGame.Scripts.Components;
using HomeKeeper.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Analytics;

namespace DefenderGame.Scripts.Systems
{
    public partial class ItemGridTurretSystem : SystemBase
    {
        private readonly List<float3> m_EnemyPositions = new();

        protected override void OnCreate()
        {
            RequireForUpdate(GetEntityQuery(typeof(DeItemGrid)));
            RequireForUpdate(GetEntityQuery(typeof(DeGameData)));
            RequireForUpdate(GetEntityQuery(typeof(DeEnemy)));
            RequireForUpdate(GetEntityQuery(typeof(DeEnemyTarget)));
        }

        protected override void OnUpdate()
        {
            m_EnemyPositions.Clear();
            
            foreach (var (localTransform, deEnemy, health, entity) in SystemAPI.Query<LocalTransform, DeEnemy, Health>().WithEntityAccess())
            {
                if (health.IsDead)
                    return;
                
                m_EnemyPositions.Add(localTransform.Position);
            }
            
            if(m_EnemyPositions.Count == 0)
                return;

            var enemyTargetEntity = SystemAPI.GetSingletonEntity<DeEnemyTarget>();
            var enemyTargetPosition = SystemAPI.GetComponent<LocalTransform>(enemyTargetEntity).Position;

            var closestEnemyPosition = m_EnemyPositions
                .OrderBy(pos => math.distancesq(pos, enemyTargetPosition))
                .FirstOrDefault();


            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var gamePrefabs = SystemAPI.GetSingleton<DeGamePrefabs>();

            Entities.ForEach((DeItemGrid itemGrid, LocalToWorld gridLocalToWorld) =>
            {
                foreach (var (item, pivot) in itemGrid.ItemGrid.Items)
                {
                    if (item is Turret turret)
                    {
                        var turretPosition = (float3)ItemGridUtils.GridToWorldPos(pivot, gridLocalToWorld, itemGrid.GridLength);
                        var targetAimDirection = math.normalize(closestEnemyPosition - (float3)turretPosition);
                        turret.AimDirection = math.normalize(math.lerp(turret.AimDirection, targetAimDirection, SystemAPI.Time.DeltaTime));
                        if (
                            math.dot(turret.AimDirection, targetAimDirection) > 0.99f &&
                            turret.TryShoot((float)SystemAPI.Time.ElapsedTime)
                        )
                        {
                            var projectile = ecb.Instantiate(gamePrefabs.Enemy0Prefab);
                            var rot = quaternion.LookRotation(turret.AimDirection, Utility.Up);
                            var pos = turretPosition + Utility.Up * 0.75f;
                            ecb.SetLocalPositionRotation(projectile, pos, rot);
                            ecb.SetVelocity(projectile, turret.AimDirection * 10f);
                        }
                    }
                }
            }).WithoutBurst().Run();
        }
    }
}