using Unity.Entities;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Components
{
    public class GhostIdToEntityMapAuthoring : MonoBehaviour
    {
        public class GhostIdToEntityMapBaker : Baker<GhostIdToEntityMapAuthoring>
        {
            public override void Bake(GhostIdToEntityMapAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponentObject(entity, new SyncedIdToEntityMap());
            }
        }
    }
}