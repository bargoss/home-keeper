using Unity.Entities;
using UnityEngine;

namespace Components
{
    public class FactionAuthoring : MonoBehaviour
    {
        public int Value;

        public class FactionBaker : Baker<FactionAuthoring>
        {
            public override void Bake(FactionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Faction { Value = authoring.Value });
            }
        }
    }
}