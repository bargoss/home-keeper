using HomeKeeper.Components;
using Unity.Entities;
using UnityEngine;

namespace HomeKeeper.Authoring
{
    public class MyTagAuthoring : MonoBehaviour
    {
        
    }

    public class MyTagBaker : Baker<MyTagAuthoring>
    {
        public override void Bake(MyTagAuthoring authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent<MyTag>(entity);
        }
    }
}