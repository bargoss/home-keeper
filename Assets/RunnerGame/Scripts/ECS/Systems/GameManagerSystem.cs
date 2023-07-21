﻿using DefaultNamespace;
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
        private int m_SpawnedCount = 0;
        private float3 m_SmoothDirection = new float3(0, 0, 1);
        protected override void OnUpdate()
        {
            if (!SystemAPI.TryGetSingleton<RgGameManagerData>(out var gameManager))
            {
                //Debug.LogError("GameManagerSystem: GameManagerData not found!");
                return;
            }

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            var camera = Camera.main;
            
            
            var playerHorizontalInput = Input.GetAxis("Horizontal");
            
            // move player and camera
            Entities.ForEach((Entity entity, ref PhysicsVelocity physicsVelocity, ref LocalTransform localTransform, in RgPlayer rgPlayer) =>
            {
                localTransform = HandlePlayerMovement(ref physicsVelocity, gameManager, playerHorizontalInput, ref localTransform, World.Time.DeltaTime, ref m_SmoothDirection);
                


                if (camera != null)
                {
                    camera.transform.position = (Vector3)localTransform.Position + new Vector3(0, 20, -10) * 1f;
                    camera.transform.LookAt((Vector3)localTransform.Position + Vector3.forward * 1);
                }


                var random = Random.CreateFromIndex((uint)(SystemAPI.Time.ElapsedTime * 321321));
                // handle spawning of particles:
                if (Input.GetKey(KeyCode.Space) || m_SpawnedCount < 1000 && random.NextFloat() % 1f >0.0f) // 2000
                {
                    var spawnPosition = localTransform.Position.z * new float3(0,0,1) + Utility.Forward * 16 + Utility.Up * 3 + new float3(4,0,0) * math.sin((float)SystemAPI.Time.ElapsedTime * 0.3f);
                    var randomness = Random.CreateFromIndex((uint)(localTransform.Position.z * 1000))
                        .NextFloat3Direction() * 0.0001f; //* 3.25f;

                    var particle = ecb.Instantiate(gameManager.ParticlePrefab);
                    ecb.SetLocalPositionRotationScale(particle, spawnPosition + randomness, quaternion.identity, 0.45f);

                    m_SpawnedCount++;
                }
                
            }).WithoutBurst().Run();
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
        
        private static LocalTransform HandlePlayerMovement(ref PhysicsVelocity physicsVelocity, RgGameManagerData gameManagerData,
            float playerHorizontalInput, ref LocalTransform localTransform, float deltaTime, ref float3 smoothDirection)
        {
            var velocity = physicsVelocity.Linear;
            var angularVelocity = physicsVelocity.Angular;

            var targetVelocity = Utility.Forward * gameManagerData.PlayerForwardSpeed +
                                 Utility.Right * playerHorizontalInput * gameManagerData.PlayerSidewaysP;
            velocity = math.lerp(velocity, targetVelocity, deltaTime * 2);
            velocity.y = 0;

            // remain inside the road bounds
            var pos = localTransform.Position;
            pos.y = 0;
            if (pos.x > gameManagerData.RoadWidth * 0.5f)
            {
                pos.x = gameManagerData.RoadWidth * 0.5f;
                if (velocity.x > 0)
                {
                    velocity.x = 0;
                }
            }
            else if (pos.x < -gameManagerData.RoadWidth * 0.5f)
            {
                pos.x = -gameManagerData.RoadWidth * 0.5f;
                if (velocity.x < 0)
                {
                    velocity.x = 0;
                }
            }

            localTransform.Position = pos;

            angularVelocity = 0;

            physicsVelocity.Linear = velocity;
            physicsVelocity.Angular = angularVelocity;
            
            //var targetRotation = quaternion.LookRotation( math.normalizesafe(velocity, new float3(0,0,1)), Utility.Up);
            //var lerpedRotation = math.slerp(localTransform.Rotation, targetRotation, deltaTime * 5);
            //localTransform.Rotation = lerpedRotation;
            
            localTransform.Rotation = quaternion.identity;;
            return localTransform;
        }
    }
}