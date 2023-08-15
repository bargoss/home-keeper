using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace DefenderGame.Scripts.Components
{
    public struct CharacterMovement : IComponentData
    {
        // stats
        [GhostField] public float MaxSpeed;
        [GhostField] public float MaxAcceleration;

        // state
        [GhostField] public NetworkTick LastJumpTime;
        [GhostField] public bool Jumped;
        [GhostField] public bool IsGrounded;
        
        // input
        [GhostField] public float2 MovementInput;
        [GhostField] public float3 LookInput;
        [GhostField] public bool JumpInput;
        
        public float3 MovementInputAsXZ => new float3(MovementInput.x, 0, MovementInput.y);
    }
}