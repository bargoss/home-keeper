using Unity.Entities;
using UnityEngine;

public class GameManager2 : MonoBehaviour
{
    private World m_World;
    private CoroutineRunner m_CoroutineRunner;
    [SerializeField] private string subScenePath = "Assets/Scenes/SampleScene/New Sub Scene.unity";

    private void Awake()
    {
        m_CoroutineRunner = gameObject.AddComponent<CoroutineRunner>();
    }

    private void Start()
    {
        //public static void Create(string worldName,string subScenePath,CoroutineRunner coroutineRunner,float timeoutSeconds,Action<Result<World>> onCreated)
        WorldCreator.Create("MyWorld", subScenePath, m_CoroutineRunner, 9999, result =>
        {
            switch (result)
            {
                case Result<World>.Success success:
                    m_World = success.Value;
                    Debug.Log("world loaded");
                    break;
                case Result<World>.Error error:
                    Debug.LogError(error.Message);
                    break;
            }
        });
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
        if (m_World != null)
        {
            m_World.GetExistingSystem<InitializationSystemGroup>().Update(m_World.Unmanaged);
            m_World.GetExistingSystem<SimulationSystemGroup>().Update(m_World.Unmanaged);
            // todo: should I include LateSimulationSystemGroup?
        }
    }

    private void LateUpdate()
    {
        if (m_World != null)
        {
            m_World.GetExistingSystem<PresentationSystemGroup>().Update(m_World.Unmanaged);        
        }
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