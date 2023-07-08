using Unity.Entities;

namespace Components
{
    public struct DamageOnCollision : IComponentData
    {
        public float FlatDamage;
        public float KineticDamage;
        public float KnockBackForce;
    }
}