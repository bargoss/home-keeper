using Unity.Entities;

namespace Components
{
    public struct EnemyState : IComponentData
    {
        public float HitPoints;
        public bool IsDead => HitPoints <= 0;
    }
}