using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(CharacterSystem))]
    public partial struct CharacterAISystem : ISystem
    {
        // follow the closest opposing faction member and attack if in range
        public void OnUpdate(ref SystemState state)
        {
            var collisionWorld = SystemAPI.GetSingleton<BuildPhysicsWorldData>().PhysicsData.PhysicsWorld.CollisionWorld;
            var collisions = new NativeList<DistanceHit>(Allocator.Temp);
            
            // iterate enemy controlled characters
            foreach (var (localToWorld, characterInputRW , faction, characterStats, entity) in SystemAPI.Query<LocalToWorld, RefRW<CharacterInput>, Faction, CharacterStats>().WithAll<EnemyControlled>().WithEntityAccess())
            {
                var characterInput = characterInputRW.ValueRO;
                characterInput.Attack = false;
                characterInput.Movement = float3.zero;
                
                collisions.Clear();
                collisionWorld.OverlapSphere(localToWorld.Position, 8, ref collisions, CollisionFilter.Default);

                var closestTarget = new Entity();
                var hasTarget = false;
                var closestDistance = float.MaxValue;
                var closestTargetPosition = float3.zero;

                foreach (var hit in collisions)
                {
                    if (
                        SystemAPI.GetComponentLookup<Faction>().TryGetComponent(hit.Entity, out var targetFaction1) &&
                        SystemAPI.GetComponentLookup<Health>().TryGetComponent(hit.Entity, out var targetHealth1)
                    )
                    {
                        var isValidTarget = targetFaction1.Value != faction.Value && !targetHealth1.IsDead; 
                        
                        if(isValidTarget)
                        {
                            if(hit.Distance < closestDistance)
                            {
                                closestDistance = hit.Distance;
                                closestTarget = hit.Entity;
                                hasTarget = true;
                                closestTargetPosition = hit.Position;
                            }
                        }
                    }
                }
                
                {
                    if (hasTarget)
                    {
                        if (closestDistance < characterStats.AttackRange)
                        {
                            characterInput.Attack = true;
                        }
                        else
                        {
                            characterInput.Movement = math.normalize(closestTargetPosition - localToWorld.Position);
                        }
                    }
                }


                characterInputRW.ValueRW = characterInput;
            }   
        }
    }
}