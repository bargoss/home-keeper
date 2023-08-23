using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace DefenderGame.Scripts.Components
{
    public struct CharacterView : IComponentData
    {
        public bool ViewIdAssigned { get; private set; }
        public CharacterViewId ViewId { get; private set; }
        
        [GhostField(Quantization = 10)] public float2 MovementVelocity;
        [GhostField(Quantization = 10)] public float3 LookDirection;
        [GhostField] public bool IsGrounded;
        [GhostField] public bool Attacked;
        [GhostField] public bool ItemThrown;
        [GhostField] public bool Dead;
        
        
        
        public void AssignViewId(CharacterViewId viewId)
        {
            ViewId = viewId;
            ViewIdAssigned = true;
        }
    }
    public struct CharacterViewId
    {
        public int Value;
        
        //ctor
        public CharacterViewId(int value)
        {
            Value = value;
        }
    }
}