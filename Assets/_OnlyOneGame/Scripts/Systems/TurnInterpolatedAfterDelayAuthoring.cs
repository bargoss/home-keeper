using Unity.Entities;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Systems
{
    public class TurnInterpolatedAfterDelayAuthoring : MonoBehaviour
    {
        public class TurnInterpolatedAfterDelayBaker : Baker<TurnInterpolatedAfterDelayAuthoring>
        {
            public override void Bake(TurnInterpolatedAfterDelayAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TurnInterpolatedAfterDelay());
            }
        }
    }
}