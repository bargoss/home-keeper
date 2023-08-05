using Unity.Entities;
using Unity.Mathematics;

namespace DefenderGame.Scripts.Components
{
    public struct CharacterMovement : IComponentData
    {
        // stats
        public float MaxSpeed;
        public float MaxAcceleration;

        // state
        public float LastJumpTime;
        public bool Jumped;
        
        // input
        public float3 MovementInput;
        public float3 LookInput;
        public bool JumpInput;
    }
}