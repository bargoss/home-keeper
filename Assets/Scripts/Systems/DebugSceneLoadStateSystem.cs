using DefaultNamespace;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems
{
    
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct DebugSceneLoadStateSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var sceneLoadStatus = SceneSystem.GetSceneStreamingState(state.WorldUnmanaged, GameManager2.SubSceneEntity);
            Debug.Log("scene loading status in system: " + sceneLoadStatus);
        }
    }
}