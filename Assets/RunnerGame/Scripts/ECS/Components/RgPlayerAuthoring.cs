using Unity.Entities;
using UnityEngine;

namespace RunnerGame.Scripts.ECS.Components
{
    public class RgPlayerAuthoring : MonoBehaviour
    {
        public class RgPlayerBaker : Baker<RgPlayerAuthoring>
        {
            public override void Bake(RgPlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RgPlayer());
            }
        }
    }
}