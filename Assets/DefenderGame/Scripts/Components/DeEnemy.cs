using Unity.Entities;

namespace DefenderGame.Scripts.Components
{
    public struct DeEnemy : IComponentData
    {
        // stats
        public float MovementSpeed;
        public float AttackCoolDown;
        public float AttackDamage;
        
        // state
        public float LastAttackTime;
        public float StunnedUntil;
    }
}