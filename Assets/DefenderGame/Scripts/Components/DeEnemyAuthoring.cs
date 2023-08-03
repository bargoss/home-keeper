using Unity.Entities;
using UnityEngine;

namespace DefenderGame.Scripts.Components
{
    public class DeEnemyAuthoring : MonoBehaviour
    {
        public float MovementSpeed = 1;
        public float AttackCoolDown = 1;
        public float AttackDamage = 1;

        public class DeEnemyBaker : Baker<DeEnemyAuthoring>
        {
            public override void Bake(DeEnemyAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new DeEnemy
                    {
                        MovementSpeed = authoring.MovementSpeed,
                        AttackCoolDown = authoring.AttackCoolDown,
                        AttackDamage = authoring.AttackDamage
                    });
            }
        }
    }
}