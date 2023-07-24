using Components;
using Unity.Entities;
using Unity.Mathematics;

namespace DefenderGame.Scripts.Components
{
    public struct DeUnitSpawner : IComponentData
    {
        // stats:
        public float SpawnInterval;
        public int UnitsCapacity;
        
        // state:
        public float LastSpawn;
        public int UnitCountInside;
        public Faction UnitFactionInside;
        
        // input:
        public float3 TargetPosition;
    }
}