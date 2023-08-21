using Unity.Entities;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Components
{
    public class SyncedIdAuthoring : MonoBehaviour
    {
        public class SyncedIdBaker : Baker<SyncedIdAuthoring>
        {
            public override void Bake(SyncedIdAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SyncedId());
            }
        }
    }
}