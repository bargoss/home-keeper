using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        private Entity m_Dome;
        
        
        private void Start()
        {
            //m_World = DefaultWorldInitialization.Initialize("MyTestWorld", false);
            //var playerLoop = UnityEngine.LowLevel.PlayerLoop.GetDefaultPlayerLoop();
            //ScriptBehaviourUpdateOrder.RemoveWorldFromPlayerLoop(m_World, ref playerLoop);
            //var query = SystemAPI.QueryBuilder().WithAll<BakedEntity>().WithOptions(EntityQueryOptions.IncludePrefab).Build();

            m_Dome = CreateDome(float3.zero);

            for (var i = 0; i < 500; i++)
            {
                CreateFlyingEnemy(math.right() * 3 + math.up() * 12);
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
        private Entity CreateDome(float3 position)
        {
            var prefab = GetPrefabs().Dome;
            var playerEntity = GetEntityManager().Instantiate(prefab);
            GetEntityManager().SetComponentData(playerEntity, new LocalTransform() { Position = position, Rotation = Quaternion.identity, Scale = 1 });
            return playerEntity;
        }
        
        private Entity CreateFlyingEnemy(float3 position)
        {
            var prefab = GetPrefabs().FlyingEnemy;
            var entity = GetEntityManager().Instantiate(prefab);
            GetEntityManager().SetComponentData(entity, new LocalTransform() { Position = position, Rotation = Quaternion.identity, Scale = 1 });
            return entity;
        }
        
        private Entity CreateProjectile(float3 position)
        {
            var prefab = GetPrefabs().Projectile;
            var entity = GetEntityManager().Instantiate(prefab);
            GetEntityManager().SetComponentData(entity, new LocalTransform() { Position = position, Rotation = Quaternion.identity, Scale = 1 });
            return entity;
        }
    }
}