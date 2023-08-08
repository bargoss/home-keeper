using Components;
using DefaultNamespace;
using HomeKeeper.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.VisualScripting;

namespace Systems
{
    /*
     * // health
            foreach (var health in SystemAPI.Query<RefRW<Health>>())
            {
                var h = health.ValueRO;
                h.HitPoints += h.RegenerationRate * SystemAPI.Time.fixedDeltaTime;
                h.HitPoints = math.clamp(h.HitPoints, 0, h.HitPoints);
                health.ValueRW = h;
            }
     */
    
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct CharacterSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var collisionWorld = SystemAPI.GetSingleton<BuildPhysicsWorldData>().PhysicsData.PhysicsWorld.CollisionWorld;
            var collisions = new NativeList<DistanceHit>(Allocator.Temp);
            
            // movement, stun regen
            foreach (var (characterMovement, physicsVelocity, characterInput, localTransform, characterState, entity) in SystemAPI.Query<RefRO<CharacterStats>, RefRW<PhysicsVelocity>, RefRO<CharacterInput>, RefRW<LocalTransform>, RefRW<CharacterState>>().WithEntityAccess())
            {
                if (SystemAPI.GetComponentLookup<Health>().TryGetComponent(entity, out var health))
                {
                    //if(health.IsDead) continue;
                }

                var velocity = physicsVelocity.ValueRO;
                var movementInput =characterInput.ValueRO.Movement;
                if (math.length(movementInput) > 1) movementInput = math.normalize(movementInput);


                if (characterState.ValueRO.StunSecondsLeft > 0)
                {
                    movementInput = float3.zero;
                }
                
                var movementSpeed = characterMovement.ValueRO.MovementSpeed;
                
                velocity.Angular = float3.zero;
                velocity.Linear = math.lerp(velocity.Linear, movementSpeed * movementInput,  SystemAPI.Time.fixedDeltaTime * 10);
                
                physicsVelocity.ValueRW = velocity;
                var lt = localTransform.ValueRO;
                var forward = lt.Forward();
                //lt.Rotation = quaternion.LookRotationSafe(math.normalize(movementInput + forward), new float3(0, 1, 0));
                lt.Rotation = quaternion.LookRotationSafe(math.right(), math.up());
                localTransform.ValueRW = lt;
                
                var cs = characterState.ValueRO;
                cs.StunSecondsLeft -= SystemAPI.Time.fixedDeltaTime;
                cs.StunSecondsLeft = math.clamp(cs.StunSecondsLeft, 0, float.MaxValue);
                characterState.ValueRW = cs;
            }
            
            // attack logic
            foreach (var (refRo, item2, refRw, item4, entity) in SystemAPI.Query<RefRO<CharacterStats>, RefRO<CharacterInput>, RefRW<LocalTransform>, RefRW<CharacterState>>().WithEntityAccess())
            {
                if (SystemAPI.GetComponentLookup<Health>().TryGetComponent(entity, out var health))
                {
                    //if(health.IsDead) continue;
                }
                
                var attackInput = item2.ValueRO.Attack;
                var lastAttackTime = item4.ValueRO.LastAttack;
                var isStunned = item4.ValueRO.StunSecondsLeft > 0;
                var onCooldown = SystemAPI.Time.ElapsedTime < refRo.ValueRO.AttackCooldown + lastAttackTime;

                if (!isStunned && !onCooldown && attackInput)
                {
                    // stun self
                    var characterStats = refRo.ValueRO;
                    var characterState = item4.ValueRO;
                    characterState.LastAttack = (float)SystemAPI.Time.ElapsedTime;
                    characterState.StunSecondsLeft += refRo.ValueRO.SelfStunDurationOnAttack;
                    item4.ValueRW = characterState;

                    var myPos = refRw.ValueRO.Position;
                    collisions.Clear();
                    collisionWorld.OverlapSphere(myPos, refRo.ValueRO.AttackRange, ref collisions, CollisionFilter.Default);
                    foreach (var distanceHit in collisions)
                    {
                        if (distanceHit.Entity != entity)
                        {
                            if (SystemAPI.GetComponentLookup<Health>().TryGetComponent(distanceHit.Entity, out var targetsHealth))
                            {
                                bool damage = false;
                                if (
                                    SystemAPI.GetComponentLookup<Faction>().TryGetComponent(distanceHit.Entity, out var targetsFaction) && 
                                    SystemAPI.GetComponentLookup<Faction>().TryGetComponent(entity, out var myFaction)
                                )
                                {
                                    if (targetsFaction.Value != myFaction.Value)
                                    {
                                        damage = true;
                                    }
                                }
                                else
                                {
                                    damage = true;
                                }
                                
                                if(damage)
                                {
                                    targetsHealth.HandleDamage(refRo.ValueRO.AttackDamage, distanceHit.Position, Utility.Up);
                                    state.EntityManager.SetComponentData(distanceHit.Entity, targetsHealth);
                                    
                                    if (SystemAPI.GetComponentLookup<CharacterState>().TryGetComponent(distanceHit.Entity, out var targetCharacterState))
                                    {
                                        targetCharacterState.StunSecondsLeft = math.max(targetCharacterState.StunSecondsLeft, characterStats.EnemyStunDurationOnAttack);
                                        state.EntityManager.SetComponentData(distanceHit.Entity, targetCharacterState);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}