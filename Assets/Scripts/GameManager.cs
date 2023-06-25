using System;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        private Entity m_Dome;
        private World m_World;
        private MyEntityPrefabsComponent m_Prefabs;

#if true
        // Q:   what is this called #if?
        //      i mean what are these # codes called?
        // A:   they are called preprocessor directives
        
#endif
        private void Fiddle()
        {
            var activeScene = SceneManager.GetActiveScene();
            var sceneSection = new SceneSection();
        }
        private void Start()
        {
            m_World = World.DefaultGameObjectInjectionWorld;
            
            
            //var activeScene = SceneManager.GetActiveScene();
            //SceneManager.LoadSceneAsync("Game").completed += operation =>
            //{
            //    if (operation.isDone)
            //    {
            //        var scene = SceneManager.GetSceneByName("Game");
            //        //var sceneEntity = scene.GetEntityScene();
            //    }
            //} 
            //
            //SceneManager.LoadSceneAsync("Basic").completed += operation =>
            //{
            //    
            //    var scene = SceneManager.GetSceneByName("Basic");
            //    
            //} 
            //var sceneSection = new SceneSection();
            m_Prefabs = World.DefaultGameObjectInjectionWorld.EntityManager
                .CreateEntityQuery(typeof(MyEntityPrefabsComponent))
                .GetSingleton<MyEntityPrefabsComponent>();
            
            m_Dome = CreateDome(float3.zero);

            for (var i = 0; i < 500; i++)
            {
                CreateFlyingEnemy(math.right() * 3 + math.up() * 12);
            }
            
            
            
            var playerLoop = UnityEngine.LowLevel.PlayerLoop.GetDefaultPlayerLoop();
            ScriptBehaviourUpdateOrder.RemoveWorldFromPlayerLoop(m_World, ref playerLoop);
            //m_World.QuitUpdate = true;
        }

        

        private void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                //m_World.GetExistingSystemManaged<SimulationSystemGroup>().Update();
                m_World.Update();
            }
        }

        private void Update()
        {
            var mousePosition = Utility.GetMousePositionInWorldSpace();
            var dome = GetEntityManager().GetComponentData<Dome>(m_Dome);
            var localTransform = GetEntityManager().GetComponentData<LocalTransform>(m_Dome);
            var aimDirection3 = mousePosition - localTransform.Position;
            var aimDirection = new float2(aimDirection3.x, aimDirection3.y);
            var aimDirectionNormalized = math.normalize(aimDirection);

            var shootInput = Input.GetMouseButton(0);
            
            dome.AimDirection = aimDirectionNormalized;
            dome.ShootInput = shootInput;
            GetEntityManager().SetComponentData(m_Dome, dome);
        }
        
        private MyEntityPrefabsComponent GetPrefabs()
        {
            return m_Prefabs;
            var query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(MyEntityPrefabsComponent));
            // get the first
            var myEntityPrefabsComponent = query.GetSingleton<MyEntityPrefabsComponent>();
            
            return myEntityPrefabsComponent;
        }

        private World GetWorld()
        {
            return m_World;
            //return World.DefaultGameObjectInjectionWorld;
        }
        private EntityManager GetEntityManager()
        {
            return GetWorld().EntityManager;
        }
        private Entity CreateDome(float3 position)
        {
            var prefab = GetPrefabs().E0;
            var playerEntity = GetEntityManager().Instantiate(prefab);
            GetEntityManager().SetComponentData(playerEntity, new LocalTransform() { Position = position, Rotation = Quaternion.identity, Scale = 1 });
            return playerEntity;
        }
        
        private Entity CreateFlyingEnemy(float3 position)
        {
            var prefab = GetPrefabs().E1;
            var entity = GetEntityManager().Instantiate(prefab);
            GetEntityManager().SetComponentData(entity, new LocalTransform() { Position = position, Rotation = Quaternion.identity, Scale = 1 });
            return entity;
        }
        
        private Entity CreateProjectile(float3 position)
        {
            var prefab = GetPrefabs().E2;
            var entity = GetEntityManager().Instantiate(prefab);
            GetEntityManager().SetComponentData(entity, new LocalTransform() { Position = position, Rotation = Quaternion.identity, Scale = 1 });
            return entity;
        }
    }
}