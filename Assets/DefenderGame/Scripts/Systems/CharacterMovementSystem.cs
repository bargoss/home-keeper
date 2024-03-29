﻿using DefaultNamespace;
using DefenderGame.Scripts.Components;
using HomeKeeper.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace DefenderGame.Scripts.Systems
{
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
    public partial struct CharacterMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<BuildPhysicsWorldData>();
            state.RequireForUpdate<CharacterMovement>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buildPhysicsWorld = SystemAPI.GetSingleton<BuildPhysicsWorldData>();
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            
            if(networkTime.IsPartialTick) return;
            
            foreach (var aspect in SystemAPI.Query<CharacterMovementAspect>().WithAll<Simulate>())
            {
                var grounded = buildPhysicsWorld.Raycast(
                    aspect.Position + Utility.Up * 0.05f,
                    aspect.Position - Utility.Up * 0.10f,
                    out _
                );

                aspect.Grounded = grounded; 

                if (grounded)
                {
                    var movementInput = aspect.CharacterMovement.ValueRO.MovementInputAsXZ.ClampMagnitude(1);
                    var maxMovementSpeed = aspect.CharacterMovement.ValueRO.MaxSpeed;
                    var maxAcceleration = aspect.CharacterMovement.ValueRO.MaxAcceleration;
                    var targetVelocity = movementInput * maxMovementSpeed;
                    
                    targetVelocity.y = aspect.Velocity.y;
                    
                    Utility.ControlVelocity(aspect.Velocity, targetVelocity, maxAcceleration, SystemAPI.Time.DeltaTime, out var newVelocity);
                    
                    aspect.Velocity = newVelocity;
                }
                
                aspect.AngularVelocity = float3.zero;
                
                 aspect.Rotation = Quaternion.identity;
                 //var targetRotation =
                 //    Quaternion.LookRotation(
                 //        math.normalize(aspect.CharacterMovement.ValueRO.MovementInputAsXZ +
                 //         aspect.LocalTransform.ValueRO.Forward() * 0.1f),
                 //        Utility.Up
                 //    );
                 //aspect.Rotation = Quaternion.Slerp(aspect.Rotation, targetRotation, 0.3f);

            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
    
    public readonly partial struct CharacterMovementAspect : IAspect
    {
        public readonly Entity Self;
        public readonly RefRW<CharacterMovement> CharacterMovement;
        public readonly RefRW<LocalTransform> LocalTransform;
        public readonly RefRW<PhysicsVelocity> PhysicsVelocity;
        [Optional] private readonly RefRO<Health> m_Health;

        public float3 Position => LocalTransform.ValueRO.Position;
        public quaternion Rotation {
            get => LocalTransform.ValueRO.Rotation;
            set => LocalTransform.ValueRW.Rotation = value;
        }

        public float3 Velocity
        {
            get => PhysicsVelocity.ValueRO.Linear;
            set => PhysicsVelocity.ValueRW.Linear = value;
        }
        
        public float3 AngularVelocity
        {
            get => PhysicsVelocity.ValueRO.Angular;
            set => PhysicsVelocity.ValueRW.Angular = value;
        }
        
        public bool TryGetHealth(out Health health)
        {
            health = default;
            if (m_Health.IsValid)
            {
                health = m_Health.ValueRO;
                return true;
            }

            return false;
        }
        
        public bool Grounded
        {
            get => CharacterMovement.ValueRO.IsGrounded;
            set => CharacterMovement.ValueRW.IsGrounded = value;
        }
        
        //private readonly RefRO<Health>


    }
}