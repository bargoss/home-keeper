using Unity.Entities;

namespace Components
{
    public struct Health : IComponentData
    {
        public float HitPoints;
        public float MaxHitPoints;
        public float RegenerationRate;
        public bool IsDead => HitPoints <= 0;
    }
}