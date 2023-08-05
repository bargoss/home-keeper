using Unity.Entities;

namespace DefenderGame.Scripts.Components
{
    public struct CharacterMeleeCombat : IComponentData
    {
        // stats:
        public float AttackDuration;
        
        // state
        public float LastAttackTime;
        
        // input
        public bool AttackInput;
    }
}