using Components;
using Unity.Entities;
using Unity.Mathematics;

namespace DefenderGame.Scripts.Components
{
    public struct DeUnit : IComponentData
    {
        
    }

    public struct DeUnitSpawner : IComponentData
    {
        // stats:
        public float SpawnInterval;
        public int UnitsCapacity;
        
        // state:
        public float LastSpawn;
        public int UnitCountInside;
        public Faction UnitFactionInside;
    }

    public struct DamageOpposingFactionOnCollision : IComponentData
    {
        public float DamagePerSecond;
    }

}
