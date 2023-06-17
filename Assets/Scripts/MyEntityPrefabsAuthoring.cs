using Unity.Entities;
using UnityEngine;

public class MyEntityPrefabsAuthoring : MonoBehaviour
{
    public GameObject Player;
    public GameObject Enemy;
}

public struct MyEntityPrefabsComponent : IComponentData
{
    public Entity Player;
    public Entity Enemy;
}

public class MyEntityPrefabsBaker : Baker<MyEntityPrefabsAuthoring>
{
    public override void Bake(MyEntityPrefabsAuthoring authoring)
    {
        /*
        // Register the Prefab in the Baker
        var entityPrefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic);
        // Add the Entity reference to a component for instantiation later
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new EntityPrefabComponent() {Value = entityPrefab});
        */
        
        // Register the Prefab in the Baker
        var playerEntityPrefab = GetEntity(authoring.Player, TransformUsageFlags.Dynamic);
        var enemyEntityPrefab = GetEntity(authoring.Enemy, TransformUsageFlags.Dynamic);
        
        var prefabsEntity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        
        AddComponent(prefabsEntity, new MyEntityPrefabsComponent() {Player = playerEntityPrefab, Enemy = enemyEntityPrefab});
    }
}