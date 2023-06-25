using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using Unity.Transforms;
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
        private int m_Frames = 0;
        
        public SubScene SubScene;
        private Entity m_SceneEntity;

        
        
        private World CreateWorld(string name)
        {
            var world = BaransWorldInitialization.Initialize(name);
            // get the SceneEntity of the subscene with name "New Scene"
            //Entity sceneEntity = world.EntityManager.CreateEntity();
            //world.EntityManager.AddComponentData(sceneEntity, new SceneReference {SceneGUID = new GUID("New Sub Scene")});
            
            m_SceneEntity = world.EntityManager.CreateEntity();
            world.EntityManager.AddComponentData(m_SceneEntity, new SceneReference {SceneGUID = SubScene.SceneGUID});
            //SceneSystem.LoadSceneAsync(world.Unmanaged,SubScene.SceneGUID, new SceneSystem.LoadParameters() { Flags = SceneLoadFlags.LoadAdditive });
            SceneSystem.LoadSceneAsync(world.Unmanaged, m_SceneEntity, new SceneSystem.LoadParameters() { Flags = SceneLoadFlags.LoadAdditive });
            return world;
        }


        private void Initialize()
        {
            m_World.GetExistingSystem<InitializationSystemGroup>();
            
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
            
            CreateTestEntity();
        }
        private void Start()
        {
            m_World = CreateWorld("barans world");
            
        }

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


        private void FixedUpdate()
        {
            var sceneLoaded = SceneSystem.IsSceneLoaded(m_World.Unmanaged, m_SceneEntity);
            Debug.Log("scene loaded: " + sceneLoaded);
            if (sceneLoaded)
            {
                if (m_Frames == 0)
                {
                    Initialize();
                }
                m_World.GetExistingSystem<SimulationSystemGroup>().Update(m_World.Unmanaged);

                m_Frames++;
            }
        }

        private void LateUpdate()
        {
            m_World.GetExistingSystem<PresentationSystemGroup>().Update(m_World.Unmanaged);
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