using Components;
using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace.Authoring
{
    public class FlyingMeleeEnemyAuthoring : MonoBehaviour
    {
        public float Acceleration = 1;
        public float MaxSpeed = 5;
        public float EvasiveManeuvering = 1;
        public float AttackCooldown = 1;
        public float AttackDamage = 1;
        public float AttackedStunDuration = 1;
        public float TookDamageStunDuration = 1;
        public float DesiredMinDistanceToDome = 5;
        public float DesiredMaxDistanceToDome = 10;
    }
    
    public class FlyingMeleeEnemyBaker : Baker<FlyingMeleeEnemyAuthoring>
    {
        public override void Bake(FlyingMeleeEnemyAuthoring authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent(entity, new FlyingMeleeEnemy()
            {
                Acceleration = authoring.Acceleration,
                MaxSpeed = authoring.MaxSpeed,
                EvasiveManeuvering = authoring.EvasiveManeuvering,
                AttackCooldown = authoring.AttackCooldown,
                AttackDamage = authoring.AttackDamage,
                AttackedStunDuration = authoring.AttackedStunDuration,
                TookDamageStunDuration = authoring.TookDamageStunDuration,
                DesiredMinDistanceToDome = authoring.DesiredMinDistanceToDome,
                DesiredMaxDistanceToDome = authoring.DesiredMaxDistanceToDome,
            });
        }
    }
}