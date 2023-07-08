using HomeKeeper.Components;
using Unity.Entities;
using UnityEngine;

namespace HomeKeeper.Authoring
{
    public class EnemyAuthoring : MonoBehaviour
    {
        
    }
    
    //public class GameResourcesBaker : Baker<GameResourcesAuthoring>
    public class EnemyBaker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<Enemy>(entity);
        }
    }
}