using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct CharacterInput : IComponentData
    {
        public float3 Movement;
        public bool Attack;
    }
}