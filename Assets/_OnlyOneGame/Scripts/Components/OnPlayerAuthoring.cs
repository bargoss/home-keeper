using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Components
{
    public class OnPlayerAuthoring : MonoBehaviour
    {
        public class OnPlayerBaker : Baker<OnPlayerAuthoring>
        {
            public override void Bake(OnPlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new OnPlayer
                    {
                        ControllingCharacter = false,
                    });
            }
        }
    }
}