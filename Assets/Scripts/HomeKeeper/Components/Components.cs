using Unity.Entities;
using Unity.Mathematics;

namespace HomeKeeper.Components
{
    public struct MeleeAttackStats : IComponentData
    {
        public float AttackCooldown;
    }
    public struct MeleeAttacksState : IComponentData
    {
        public float LastAttack;
    }
    public struct MovementInput : IComponentData
    {
        public float2 Value;
    }
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
    public struct DamageOthersOnCollision : IComponentData
    {
        public float Damage;
        public float CollisionDamage;
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
    
}