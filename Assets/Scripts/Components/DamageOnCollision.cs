using Unity.Entities;

namespace Components
{
    public struct DamageOnCollision : IComponentData
    {
        public float DamageRate;
        public float KnockbackForce;
    }
}