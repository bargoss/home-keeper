using Unity.Entities;

namespace Components
{
    public struct FlyingMeleeEnemy : IComponentData
    {
        // stats
        public float Acceleration;
        public float MaxSpeed;
        public float EvasiveManeuvering;
        public float AttackCooldown;
        public float AttackDamage;
        public float AttackedStunDuration;
        public float TookDamageStunDuration;
        public float DesiredMinDistanceToDome;
        public float DesiredMaxDistanceToDome;
        
        // state
        public float LastAttackTime;
        public float LastTookDamageTime;
    }
}