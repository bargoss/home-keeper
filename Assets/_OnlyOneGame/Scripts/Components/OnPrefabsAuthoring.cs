using Unity.Entities;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Components
{
    public class OnPrefabsAuthoring : MonoBehaviour
    {
        public GameObject GroundItemPrefab;
        public GameObject DeployingItemPrefab;
        public GameObject SimplePlayerPrefab;
        public GameObject PlayerPrefab;
        public GameObject PlayerCharacterPrefab;

        public class OnPrefabsBaker : Baker<OnPrefabsAuthoring>
        {
            public override void Bake(OnPrefabsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new OnPrefabs
                    {
                        GroundItemPrefab = new EntWithComponent<GroundItem>(GetEntity(authoring.GroundItemPrefab,
                            TransformUsageFlags.Dynamic)),
                        DeployingItemPrefab =
                            new EntWithComponent<DeployingItem>(GetEntity(authoring.DeployingItemPrefab,
                                TransformUsageFlags.Dynamic)),
                        PlayerPrefab = GetEntity(authoring.PlayerPrefab, TransformUsageFlags.Dynamic),
                        PlayerCharacterPrefab = GetEntity(authoring.PlayerCharacterPrefab, TransformUsageFlags.Dynamic),
                    }
                );
            }
        }
    }
}