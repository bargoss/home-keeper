using DefenderGame.Scripts.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics.Stateful;

namespace DefenderGame.Scripts.Systems
{
    public partial struct DestroyOthersOnCollisionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DestroyOthersOnCollision>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (destroyOthersOnCollision, statefulCollisionEvents, entity) in SystemAPI.Query<DestroyOthersOnCollision, DynamicBuffer<StatefulCollisionEvent>>().WithEntityAccess())
            {
                foreach (var statefulCollisionEvent in statefulCollisionEvents)
                {
                    var other = statefulCollisionEvent.GetOtherEntity(entity);
                    
                }
            }
        }
        

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}