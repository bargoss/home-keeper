using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace RunnerGame.Scripts.ECS.Components
{
    public class ParticleAcceleratorAuthoring : MonoBehaviour
    {
        public Vector3 Acceleration = new Vector3(0, 1, 0.05f);

        public class ParticleAcceleratorAuthoringBaker : Baker<ParticleAcceleratorAuthoring>
        {
            public override void Bake(ParticleAcceleratorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ParticleAccelerator { Acceleration = authoring.Acceleration });
            }
        }
    }

    public struct ParticleAccelerator : IComponentData
    {
        public float3 Acceleration;
    }
}