using _OnlyOneGame.Scripts.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

namespace _OnlyOneGame.Scripts.Systems
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct GroundItemActivationSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            
        }

        public void OnDestroy(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (groundItem, localTransform, physicsVelocity, entity) in SystemAPI
                         .Query<RefRO<GroundItem>, RefRO<LocalTransform>, RefRO<PhysicsVelocity>>()
                         .WithAll<ActivatedGroundItem>().WithAll<Simulate>().WithEntityAccess())
            {
                
                
                ecb.RemoveComponent<ActivatedGroundItem>(entity);
            }

            if (!ecb.IsEmpty)
            {
                ecb.Playback(state.EntityManager);
                ecb.Dispose();
            }
            
        }
        
    }
}