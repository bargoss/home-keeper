using DefenderGame.Scripts.Components;
using HomeKeeper.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace DefenderGame.Scripts.Systems
{
    public partial struct CharacterMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CharacterMovement>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //SystemAPI.Query<CharacterMovement, LocalTransform, PhysicsVelocity>()
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
    
    public readonly partial struct CharacterMovementAspect : IAspect
    {
        public readonly Entity Self;
        private readonly RefRW<CharacterMovement> m_CharacterMovement;
        private readonly RefRO<LocalTransform> m_LocalTransform;
        //private readonly RefRO<Health>


    }
}