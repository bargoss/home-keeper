using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Components
{
    public struct NetIdToEntityMap : IComponentData
    {
        private NativeHashMap<int, Entity> m_Value;
        
        
        public void Clear()
        {
            m_Value.Clear();
        }
        
        public void Set(NetworkId netId, Entity entity)
        {
            m_Value[netId.Value] = entity;
        }
        
        public bool TryGet(NetworkId id, out Entity entity)
        {
            return m_Value.TryGetValue(id.Value, out entity);
        }
        
        public void Dispose()
        {
            m_Value.Dispose();
        }
        
        public static NetIdToEntityMap Create()
        {
            return new NetIdToEntityMap
            {
                m_Value = new NativeHashMap<int, Entity>(100, Allocator.Persistent)
            };
        }
    }
    
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct NetIdToEntityMapSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkId>();
        }
        public void OnDestroy(ref SystemState state)
        {
            if (SystemAPI.TryGetSingletonRW(out RefRW<NetIdToEntityMap> netIdToEntityMapRw))
            {
                netIdToEntityMapRw.ValueRW.Dispose();
            }
        }
        public void OnUpdate(ref SystemState state)
        {
            if (SystemAPI.TryGetSingletonRW(out RefRW<NetIdToEntityMap> netIdToEntityMapRw))
            {
                
            }
            else
            {
                var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
                var netIdToEntityMapEntity = commandBuffer.CreateEntity();
                commandBuffer.AddComponent<NetIdToEntityMap>(netIdToEntityMapEntity);
                commandBuffer.Playback(state.EntityManager);
                commandBuffer.Dispose();
                netIdToEntityMapRw = SystemAPI.GetSingletonRW<NetIdToEntityMap>();
            }

            netIdToEntityMapRw.ValueRW.Clear();
            
            foreach (var (networkId, entity) in SystemAPI.Query<NetworkId>().WithEntityAccess())
            {
                netIdToEntityMapRw.ValueRW.Set(networkId, entity);
            }
        }
    }
}