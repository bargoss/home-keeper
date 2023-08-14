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
        [GhostField] public float3 MovementInput;
        [GhostField] public float3 LookInput;
        [GhostField] public bool JumpInput;
    }
}