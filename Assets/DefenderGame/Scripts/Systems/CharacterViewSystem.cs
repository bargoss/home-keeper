using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.GoViews;
using HomeKeeper.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using ValueVariant;
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
        [Optional] private readonly RefRO<Health> m_Health;

        
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
            //if (m_Health is { IsValid: true, ValueRO: { IsDead: false } })
            if (m_Health.IsValid && m_Health.ValueRO.Status.TryGetValue(out HealthStatus.Alive _))
            {
                view.HandleFixedUpdate(
                    m_LocalTransform.ValueRO.Position,
                    m_PhysicsVelocity.ValueRO.Linear,
                    GetSafeLookDirection(),
                    m_CharacterMovement.ValueRO.IsGrounded,
                    m_CharacterMeleeCombat is { IsValid: true, ValueRO: { Attacked: true } }
                );
            }
            else if (m_Health is { IsValid: true, ValueRO: { DiedNow: true } })
            {
                view.SetRagdoll(true);
                view.ApplyExplosionForceToRagdoll(m_Health.ValueRO.BiggestDamagePosition, 750, 2);
                //var normal = math.normalizesafe(m_Health.ValueRO.BiggestDamagePosition - m_LocalTransform.ValueRO.Position, new float3(0,1,0));
                var normal = m_Health.ValueRO.BiggestDamageNormal;
                //var cameraForward = (float3)Camera.main.transform.forward;
                // rotate away from camera towards camera forward plane
                //normal = math.normalize(normal - cameraForward * math.dot(normal, cameraForward));
                    

                PoolManager.Instance.PlayBloodEffect(m_Health.ValueRO.BiggestDamagePosition - normal * 2.0f * 0, normal);
            }
            else
            {
                
            }
        }
        
        
    }
}