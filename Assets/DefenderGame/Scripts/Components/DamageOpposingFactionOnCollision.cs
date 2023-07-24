using Unity.Entities;
using Unity.Mathematics;

namespace DefenderGame.Scripts.Components
{
    public struct DamageOpposingFactionOnCollision : IComponentData
    {
        public float DamagePerSecond;
    }
}
