using DefaultNamespace;
using RunnerGame.Scripts.ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Stateful;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace RunnerGame.Scripts.ECS.Systems
{
    public partial struct GateSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var particleLookup = SystemAPI.GetComponentLookup<Particle>();
            var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
            var physicsVelocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>();
            var random = Random.CreateFromIndex((uint)(SystemAPI.Time.ElapsedTime * 3223.2323f));
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (statefulCollisionEvents, gate, localToWorld, gateEntity) in SystemAPI.Query<DynamicBuffer<StatefulCollisionEvent>, Gate, LocalToWorld>().WithEntityAccess())
            {
                foreach (var statefulCollisionEvent in statefulCollisionEvents)
                {
                    var otherEntity = statefulCollisionEvent.GetOtherEntity(gateEntity);
                    if (
                        particleLookup.TryGetRw(otherEntity, out var particleRw))
                    {
                        var otherPos = localToWorldLookup[otherEntity].Position;
                        var particle = particleRw.ValueRW;
                        particle.LastGateInteractionTime = (float)SystemAPI.Time.ElapsedTime;


                        switch (gate.GateType)
                        {
                            case GateType.Multiply:
                                for (int i = 0; i < (int)gate.Value; i++)
                                {
                                    var pos = otherPos + random.NextFloat3Direction();
                                    
                                    var clone = ecb.Instantiate(otherEntity);
                                    ecb.SetLocalPositionRotation(clone, pos, quaternion.identity);
                                }
                                
                                ecb.DestroyEntity(otherEntity);
                                break;
                            case GateType.Destroy:
                                ecb.DestroyEntity(otherEntity);
                                break;
                            case GateType.SpeedEffect:
                                if(physicsVelocityLookup.TryGetRw(otherEntity, out var otherPhysicsVelocityRw))
                                {
                                    var otherPhysicsVelocity = otherPhysicsVelocityRw.ValueRW;
                                    otherPhysicsVelocity.Linear += gate.Value * localToWorld.Forward;
                                    otherPhysicsVelocityRw.ValueRW = otherPhysicsVelocity;
                                }
                                break;
                            case GateType.Money:
                                Debug.Log("Money");
                                break;
                            default:
                                Debug.LogError($"Unknown gate type {gate.GateType}");
                                break;
                        }
                        
                        particleRw.ValueRW = particle;
                    }
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}