using System;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes.Editor;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        private Entity m_Player;
        
        
        public void Start()
        {
            //m_World = DefaultWorldInitialization.Initialize("MyTestWorld", false);
            //var playerLoop = UnityEngine.LowLevel.PlayerLoop.GetDefaultPlayerLoop();
            //ScriptBehaviourUpdateOrder.RemoveWorldFromPlayerLoop(m_World, ref playerLoop);
            //var query = SystemAPI.QueryBuilder().WithAll<BakedEntity>().WithOptions(EntityQueryOptions.IncludePrefab).Build();

            m_Player = CreatePlayer(-math.right() * 6);
            CreateEnemy(math.right() * 6 - math.up() * 3);
            CreateEnemy(math.right() * 6 + math.up() * 3);
        }

        private MyEntityPrefabsComponent GetPrefabs()
        {
            var query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(MyEntityPrefabsComponent));
            var myEntityPrefabsComponent = query.GetSingleton<MyEntityPrefabsComponent>();
            return myEntityPrefabsComponent;
        }

        private World GetWorld()
        {
            return World.DefaultGameObjectInjectionWorld;
        }
        private EntityManager GetEntityManager()
        {
            return GetWorld().EntityManager;
        }
        private Entity CreatePlayer(float3 position)
        {
            var prefab = GetPrefabs().Player;
            var playerEntity = GetEntityManager().Instantiate(prefab);
            GetEntityManager().SetComponentData(playerEntity, new LocalTransform() { Position = position, Rotation = Quaternion.identity, Scale = 1 });
            return playerEntity;
        }
        
        private Entity CreateEnemy(float3 position)
        {
            var prefab = GetPrefabs().Enemy;
            var entity = GetEntityManager().Instantiate(prefab);
            GetEntityManager().SetComponentData(entity, new LocalTransform() { Position = position, Rotation = Quaternion.identity, Scale = 1 });
            return entity;
        }


        public void Update()
        {
            GetEntityManager().SetComponentData(m_Player, new CharacterInput()
            {
                Attack = Input.GetKeyDown(KeyCode.Space),
                Movement = Utility.GetInputDirection()
            });
        }
    }
}