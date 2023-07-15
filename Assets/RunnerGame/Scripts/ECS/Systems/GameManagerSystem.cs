using DefaultNamespace;
using RunnerGame.Scripts.ECS.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace RunnerGame.Scripts.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class GameManagerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (!SystemAPI.ManagedAPI.TryGetSingleton<RgGameManager>(out var gameManager))
            {
                return;
            }
            var camera = gameManager.MainCamera;
            
            var playerHorizontalInput = Input.GetAxis("Horizontal");
            
            // move player and camera
            Entities.ForEach((Entity entity, ref PhysicsVelocity physicsVelocity, ref LocalTransform localTransform, in RgPlayer rgPlayer) =>
            {
                var velocity = physicsVelocity.Linear;
                var angularVelocity = physicsVelocity.Angular;

                var targetVelocity = Utility.Forward * gameManager.PlayerForwardSpeed + Utility.Right * playerHorizontalInput * gameManager.PlayerSidewaysP;
                velocity = math.lerp(velocity, targetVelocity, SystemAPI.Time.DeltaTime * 2);
                velocity.y = 0;
                
                // remain inside the road bounds
                var pos = localTransform.Position;
                pos.y = 0;
                if(pos.x > gameManager.RoadWidth * 0.5f)
                {
                    pos.x = gameManager.RoadWidth * 0.5f;
                    if (velocity.x > 0)
                    {
                        velocity.x = 0;
                    }
                }
                else if(pos.x < -gameManager.RoadWidth * 0.5f)
                {
                    pos.x = -gameManager.RoadWidth * 0.5f;
                    if (velocity.x < 0)
                    {
                        velocity.x = 0;
                    }
                }
                localTransform.Position = pos;

                angularVelocity = 0;

                physicsVelocity.Linear = velocity;
                physicsVelocity.Angular = angularVelocity;
                localTransform.Rotation = quaternion.LookRotation(Utility.Forward, Utility.Up);
                

                camera.position = (Vector3)localTransform.Position + new Vector3(0, 8, -15);
                camera.LookAt(localTransform.Position);
            }).WithoutBurst().Run();
        }
    }
}