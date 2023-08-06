using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.GoViews;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace DefenderGame.Scripts.Systems
{
    public partial class CharacterViewSystem : SystemBase
    {
        private PairMaintainer<CharacterView, CharacterGOView> m_PairMaintainer = new(
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
            RequireForUpdate<CharacterView>();
        }
        protected override void OnUpdate()
        {
            var random = new Random(51612323);
            foreach (var characterViewAspect in SystemAPI.Query<CharacterViewAspect>())
            {
                if (characterViewAspect.CharacterView.ValueRO.ViewIdAssigned == false)
                {
                    characterViewAspect.CharacterView.ValueRW.AssignViewId(random.NextInt());
                }
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

        
        
        public void HandleFixedUpdateOfView(CharacterGOView view)
        {
            view.HandleFixedUpdate(
                m_PhysicsVelocity.ValueRO.Linear,
                m_CharacterMovement.ValueRO.LookInput,
                m_CharacterMovement.ValueRO.IsGrounded,
                m_CharacterMeleeCombat is { IsValid: true, ValueRO: { Attacked: true } }
            );
        }
        
        
    }
}