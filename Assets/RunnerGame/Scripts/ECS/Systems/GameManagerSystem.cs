using DefaultNamespace;
using RunnerGame.Scripts.ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

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

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            var camera = Camera.main;
            
            
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


                if (camera != null)
                {
                    camera.transform.position = (Vector3)localTransform.Position + new Vector3(0, 8, -15);
                    camera.transform.LookAt(localTransform.Position);
                }


                // handle spawning of particles:
                if (Input.GetKey(KeyCode.Space))
                {
                    var spawnPosition = localTransform.Position + Utility.Forward * 16 + Utility.Up * 3;
                    var randomness = Random.CreateFromIndex((uint)(localTransform.Position.z * 1000)).NextFloat3Direction() * 3.25f;
                    
                    var particle = ecb.Instantiate(gameManager.ParticlePrefab);
                    ecb.SetLocalPositionRotation(particle, spawnPosition + randomness, quaternion.identity);
                }
                
            }).WithoutBurst().Run();
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}