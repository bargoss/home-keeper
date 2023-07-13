using Unity.Entities;
using UnityEngine;
using WaterGame.Components;

namespace WaterGame.Authoring
{
    public class ParticleAuthoring : MonoBehaviour {}
    
    public class ParticleBaker : Baker<ParticleAuthoring>
    {
        public override void Bake(ParticleAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<SpacialPartitioningEntry>(entity);
            AddComponent<Particle>(entity);
        }
    }
}