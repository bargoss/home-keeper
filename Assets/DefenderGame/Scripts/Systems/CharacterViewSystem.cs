using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.GoViews;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace DefenderGame.Scripts.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class CharacterViewSystem : SystemBase
    {
        private readonly PairMaintainer<CharacterView, CharacterGOView> m_PairMaintainer = new(
            logical =>
            {
                var characterView = Object.Instantiate(GameResources.Instance.CharacterGOViewPrefab);
                characterView.Restore();
                return characterView;
            },
            view =>
            {
                Object.Destroy(view.gameObject);
            }
        );

        protected override void OnCreate()
        {
            //RequireForUpdate<CharacterView>(); // this line creates problems when disposing the views
        }
        protected override void OnUpdate()
        {
            var random = new Random((uint)(SystemAPI.Time.ElapsedTime * 10000));
            foreach (var characterViewAspect in SystemAPI.Query<CharacterViewAspect>())
            {
                var characterView = characterViewAspect.CharacterView.ValueRO; 
                if (characterView.ViewIdAssigned == false)
                {
                    characterView.AssignViewId(random.NextInt());
                }
                
                characterViewAspect.CharacterView.ValueRW = characterView;
                
                
                var viewPair = m_PairMaintainer.GetOrCreateView(characterViewAspect.CharacterView.ValueRO);
                characterViewAspect.HandleFixedUpdateOfView(viewPair);
            }
            
            m_PairMaintainer.DisposeAndClearUntouchedViews();
        }
    }

    public readonly partial struct CharacterViewAspect : IAspect
    {
        public readonly Entity Self;

        public readonly RefRW<CharacterView> CharacterView;
        private readonly RefRO<LocalTransform> m_LocalTransform;
        private readonly RefRO<PhysicsVelocity> m_PhysicsVelocity;
        private readonly RefRO<CharacterMovement> m_CharacterMovement;
        [Optional] private readonly RefRO<CharacterMeleeCombat> m_CharacterMeleeCombat;

        
        private float3 GetSafeLookDirection()
        {
            var lookDirection = m_CharacterMovement.ValueRO.MovementInput;
            if (math.lengthsq(lookDirection) < 0.0001f)
            {
                return m_LocalTransform.ValueRO.Forward();
            }
            return lookDirection;
        }
        
        
        public void HandleFixedUpdateOfView(CharacterGOView view)
        {
            view.HandleFixedUpdate(
                m_LocalTransform.ValueRO.Position,
                m_PhysicsVelocity.ValueRO.Linear,
                GetSafeLookDirection(),
                m_CharacterMovement.ValueRO.IsGrounded,
                m_CharacterMeleeCombat is { IsValid: true, ValueRO: { Attacked: true } }
            );
        }
        
        
    }
}