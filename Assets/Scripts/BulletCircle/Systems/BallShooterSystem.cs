using Unity.Entities;

namespace BulletCircle.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct BallShooterSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var commandBuffer = new EntityCommandBuffer();
            var b = SystemAPI.TryGetSingleton<GameResources>(out var a);
            
            foreach (var (ballShooterRw, entity) in SystemAPI.Query<RefRW<BallShooter>>().WithEntityAccess())
            {
                var ballShooter = ballShooterRw.ValueRO;
                
                var fireCooldown = 1.0f / ballShooter.FireRate;
                
                var nextShot = ballShooter.LastShootTime + fireCooldown;

                if ((float)SystemAPI.Time.ElapsedTime > nextShot && ballShooter.Shooting)
                {
                    ballShooter.LastShootTime = (float)SystemAPI.Time.ElapsedTime;
                    
                    var ball = Ball.Default;
                    ball.Money = ballShooter.BallMoney;
                    
                    var ballEntity = commandBuffer.Instantiate(a.BallPrefabs[ballShooter.BallPrefabIndex]);
                    commandBuffer.SetComponent(ballEntity, ball);
                    
                    ballShooterRw.ValueRW = ballShooter;
                }
            }
            commandBuffer.Playback(state.EntityManager);
        }
    }
}