using Unity.Entities;

namespace Components
{
    public struct Projectile : IComponentData
    {
        public float BaseDamage; // base damage before speed is taken into account
    }
}