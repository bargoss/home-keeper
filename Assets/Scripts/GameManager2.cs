using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Mathematics;
using Unity.Scenes;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class GameManager2 : MonoBehaviour
    {
        private World m_World;
        //private SystemHandle m_SimulationSystemGroup;
        //private SystemHandle m_PresentationSystemGroup;
        //private SystemHandle m_InitializationSystemGroup;
        public static Entity SubSceneEntity;

        //public SubScene SubScene;
        //private Entity m_SceneEntity;



        private World GetDefaultWorld()
        {
            return World.DefaultGameObjectInjectionWorld;
        }
        private World CreateWorld(string name)
        {
            var world = new World(name);
            
            // add default system groups
            var initializationSystemGroup = world.GetOrCreateSystem<InitializationSystemGroup>();
            world.GetOrCreateSystem<SimulationSystemGroup>();
            world.GetOrCreateSystem<PresentationSystemGroup>();

            var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systems);
            
            initializationSystemGroup.Update(world.Unmanaged);
            
            return world;
        }
        private void LoadScene(){
            const string subScenePath = "Assets/Scenes/SampleScene/New Sub Scene.unity";
            var guid = AssetDatabase.GUIDFromAssetPath(subScenePath);
            SubSceneEntity = SceneSystem.LoadSceneAsync(m_World.Unmanaged, guid,
                new SceneSystem.LoadParameters() { Flags = SceneLoadFlags.LoadAdditive });
        }


        private void Start()
        {
            m_World = CreateWorld("bargos world");
            //m_World = GetDefaultWorld();
            
            DebugSceneLoadStatus();
            
            LoadScene();
            
            //const string subScenePath = "Assets/Scenes/SampleScene/New Sub Scene.unity";
            //var guid = AssetDatabase.GUIDFromAssetPath(subScenePath);
            //SubSceneEntity = SceneSystem.LoadSceneAsync(m_World.Unmanaged, guid,
            //    new SceneSystem.LoadParameters() { Flags = SceneLoadFlags.LoadAdditive });
            
        }

          /*      
        private Entity CreateTestEntity()
        {
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            var entity = commandBuffer.CreateEntity();
            commandBuffer.AddComponent(entity, typeof(TestComponent));
            // add transform related components
            commandBuffer.AddComponent(entity, new LocalTransform()
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1,
            });
            commandBuffer.AddComponent(entity, new LocalToWorld()
            {
                Value = float4x4.identity,
            });

            commandBuffer.Playback(m_World.EntityManager);
            return entity;
        }
*/

        /*
        private void InitializeMyStuff()
        {
            var prefabsComponent = m_World.EntityManager
                .CreateEntityQuery(typeof(MyEntityPrefabsComponent))
                .GetSingleton<MyEntityPrefabsComponent>();
            
            var myPrefab = prefabsComponent.E0;
            
            // instantiate the prefab
            var prefab = m_World.EntityManager.Instantiate(myPrefab);
            // set its position
            m_World.EntityManager.SetComponentData(prefab, new LocalTransform()
            {
                Position = new float3(10, 0, 0),
                Rotation = quaternion.identity,
                Scale = 2,
            });
            
            //CreateTestEntity();
        }
        */
        
        private void FixedUpdate()
        {
            DebugSceneLoadStatus();


            if (!Input.GetKey(KeyCode.Space))
            {
                m_World.GetExistingSystem<InitializationSystemGroup>().Update(m_World.Unmanaged);
                m_World.GetExistingSystem<SimulationSystemGroup>().Update(m_World.Unmanaged);
            }
            
            var time = m_World.Time.ElapsedTime;
            Debug.Log("time: " + time);
        }

        private void LateUpdate()
        {
            m_World.GetExistingSystem<PresentationSystemGroup>().Update(m_World.Unmanaged);        
        }

        private void DebugSceneLoadStatus()
        {
            //var sceneLoadStatus = SceneSystem.GetSceneStreamingState(m_World.Unmanaged, SubSceneEntity);
            var sceneLoadStatus = SceneSystem.GetSceneStreamingState(m_World.Unmanaged, SubSceneEntity);
            Debug.Log("scene loading status: " + sceneLoadStatus);

            /*
            if (Input.GetKey(KeyCode.D))
            {
                var sceneLoadedBool = SceneSystem.IsSceneLoaded(World.DefaultGameObjectInjectionWorld.Unmanaged, SubSceneEntity);
                Debug.Log("scene loaded bool: " + sceneLoadedBool);
            }
            */
        }

        private void Noted()
        {
            // This query will return all baked entities, including the prefab entities
            //var prefabQuery = SystemAPI.QueryBuilder().WithAll<BakedEntity>().WithOptions(EntityQueryOptions.IncludePrefab).Build();
        }
        /*
        private void RegisterPrefabs()
        {
            var prefabsComponent = World.DefaultGameObjectInjectionWorld.EntityManager
                .CreateEntityQuery(typeof(MyEntityPrefabsComponent))
                .GetSingleton<MyEntityPrefabsComponent>();
            
            var prefabsEntity = m_World.EntityManager.CreateEntity();
            m_World.EntityManager.AddComponentData(prefabsEntity,
                
            );
        }
        */
    }
}