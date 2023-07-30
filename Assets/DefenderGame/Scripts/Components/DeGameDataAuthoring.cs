using Unity.Entities;
using UnityEngine;

namespace DefenderGame.Scripts.Components
{
    public class DeGameDataAuthoring : MonoBehaviour
    {
        public class DeGameDataBaker : Baker<DeGameDataAuthoring>
        {
            public override void Bake(DeGameDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DeGameData());
            }
        }
    }
}