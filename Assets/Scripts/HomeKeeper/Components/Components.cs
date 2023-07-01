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
        public Entity ProjectilePrefab;
        public float MuzzleVelocity;
        public float AccuracyAngles;
        public Entity ShootPositionEntity;
    }

    public struct Enemy : IComponentData
    {

    }

    public struct DyingEnemy : IComponentData
    {
        public FixedList128Bytes<Entity> BodyParts;
    }
}