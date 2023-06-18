using Unity.Entities;

namespace Components
{
    public struct LifeSpan : IComponentData
    {
        public float SecondsToLive;
    }
}