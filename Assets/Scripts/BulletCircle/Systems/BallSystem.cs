using Unity.Entities;

namespace BulletCircle.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct BallSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer();
            foreach (var (ball, entity) in SystemAPI.Query<Ball>().WithEntityAccess())
            {
                if (ball.Bounces >= ball.MaxBounces)
                {
                    commandBuffer.DestroyEntity(entity);
                }
            }
            commandBuffer.Playback(state.EntityManager);
        }
    }
}