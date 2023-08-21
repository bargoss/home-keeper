using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Components
{
    public class SyncedIdToEntityMap : IComponentData
    {
        private Dictionary<SyncedId, Entity> m_Value;

        private Dictionary<SyncedId, Entity> Value => m_Value ??= new Dictionary<SyncedId, Entity>(100);


        public void Clear()
        {
            Value.Clear();
        }
        
        public void Set(SyncedId ghostId, Entity entity)
        {
            Value[ghostId] = entity;
        }
        
        public bool TryGet(SyncedId ghostId, out Entity entity)
        {
            return Value.TryGetValue(ghostId, out entity);
        }
    }

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct GhostIdToEntityMapSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GhostInstance>();
            state.RequireForUpdate<SyncedIdToEntityMap>();
        }
        public void OnDestroy(ref SystemState state)
        {
            
        }
        public void OnUpdate(ref SystemState state)
        {
            var ghostIdToEntityMap = SystemAPI.ManagedAPI.GetSingleton<SyncedIdToEntityMap>();

            ghostIdToEntityMap.Clear();
            
            foreach (var (syncedId, entity) in SystemAPI.Query<SyncedId>().WithEntityAccess())
            {
                ghostIdToEntityMap.Set(syncedId, entity);
            }
        }
    }
}