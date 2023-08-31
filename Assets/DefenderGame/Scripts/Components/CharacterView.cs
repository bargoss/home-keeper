using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace DefenderGame.Scripts.Components
{
    public struct CharacterView : IComponentData
    {
        public ViewId ViewId;
        
        [GhostField(Quantization = 10)] public float2 MovementVelocity;
        [GhostField(Quantization = 10)] public float3 LookDirection;
        [GhostField] public bool IsGrounded;
        [GhostField] public NetworkTick LastAttacked;
        [GhostField] public NetworkTick LastItemThrown;
        [GhostField] public bool Dead;
        
        
    }
    public struct ViewId
    {
        private int m_Value;
        
        public bool Assigned => m_Value != 0;
        
        //ctor
        public ViewId(int value)
        {
            m_Value = value;
        }
    }
}