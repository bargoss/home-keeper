using Unity.Entities;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Components
{
    public class LoseOwnerWithDelayAuthoring : MonoBehaviour
    {
        public int Ticks;

        public class LoseOwnerWithDelayBaker : Baker<LoseOwnerWithDelayAuthoring>
        {
            public override void Bake(LoseOwnerWithDelayAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new LoseOwnerWithDelay { Ticks = authoring.Ticks });
            }
        }
    }
}