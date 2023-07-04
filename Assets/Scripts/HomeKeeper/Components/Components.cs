using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

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
    }

    public struct Shooter : IComponentData
    {
        public ShooterStats Stats;
        
        // state
        public float LastShotTime;
        public bool ShotThisFrame;
        //public Entity AttachedMagazine;
        
        // input
        public bool ShootInput;
        public float3 LookInput;
        
        public struct ShooterStats
        {
            public float FireRate;
            public float MuzzleVelocity;
            public float AccuracyAngles;
            public Entity ShootPositionEntity;
        }
    }

    [Flags]
    public enum ItemType
    {
        Resource = 0,
        Magazine = 2,
        All = ~0
    }
    public struct Item : IComponentData
    {
        public ItemType ItemType;
    }
    
    public struct ItemSocket : IComponentData
    {
        // stats
        public ItemType AcceptedItemType;
        
        // states
        public Entity HeldItemOpt;
    }
    
    public static class CollisionTags
    {
        public const uint Default = 0;
        public const uint Item = 2;
        public const uint GrabObjectSocket = 4;
        public const uint Projectile = 8;
        public const uint Enemy = 16;
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

    public struct CharacterMovement : IComponentData
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
    
    public struct GameResources : IComponentData
    {
        public Entity ProjectilePrefab;
        public Entity EnemyPrefab;
        public Entity DyingEnemyPrefab;
        public Entity BloodEffectPrefab;
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
        public Entity ItemOpt;
        public float GrabDistance;
    }
    
    // events
    public struct EcsEvent : IComponentData { }
}