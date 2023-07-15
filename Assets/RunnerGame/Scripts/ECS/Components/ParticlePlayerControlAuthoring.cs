using Unity.Entities;
using UnityEngine;

namespace RunnerGame.Scripts.ECS.Components
{
    public class ParticlePlayerControlAuthoring : MonoBehaviour
    {
        public class ParticlePlayerControlBaker : Baker<ParticlePlayerControlAuthoring>
        {
            public override void Bake(ParticlePlayerControlAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ParticlePlayerControl());
            }
        }
    }
}