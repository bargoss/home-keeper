using Components;
using HomeKeeper.Components;
using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace.Authoring
{
    public class ProjectileAuthoring : MonoBehaviour
    {
        //public float BaseDamage = 1;
    }
    
    public class ProjectileBaker : Baker<ProjectileAuthoring>
    {
        public override void Bake(ProjectileAuthoring authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent(entity, new Projectile()
            {
                Penetration = 0,
                BaseDamage = 1
            });
        }
    }
}