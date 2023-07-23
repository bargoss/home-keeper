using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using WaterGame.Components;

namespace RunnerGame.Scripts.ECS.Components
{
    public class ParticleAuthoring : MonoBehaviour
    {
        class ParticleBaker : Baker<ParticleAuthoring>
        {
            public override void Bake(ParticleAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<SpacialPartitioningEntry>(entity);
                AddComponent<Particle>(entity);
                //AddComponent<ParticleView>(entity);
            }
        }
    }

    public struct Particle : IComponentData
    {
        public float LastGateInteractionTime;
    }

    public struct ParticleView : IComponentData
    {
        public bool PositiveEffect;
        public float3 Normal;
    }
}