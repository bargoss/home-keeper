using _OnlyOneGame.Scripts.Components.Deployed;
using DefaultNamespace;
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
    [UpdateBefore(typeof(CharacterViewSystem))]
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
            
            foreach (var (onMinionRw, onMinionAIRw, localTransform, entity) in SystemAPI.Query<RefRW<OnMinion>, RefRW<OnMinionAI>, LocalTransform>().WithAll<Simulate>().WithEntityAccess())
            {
                physics.GetAllOverlapSphereNoAlloc(
                    localTransform.Position,
                    onMinionAIRw.ValueRO.CanAttackRange,
                    ref m_OverlapSphereResultBuffer
                );

                foreach (var (targetPos, targetEntity) in m_OverlapSphereResultBuffer)
                {
                    if (targetEntity == entity)
                    {
                        continue;
                    }
                    
                    var delta = targetPos - localTransform.Position;
                    var sqrDistance = math.lengthsq(delta);
                    var maxSquaredDistance = onMinionAIRw.ValueRO.CanAttackRange * onMinionAIRw.ValueRO.CanAttackRange;
                    if(sqrDistance < maxSquaredDistance)
                    {
                        var deltaNormalized = math.normalize(delta);
                        onMinionRw.ValueRW.LookDirection = deltaNormalized;
                        onMinionRw.ValueRW.AttackInput = true;
                        wip
                    }
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