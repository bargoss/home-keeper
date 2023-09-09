using DefaultNamespace;
using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.GoViews;
using HomeKeeper.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using ValueVariant;
using Random = Unity.Mathematics.Random;

namespace DefenderGame.Scripts.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)] //  | WorldSystemFilterFlags.ServerSimulation
    public partial class CharacterViewSystem : SystemBase
    {
        private readonly PairMaintainer<ViewId, CharacterGOView> m_PairMaintainer = new(
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
            var random = new Random((uint)(SystemAPI.Time.ElapsedTime * 10000 + 1));
            var deltaTime = SystemAPI.Time.DeltaTime;
            
            foreach (var (characterViewRw, localTransform) in SystemAPI.Query<RefRW<CharacterView>, LocalTransform>())
            {
                var characterView = characterViewRw.ValueRO;
                
                if (!characterView.ViewId.Assigned)
                {
                    characterView.ViewId = new ViewId(random.NextInt());
                }
                
                
                characterViewRw.ValueRW = characterView;
                
                
                var viewPair = m_PairMaintainer.GetOrCreateView(characterView.ViewId);

                viewPair.SetDead(characterView.Dead, characterView.RagdollForcePoint, characterView.RagdollForce);

                var offsetForServer = World.Flags.HasFlag(WorldFlags.GameServer) ? new float3(0, 2f, 0) : float3.zero;
                viewPair.HandleFixedUpdate(
                    localTransform.Position + offsetForServer,
                    characterView.MovementVelocity.X0Y(),
                    characterView.LookDirection,
                    characterView.IsGrounded,
                    characterView.LastAttacked,
                    characterView.LastItemThrown,
                    deltaTime
                );
            }
            
            m_PairMaintainer.DisposeAndClearUntouchedViews();
        }
    }
}