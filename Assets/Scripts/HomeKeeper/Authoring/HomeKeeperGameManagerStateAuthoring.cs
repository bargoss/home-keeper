using HomeKeeper.Components;
using Unity.Entities;
using UnityEngine;

namespace HomeKeeper.Authoring
{
    public class HomeKeeperGameManagerStateAuthoring : MonoBehaviour
    {
        public float EnemySpawnInterval = 5;
    }
    
    public class HomeKeeperGameManagerStateBaker : Baker<HomeKeeperGameManagerStateAuthoring>
    {
        public override void Bake(HomeKeeperGameManagerStateAuthoring stateAuthoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            var data = new GameManagerState(
                0,
                stateAuthoring.EnemySpawnInterval
            );
            AddComponent(entity, data);
        }
    }
}