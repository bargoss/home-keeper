using System;
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
    public struct ItemSocket : IComponentData
    {
        public ItemType AcceptedItemType;
        public bool DestroyedIfEmpty;
        
        // if it has an item, there will be an Item component on the entity as well as this
    }
    
    public struct Item : IComponentData
    {
        public ItemType ItemType;
        public int ItemId;
    }
    
    public readonly partial struct ItemSocketAspect : IAspect
    {
        // An Entity field in an Aspect gives access to the Entity itself.
        // This is required for registering commands in an EntityCommandBuffer for example.
        public readonly Entity Self;
        public readonly RefRO<ItemSocket> ItemSocket;
        [Optional] private readonly RefRO<Item> m_Item;
        private readonly RefRO<LocalToWorld> LocalToWorld;
        private readonly RefRW<LocalTransform> LocalTransform;
        public float3 WorldPosition => LocalToWorld.ValueRO.Position;

        public bool TryGetItem(out Item item)
        {
            item = default;
            
            if (m_Item.IsValid)
            {
                item = m_Item.ValueRO;
                return true;
            }

            return false;
        }
        
        
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
    
    public struct GameResourcesUnmanaged : IComponentData
    {
        public Entity ProjectilePrefab;
        public Entity EnemyPrefab;
        public Entity DyingEnemyPrefab;
        public Entity BloodEffectPrefab;
        public Entity FreeItemSocketPrefab;
        
        public GameResourcesUnmanaged(Entity projectilePrefab, Entity enemyPrefab, Entity dyingEnemyPrefab, Entity bloodEffectPrefab, Entity freeItemSocketPrefab)
        {
            ProjectilePrefab = projectilePrefab;
            EnemyPrefab = enemyPrefab;
            DyingEnemyPrefab = dyingEnemyPrefab;
            BloodEffectPrefab = bloodEffectPrefab;
            FreeItemSocketPrefab = freeItemSocketPrefab;
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
        public Entity ItemEntityOpt;
        public float GrabDistance;
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