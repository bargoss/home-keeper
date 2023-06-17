using Components;
using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace.Authoring
{
    public class CharacterAuthoring : MonoBehaviour
    {
        public float MovementSpeed = 1;
        public float AttackDamage = 1;
        public float AttackCooldown = 1;
        public float AttackRange = 1;
        public float SelfStunDurationOnAttack = 1;
        public float EnemyStunDurationOnAttack = 1;
        
        public int Faction = 0;
    }
    
    public class CharacterBaker : Baker<CharacterAuthoring>
    {
        public override void Bake(CharacterAuthoring authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent(entity, new CharacterStats()
            {
                MovementSpeed = authoring.MovementSpeed,
                AttackDamage = authoring.AttackDamage,
                AttackCooldown = authoring.AttackCooldown,
                AttackRange = authoring.AttackRange,
                SelfStunDurationOnAttack = authoring.SelfStunDurationOnAttack,
                EnemyStunDurationOnAttack = authoring.EnemyStunDurationOnAttack
            });
            AddComponent<CharacterState>(entity);
            AddComponent<CharacterInput>(entity);
            AddComponent(entity, new Faction() { Value = authoring.Faction });
        }
    }
}