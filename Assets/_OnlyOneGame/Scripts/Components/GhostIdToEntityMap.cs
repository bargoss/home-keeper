using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Components
{
    public class GhostIdToEntityMap : IComponentData
    {
        private Dictionary<int, Entity> m_Value;

        private Dictionary<int, Entity> Value => m_Value ??= new Dictionary<int, Entity>(100);


        public void Clear()
        {
            Value.Clear();
        }
        
        public void Set(int ghostId, Entity entity)
        {
            Value[ghostId] = entity;
        }
        
        public bool TryGet(int ghostId, out Entity entity)
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
            state.RequireForUpdate<GhostIdToEntityMap>();
        }
        public void OnDestroy(ref SystemState state)
        {
            
        }
        public void OnUpdate(ref SystemState state)
        {
            var ghostIdToEntityMap = SystemAPI.ManagedAPI.GetSingleton<GhostIdToEntityMap>();

            ghostIdToEntityMap.Clear();
            
            foreach (var (ghostInstance, entity) in SystemAPI.Query<GhostInstance>().WithEntityAccess())
            {
                ghostIdToEntityMap.Set(ghostInstance.ghostId, entity);
            }
        }
    }
}