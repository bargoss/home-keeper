using System;
using System.Collections.Generic;
using SpacialIndexing;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Content;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace HomeKeeper.Components
{
    public struct Health : IComponentData
    {
        public float HitPoints;
        public float MaxHitPoints;
        public bool DestroyOnDeath;
        public bool IsDead => HitPoints <= 0;
    }
    public struct LifeSpan : IComponentData
    {
        public float SecondsToLive;
    }
    public struct Projectile : IComponentData
    {
        public float BaseDamage;
        public float Penetration;
    }

    public struct FlakDetonation : IComponentData
    {
        // stats
        public float ShardBaseDamage;
        public float ShardPenetration;
        public int ShardCount;
        public float DetonationRange;
        
        // state
        public float TravelledDistance;
    }
    
    public struct DamageRangeCurve : IComponentData
    {
        public float RangeA;
        public float RangeB;
        public float DamageA;
        public float DamageB;

        public float GetDamage(float range)
        {
            var t = math.unlerp(RangeA, RangeB, range);
            
            if (t <= 0) return DamageA;
            if (t >= 1) return DamageB;
            return math.lerp(DamageA, DamageB, t);
        }
    }
    public struct ExplosiveContact : IComponentData
    {
        public DamageRangeCurve DamageRangeCurve;
    }
    
    public struct RigidbodyAxisLock : IComponentData
    {
        public bool LockLinearX;
        public bool LockLinearY;
        public bool LockLinearZ;
        
        public bool LockRotation;
    }

    public struct Shooter : IComponentData
    {
        public ShooterStats Stats;
        
        // state
        public float LastShotTime;
        public bool ShotThisFrame;
        //public Entity AttachedMagazine;
        public float3 Look;
        
        // input
        public bool ShootInput;
        public float3 LookInput;
        
        public struct ShooterStats
        {
            public float FireRate;
            public float MuzzleVelocity;
            public float AccuracyAngles;
            //public Entity ShootPositionEntity;
        }
    }

    // todo, need to use cleanupdata or something
    public struct ShooterView : IComponentData //ICleanupComponentData
    {
        
    }
    
    public struct Magazine : IComponentData
    {
        // Stats
        public int Capacity;
        public Entity ProjectilePrefab;
        
        // State
        public int Current;
    }

    public struct Enemy : IComponentData
    {
        
    }

    public struct CharacterMovement2 : IComponentData
    {
        public struct SStats
        {
            public float MaxSpeed;
            public float AccelerationMultiplier;
            public float MaxAcceleration;
        }

        public SStats Stats;
        public float3 DirectionInput;
    }

    public struct DyingEnemy : IComponentData
    {
        public FixedList128Bytes<Entity> BodyParts;
    }

    // consumed by a gameobject view system
    public struct BloodEffect : IComponentData
    {
        
    }
    
    public struct GameResourcesUnmanaged : IComponentData
    {
        public Entity ProjectilePrefab;
        public Entity EnemyPrefab;
        public Entity DyingEnemyPrefab;
        public Entity FreeItemSocketPrefab;
        public Entity ShooterPrefab;
        
        public GameResourcesUnmanaged(Entity projectilePrefab, Entity enemyPrefab, Entity dyingEnemyPrefab, Entity freeItemSocketPrefab, Entity shooterPrefab)
        {
            ProjectilePrefab = projectilePrefab;
            EnemyPrefab = enemyPrefab;
            DyingEnemyPrefab = dyingEnemyPrefab;
            FreeItemSocketPrefab = freeItemSocketPrefab;
            ShooterPrefab = shooterPrefab;
        }
    }

    
    public struct EnemySpawner : IComponentData
    {
        public float SpawnInterval;
        public float LastSpawnTime;
        public float SpawnInnerRadius;
        public float SpawnOuterRadius;
        public float3 SpawnDirection;
        public float SpawnArcDegrees;
    }
    
    public struct PlayerAction : IComponentData
    {
        // input
        public float3 CameraPosition;
        public float3 CameraForward;
        public float3 MouseDirection;
        public bool Grab;
        public bool Drop;
        
        // state
        //public Entity ItemEntityOpt;
        public bool HoldsItem;
        
        public float GrabDistance;
    }
    
    public struct GameManagerState : IComponentData
    {
        public float LastEnemySpawnTime;
        public float EnemySpawnInterval;
        
        
        public GameManagerState(float lastEnemySpawnTime, float enemySpawnInterval)
        {
            LastEnemySpawnTime = lastEnemySpawnTime;
            EnemySpawnInterval = enemySpawnInterval;
        }
    }
    
    public struct ChildForOneFrame : IComponentData
    {
        public Entity Parent;
        public float4x4 LocalTransform;
    }
    
    public struct MyTag : IComponentData
    {
        
    }
    /*
     * Platform will be an entity, it will have health
     * There will be no other entity
     * 
     */
    public class Platform : IComponentData
    {
        public List<int> Slots;
    }

    public abstract class PlatformObject
    {
        
    }
    
    
    public static class CollisionTags
    {
        public const uint None = 0;
        public const uint Default = 2;
        public const uint ItemSocket = 4;
        public const uint Projectile = 8;
        public const uint Enemy = 16;
        
        public const uint All = 0xffffffff;
    }
    
    
    [Flags]
    public enum ItemType
    {
        Resource = 1,
        Magazine = 2,
        All = ~0
    }
    
    // events
    public struct EcsEvent : IComponentData { }
}