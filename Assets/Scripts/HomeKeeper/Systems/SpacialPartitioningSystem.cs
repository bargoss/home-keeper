using System;
using HomeKeeper.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace HomeKeeper.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct SpacialPartitioningSystem : ISystem
    {
        private NativeList<GridCell> m_Grids;
        private NativeHashMap<Entity, float3> m_Velocities;
        private const float GridSize = 5;
        
        
        public void OnCreate(ref SystemState state)
        {
            m_Grids = new NativeList<GridCell>(200, Allocator.Persistent);
            m_Velocities = new NativeHashMap<Entity, float3>(1000, Allocator.Persistent);
        }
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (localToWorld, spacialPartitioningEntry) in SystemAPI.Query<LocalToWorld, SpacialPartitioningEntry>())
            {
                
            }

            // gotta use this to make sure system isn't finished before the parallel jobs are finished
            // state.Dependency
        }
        public void OnDestroy(ref SystemState state)
        {
            m_Grids.Dispose();
            m_Velocities.Dispose();
        }
    }
    
    public struct GridCell
    {
        // 4 + 4 + 4, 4 + 4
        // 12, 8
        // 20 bytes per element
        public FixedList512Bytes<(float3, Entity)> Entities;
    }

    public struct SpacialIndexing : IDisposable
    {
        private NativeHashMap<int3, GridCell> m_Grids;
        private NativeHashMap<Entity, float3> m_Velocities;
        
        
        

        public void Dispose()
        {
            m_Grids.Dispose();
            m_Velocities.Dispose();
        }
    }
}