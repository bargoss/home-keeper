using Unity.Entities;

namespace Components
{
    public struct CharacterStats : IComponentData
    {
        public float MovementSpeed;
        public float AttackDamage;
        public float AttackCooldown;
        public float AttackRange;
        public float SelfStunDurationOnAttack;
        public float EnemyStunDurationOnAttack;
    }
}