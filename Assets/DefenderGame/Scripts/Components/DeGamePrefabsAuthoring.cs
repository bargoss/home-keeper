using Unity.Entities;
using UnityEngine;

namespace DefenderGame.Scripts.Components
{
    public class DeGamePrefabsAuthoring : MonoBehaviour
    {
        public GameObject ProjectilePrefab;
        public GameObject Enemy0Prefab;

        public class DeGamePrefabsBaker : Baker<DeGamePrefabsAuthoring>
        {
            public override void Bake(DeGamePrefabsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new DeGamePrefabs
                    {
                        ProjectilePrefab = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic),
                        Enemy0Prefab = GetEntity(authoring.Enemy0Prefab, TransformUsageFlags.Dynamic)
                    });
            }
        }
    }
}