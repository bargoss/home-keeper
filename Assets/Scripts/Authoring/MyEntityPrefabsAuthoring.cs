using System.Collections.Generic;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public class MyEntityPrefabsAuthoring : MonoBehaviour
{
    public GameObject E0;
    public GameObject E1;
    public GameObject E2;
    public GameObject E3;
    public GameObject E4;
    public GameObject E5;
    public GameObject E6;
    public GameObject E7;
}

public class MyEntityPrefabsBaker : Baker<MyEntityPrefabsAuthoring>
{
    public override void Bake(MyEntityPrefabsAuthoring authoring)
    {
        var prefabsEntity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
        
        AddComponent(prefabsEntity,
            new MyEntityPrefabsComponent()
            {
                E0 = GetEntity(authoring.E0, TransformUsageFlags.Dynamic),
                E1 = GetEntity(authoring.E1, TransformUsageFlags.Dynamic),
                E2 = GetEntity(authoring.E2, TransformUsageFlags.Dynamic),
                E3 = GetEntity(authoring.E3, TransformUsageFlags.Dynamic),
                E4 = GetEntity(authoring.E4, TransformUsageFlags.Dynamic),
                E5 = GetEntity(authoring.E5, TransformUsageFlags.Dynamic),
                E6 = GetEntity(authoring.E6, TransformUsageFlags.Dynamic),
                E7 = GetEntity(authoring.E7, TransformUsageFlags.Dynamic),
            }
        );
    }
}