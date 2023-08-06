using Unity.Entities;
using UnityEngine;

namespace DefenderGame.Scripts.Components
{
    public class CharacterMeleeCombatAuthoring : MonoBehaviour
    {
        public float AttackCooldown;
        public float AttackRange;
        public float AttackDamage;

        public class CharacterMeleeCombatBaker : Baker<CharacterMeleeCombatAuthoring>
        {
            public override void Bake(CharacterMeleeCombatAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new CharacterMeleeCombat
                    {
                        AttackCooldown = authoring.AttackCooldown,
                        AttackRange = authoring.AttackRange,
                        AttackDamage = authoring.AttackDamage
                    });
            }
        }
    }
}