using _OnlyOneGame.Scripts.Components;
using _OnlyOneGame.Scripts.Components.Deployed;
using Components;
using DefaultNamespace;
using HomeKeeper.Components;
using Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace _OnlyOneGame.Scripts.Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateAfter(typeof(HealthSystem))]
    public partial struct OnTurretSystem : ISystem
    {
        //private NativeList<(float3, Entity, RefRW<Health>)> m_OverlapSphereResults;
        private NativeList<(float3, Entity)> m_Cache;
        
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<OnPrefabs>();
            state.RequireForUpdate<BuildPhysicsWorldData>();
            state.RequireForUpdate<OnTurret>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            var buildPhysicsWorld = SystemAPI.GetSingleton<BuildPhysicsWorldData>();
            var prefabs = SystemAPI.GetSingleton<OnPrefabs>();
            
            var healthLookup = state.GetComponentLookup<Health>(true);
            var factionLookup = state.GetComponentLookup<Faction>(true);

            
            foreach (var (onTurretRw, localTransform, entity) in SystemAPI.Query<RefRW<OnTurret>, LocalTransform>().WithEntityAccess())
            {
                if (healthLookup.TryGetComponent(entity, out var myHealth) && myHealth.IsDead)
                {
                    continue;
                }
                
                
                onTurretRw.ValueRW.ShootInput = false;
                onTurretRw.ValueRW.LookDirection = math.normalizesafe(onTurretRw.ValueRW.LookDirection -
                                                                      onTurretRw.ValueRW.LookDirection.y * Utility.Up);
                
                var iHaveFaction = factionLookup.TryGetComponent(entity, out var myFactionOpt);
                
                buildPhysicsWorld.TryGetAllOverlapSphereNoAlloc(
                    localTransform.Position,
                    5,
                    ref m_Cache
                );

                bool foundOne = false;
                float smallestDistanceSqr = math.INFINITY;
                float3 foundPos = float3.zero;
                
                foreach (var (targetPos, targetEntity) in m_Cache)
                {
                    if (targetEntity == entity)
                        continue;
                    
                    if(!healthLookup.TryGetComponent(targetEntity, out var targetHealthOpt))
                        continue;
                    
                    if (targetHealthOpt.HitPoints <= 0)
                        continue;

                    if (iHaveFaction && factionLookup.TryGetComponent(targetEntity, out var targetFaction) && myFactionOpt.Value == targetFaction.Value)
                        continue;
                    
                    var distanceSqr = math.distancesq(localTransform.Position, targetPos);
                    if (distanceSqr < smallestDistanceSqr)
                    {
                        smallestDistanceSqr = distanceSqr;
                        foundPos = targetPos;
                        foundOne = true;
                    }
                }
                
                if (foundOne)
                {
                    onTurretRw.ValueRW.LookDirection = math.normalize(foundPos - localTransform.Position);
                    onTurretRw.ValueRW.ShootInput = true;
                }
                
            }


            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            // handle shooting
            foreach (var (onTurretRw, localTransform, entity) in SystemAPI.Query<RefRW<OnTurret>, LocalTransform>()
                         .WithEntityAccess())
            {
                var projectilePrefab = prefabs.ProjectilePrefab;

                if (onTurretRw.ValueRO.ShootInput && onTurretRw.ValueRO.LastShot.TicksSince(networkTime.ServerTick) > 10)
                {
                    onTurretRw.ValueRW.LastShot = networkTime.ServerTick;
                    var projectile = ecb.Instantiate(projectilePrefab);
                    // public static void SetLocalPositionRotation(this ref EntityCommandBuffer commandBuffer, Entity entity, float3 position, quaternion rotation)
                    ecb.SetLocalPositionRotation(projectile,
                        localTransform.Position + onTurretRw.ValueRO.LookDirection * 1.0f + Utility.Up * 0.5f,
                        quaternion.LookRotationSafe(onTurretRw.ValueRO.LookDirection, Utility.Up));
                    
                    ecb.SetVelocity(projectile, onTurretRw.ValueRO.LookDirection * 20.0f);
                }
            }
            
            if (!ecb.IsEmpty)
            {
                ecb.Playback(state.EntityManager);
            }
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}