using Unity.Entities;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Components
{
    public class GroundItemAuthoring : MonoBehaviour
    {
        public class GroundItemBaker : Baker<GroundItemAuthoring>
        {
            public override void Bake(GroundItemAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new GroundItem(new Item(ItemTypeResource.Metal)));
            }
        }
    }
}