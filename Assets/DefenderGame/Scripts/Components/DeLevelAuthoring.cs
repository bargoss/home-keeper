using Unity.Entities;
using UnityEngine;

namespace DefenderGame.Scripts.Components
{
    public class DeLevelAuthoring : MonoBehaviour
    {
        public class LevelBaker : Baker<DeLevelAuthoring>
        {
            public override void Bake(DeLevelAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.WorldSpace);
                AddComponent(entity, new DeLevel());
            }
        }
    }

    public struct DeLevel : IComponentData
    {
    }
}