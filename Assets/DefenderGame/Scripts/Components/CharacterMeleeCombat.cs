using Unity.Entities;

namespace DefenderGame.Scripts.Components
{
    public struct CharacterMeleeCombat : IComponentData
    {
        // stats:
        public float AttackCooldown;
        public float AttackRange;
        public float AttackDamage;
        
        // state
        public float LastAttackTime;
        public bool Attacked;
        
        // input
        public bool AttackInput;
    }
}