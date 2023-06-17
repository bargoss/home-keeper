using Components;
using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace.Authoring
{
    public class PlayerControlledAuthoring : MonoBehaviour { }
    public class PlayerControlledBaker : Baker<PlayerControlledAuthoring>
    {
        public override void Bake(PlayerControlledAuthoring authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerControlled());
        }
    }
}