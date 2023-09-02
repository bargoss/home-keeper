using Unity.Entities;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Components
{
    public class DestroyableGhostAuthoring : MonoBehaviour
    {
        public class GhostDestroyedBaker : Baker<DestroyableGhostAuthoring>
        {
            public override void Bake(DestroyableGhostAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DestroyableGhost());
            }
        }
    }
}