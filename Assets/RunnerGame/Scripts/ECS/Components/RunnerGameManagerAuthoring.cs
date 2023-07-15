using Unity.Entities;
using UnityEngine;

namespace RunnerGame.Scripts.ECS.Components
{
    public class RunnerGameManagerAuthoring : MonoBehaviour
    {
        class Baker : Baker<RunnerGameManagerAuthoring>
        {
            public override void Bake(RunnerGameManagerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RunnerGameManager());
            }
        }
    }
    
    public struct RunnerGameManager : IComponentData
    {
        
    }
}