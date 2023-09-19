using _OnlyOneGame.Scripts.Components.Deployed;
using DefaultNamespace;
using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace _OnlyOneGame.Scripts.Systems
{
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(MinionSystem))]
    public partial struct MinionAISystem : ISystem
    {
        private NativeList<(float3, Entity)> m_OverlapSphereResultBuffer;
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ClientServerTickRate>();
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<BuildPhysicsWorldData>();
            m_OverlapSphereResultBuffer = new NativeList<(float3, Entity)>(Allocator.Persistent);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physics = SystemAPI.GetSingleton<BuildPhysicsWorldData>();
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            var clientServerTickRate = SystemAPI.GetSingleton<ClientServerTickRate>();
            
            foreach (var (onMinionRw, onMinionAIRw, characterMovementRw, localTransform, entity) in SystemAPI.Query<RefRW<OnMinion>, RefRW<OnMinionAI>, RefRW<CharacterMovement>, LocalTransform>().WithAll<Simulate>().WithEntityAccess())
            {
                onMinionRw.ValueRW.AttackInput = false;
                characterMovementRw.ValueRW.MovementInput = float2.zero;
                characterMovementRw.ValueRW.JumpInput = false;
                

                physics.GetAllOverlapSphereNoAlloc(
                    localTransform.Position,
                    onMinionAIRw.ValueRO.CombatDistanceMax,
                    ref m_OverlapSphereResultBuffer
                );

                var closestTarget = (pos: float3.zero, entity: Entity.Null);
                var closestTargetDistanceSqr = float.MaxValue;
                var maxAcceptedDistanceSqr = onMinionAIRw.ValueRO.CombatDistanceMax * onMinionAIRw.ValueRO.CombatDistanceMax;
                
                // try find the closest target
                foreach (var (targetPos, targetEntity) in m_OverlapSphereResultBuffer)
                {
                    if (targetEntity == entity)
                    {
                        continue;
                    }
                    
                    var delta = targetPos - localTransform.Position;
                    var sqrDistance = math.lengthsq(delta);
                    if(sqrDistance < closestTargetDistanceSqr && sqrDistance < maxAcceptedDistanceSqr)
                    {
                        closestTargetDistanceSqr = sqrDistance;
                        closestTarget = (targetPos, targetEntity);
                    }
                }

                // fighting
                if (closestTarget.entity != Entity.Null)
                {
                    var deltaToTarget = closestTarget.pos - localTransform.Position;
                    var distanceToTarget = math.length(deltaToTarget);
                    var directionToTarget = deltaToTarget / distanceToTarget;

                    onMinionRw.ValueRW.LookDirection = directionToTarget;
                    
                    // in attack range
                    if(distanceToTarget <= onMinionAIRw.ValueRO.AttackRange)
                    {
                        onMinionRw.ValueRW.AttackInput = true;
                    }
                    
                    // maintain desired distance
                    var desiredDistanceMin = onMinionAIRw.ValueRO.CombatDistanceMin;
                    var desiredDistanceMax = onMinionAIRw.ValueRO.CombatDistanceMax;
                    var desiredDistance = math.clamp(distanceToTarget, desiredDistanceMin, desiredDistanceMax);
                    var error = desiredDistance - distanceToTarget;
                    var correction = directionToTarget * error;
                    characterMovementRw.ValueRW.MovementInputAsXZ = correction.ClampMagnitude(1);
                }
                // moving to target position
                else
                {
                    var deltaToTarget = onMinionAIRw.ValueRO.TargetPosition - localTransform.Position;
                    var distanceToTarget = math.length(deltaToTarget);
                    var directionToTarget = deltaToTarget / distanceToTarget;
                    onMinionRw.ValueRW.LookDirection = math.normalizesafe(directionToTarget + localTransform.Forward() * 0.01f, Utility.Forward);
                    
                    var error = distanceToTarget - distanceToTarget;
                    var correction = directionToTarget * error;
                    characterMovementRw.ValueRW.MovementInputAsXZ = correction.ClampMagnitude(1);
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            m_OverlapSphereResultBuffer.Dispose();
        }
    }
}