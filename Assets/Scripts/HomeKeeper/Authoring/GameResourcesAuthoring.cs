using HomeKeeper.Components;
using Unity.Entities;
using UnityEngine;

namespace HomeKeeper.Authoring
{
    public class GameResourcesAuthoring : MonoBehaviour
    {
        // goes into managed
        public Mesh MagazineMesh;
        public Material MagazineMaterial;
        
        // goes into unmanaged
        public GameObject ProjectilePrefab;
        public GameObject EnemyPrefab;
        public GameObject DyingEnemyPrefab;
        public GameObject BloodEffectPrefab;
        public GameObject FreeItemSocketPrefab;
    }
    
    public class GameResourcesBaker : Baker<GameResourcesAuthoring>
    {
        public override void Bake(GameResourcesAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponentObject(entity,
                new GameResourcesManaged(
                    new GameResourcesManaged.Drawable(authoring.MagazineMesh, authoring.MagazineMaterial)
                )
            );
            AddComponent(entity, new GameResourcesUnmanaged(
                GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
                GetEntity(authoring.DyingEnemyPrefab, TransformUsageFlags.Dynamic),
                GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic),
                GetEntity(authoring.BloodEffectPrefab, TransformUsageFlags.Dynamic),
                GetEntity(authoring.FreeItemSocketPrefab, TransformUsageFlags.Dynamic)
            ));
        }
    }
}