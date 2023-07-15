using Unity.Entities;
using UnityEngine;

namespace SwarmRunner.Components
{
    public class GameManagerSwarmRunnerStateAuthoring : MonoBehaviour
    {
        class Baker : Baker<GameManagerSwarmRunnerStateAuthoring>
        {
            public override void Bake(GameManagerSwarmRunnerStateAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new GameManagerSwarmRunnerState());
            }
        }
    }
    
    public struct GameManagerSwarmRunnerState : IComponentData
    {
        
    }
}