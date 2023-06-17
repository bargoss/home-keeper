using Unity.Entities;

namespace Components
{
    public struct CharacterState : IComponentData
    {
        public float StunSecondsLeft;
        public float LastAttack;
    }
}