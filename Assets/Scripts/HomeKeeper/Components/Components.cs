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
        
        // input
        public bool ShootInput;
        public float3 LookInput;
    }

    public struct ShooterStats
    {
        public float FireRate;
        public float MuzzleVelocity;
        public float AccuracyAngles;
        public Entity ShootPositionEntity;
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
}