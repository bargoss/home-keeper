using Unity.Entities;
using UnityEngine;

public class MyEntityPrefabsAuthoring : MonoBehaviour
{
    public GameObject Dome;
    public GameObject FlyingEnemy;
    public GameObject Projectile;
}

public struct MyEntityPrefabsComponent : IComponentData
{
    public Entity Dome;
    public Entity FlyingEnemy;
    public Entity Projectile;
}

public class MyEntityPrefabsBaker : Baker<MyEntityPrefabsAuthoring>
{
    public override void Bake(MyEntityPrefabsAuthoring authoring)
    {
        var playerEntityPrefab = GetEntity(authoring.Dome, TransformUsageFlags.Dynamic);
        var enemyEntityPrefab = GetEntity(authoring.FlyingEnemy, TransformUsageFlags.Dynamic);
        var projectileEntityPrefab = GetEntity(authoring.Projectile, TransformUsageFlags.Dynamic);
        
        var prefabsEntity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);

        AddComponent(prefabsEntity,
            new MyEntityPrefabsComponent()
            {
                Dome = playerEntityPrefab,
                FlyingEnemy = enemyEntityPrefab,
                Projectile = projectileEntityPrefab
            }
        );
    }
}