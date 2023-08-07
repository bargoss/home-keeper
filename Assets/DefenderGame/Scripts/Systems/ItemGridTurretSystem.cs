using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DefenderGame.Scripts.Components;
using HomeKeeper.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Analytics;

namespace DefenderGame.Scripts.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(ItemGridViewSystem))]
    public partial class ItemGridTurretSystem : SystemBase
    {
        private readonly List<float3> m_EnemyPositions = new();

        protected override void OnCreate()
        {
            RequireForUpdate(GetEntityQuery(typeof(DeItemGrid)));
            //RequireForUpdate(GetEntityQuery(typeof(DeEnemy)));
        }

        protected override void OnUpdate()
        {
            m_EnemyPositions.Clear();
            
            foreach (var (localTransform, deEnemy, health, entity) in SystemAPI.Query<LocalTransform, DeEnemy, Health>().WithEntityAccess())
            {
                if (health.IsDead)
                    continue;
                
                m_EnemyPositions.Add(localTransform.Position);
            }

            var enemyPresent = m_EnemyPositions.Count > 0;
            

            var enemyTargetEntity = SystemAPI.GetSingletonEntity<DeEnemyTarget>();
            var enemyTargetPosition = SystemAPI.GetComponent<LocalTransform>(enemyTargetEntity).Position;

            var closestEnemyPosition = m_EnemyPositions
                .OrderBy(pos => math.distancesq(pos, enemyTargetPosition))
                .FirstOrDefault();


            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var gamePrefabs = SystemAPI.GetSingleton<DeGamePrefabs>();
            var time = (float)SystemAPI.Time.ElapsedTime;
            var deltaTime = SystemAPI.Time.DeltaTime;

            var itemGridS = SystemAPI.ManagedAPI.GetSingleton<DeItemGrid>();
            
            var turrets = itemGridS.ItemGrid.Items.Where(item => item.item is Turret).Select((tuple, i) =>
                (tuple.item, i)).ToList();

            string msg = "";
            msg += "TurretA: " + ((Turret)turrets[0].item).LastShotTime + " ";
            msg += "TurretB: " + ((Turret)turrets[1].item).LastShotTime + "\n";
            
            Entities.ForEach((DeItemGrid itemGrid, LocalToWorld gridLocalToWorld) =>
            {
                ecb = HandleTurretUpdate(itemGrid, gridLocalToWorld, closestEnemyPosition, enemyPresent, time, deltaTime, ecb, gamePrefabs);
            }).WithoutBurst().Run();
            
            msg += "Time : " + time + " ";
            msg += "TurretA: " + ((Turret)turrets[0].item).LastShotTime + " ";
            msg += "TurretB: " + ((Turret)turrets[1].item).LastShotTime + "\n";

            Debug.Log(msg);
            
            ecb.Playback(EntityManager);
        }

        public static EntityCommandBuffer HandleTurretUpdate(DeItemGrid itemGrid, LocalToWorld gridLocalToWorld,
            float3 closestEnemyPosition, bool enemyPresent, float time, float deltaTime, EntityCommandBuffer ecb, DeGamePrefabs gamePrefabs, bool dontSpawnProjectile = false)
        {
            foreach (var (item, pivot) in itemGrid.ItemGrid.Items)
            {
                if (item is Turret turret)
                {
                    var turretPosition = (float3)ItemGridUtils.GridToWorldPos(pivot, gridLocalToWorld, itemGrid.GridLength);
                    var targetAimDirection = math.normalize(closestEnemyPosition - (float3)turretPosition);
                    if (enemyPresent)
                    {
                        turret.AimDirection =
                            math.normalize(math.lerp(turret.AimDirection, targetAimDirection, deltaTime));
                    }

                    if (
                        math.dot(turret.AimDirection, targetAimDirection) > 0.99f &&
                        turret.TryShoot(time) &&
                        enemyPresent
                    )
                    {
                        if(dontSpawnProjectile) continue;
                        var projectile = ecb.Instantiate(gamePrefabs.ProjectilePrefab);
                        var rot = quaternion.LookRotation(turret.AimDirection, Utility.Up);
                        var pos = turretPosition + Utility.Up * 0.75f;
                        ecb.SetLocalPositionRotation(projectile, pos, rot);
                        ecb.SetVelocity(projectile, turret.AimDirection * 30f);
                    }
                }
            }

            return ecb;
        }
    }
}