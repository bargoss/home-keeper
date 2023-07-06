using HomeKeeper.Components;
using Unity.Entities;
using Unity.Entities.Content;
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

            var gameResourcesManaged = new GameResourcesManaged
            {
                Magazine = new GameResourcesManaged.Drawable
                {
                    Material = new WeakObjectReference<Material>(authoring.MagazineMaterial),
                    Mesh = new WeakObjectReference<Mesh>(authoring.MagazineMesh)
                }
            };
            AddComponentObject(entity, gameResourcesManaged);
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