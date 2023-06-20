using Unity.Entities;
using UnityEngine;

namespace Common.Scripts
{
    public struct LifeTime : IComponentData
    {
        public int Value;
    }

    public class LifeTimeAuthoring : MonoBehaviour
    {
        [Tooltip("The number of frames until the entity should be destroyed.")]
        public int Value;
    }

    public class LifeTimeBaker : Baker<LifeTimeAuthoring>
    {
        public override void Bake(LifeTimeAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new LifeTime { Value = authoring.Value });
        }
    }
}
