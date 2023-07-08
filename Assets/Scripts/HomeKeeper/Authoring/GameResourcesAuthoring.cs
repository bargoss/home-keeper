using HomeKeeper.Components;
using Unity.Entities;
using Unity.Entities.Content;
using UnityEngine;

namespace HomeKeeper.Authoring
{
    public class GameResourcesAuthoring : MonoBehaviour
    {
        // goes into unmanaged
        public GameObject ProjectilePrefab;
        public GameObject EnemyPrefab;
        public GameObject DyingEnemyPrefab;
        public GameObject FreeItemSocketPrefab;
        public GameObject ShooterPrefab;
    }
    
    public class GameResourcesBaker : Baker<GameResourcesAuthoring>
    {
        public override void Bake(GameResourcesAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            
            AddComponent(entity, new GameResourcesUnmanaged(
                GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic),
                GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
                GetEntity(authoring.DyingEnemyPrefab, TransformUsageFlags.Dynamic),
                GetEntity(authoring.FreeItemSocketPrefab, TransformUsageFlags.Dynamic),
                GetEntity(authoring.ShooterPrefab, TransformUsageFlags.Dynamic)
            ));
        }
    }
}