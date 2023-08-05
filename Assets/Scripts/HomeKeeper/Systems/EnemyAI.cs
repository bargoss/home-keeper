using Components;
using HomeKeeper.Components;
using Systems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace HomeKeeper.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct EnemyAI : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (enemy, localToWorld, characterMovementRw, entity) in SystemAPI.Query<Enemy, LocalToWorld,RefRW<CharacterMovement2>>().WithEntityAccess())
            {
                var characterMovement = characterMovementRw.ValueRO;
                
                characterMovement.DirectionInput = math.normalizesafe(float3.zero - localToWorld.Position);
                
                characterMovementRw.ValueRW = characterMovement;
            }
        }
    }
}