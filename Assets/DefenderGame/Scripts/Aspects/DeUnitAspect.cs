using Components;
using DefenderGame.Scripts.Components;
using HomeKeeper.Components;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Stateful;
using Unity.Transforms;

namespace DefenderGame.Scripts.Aspects
{
    public readonly partial struct DeUnitAspect : IAspect
    {
        public readonly Entity Self;
        
        private readonly RefRW<DeUnit> m_DeUnit;
        private readonly RefRW<Health> m_Health;
        private readonly RefRW<Faction> m_Faction;
        private readonly RefRW<DamageOpposingFactionOnCollision> m_DamageOnCollision;
        private readonly DynamicBuffer<StatefulCollisionEvent> m_StatefulCollisionEvents;

        private readonly RefRW<LocalTransform> m_LocalTransform;
        private readonly RefRW<PhysicsVelocity> m_PostTransformMatrix;

    }

    public readonly partial struct DeUnitSpawnerAspect : IAspect
    {
        public readonly Entity Self;

        private readonly RefRW<DeUnitSpawner> m_UnitSpawner;
        private readonly RefRW<Faction> m_Faction;

        public void HandleUnitInteraction(Faction capturingUnitFaction, out bool shouldConsumeUnit)
        {
            shouldConsumeUnit = false;
            
            var unitCapacity = m_UnitSpawner.ValueRO.UnitsCapacity;
            var unitCountInside = m_UnitSpawner.ValueRO.UnitCountInside;
            var unitFactionInside = m_UnitSpawner.ValueRO.UnitFactionInside;
            
            
            // add
            if (unitFactionInside.Value == capturingUnitFaction.Value || unitCountInside == 0)
            {
                shouldConsumeUnit = true;
                
                if (unitCountInside < unitCapacity)
                {
                    unitCountInside += 1;

                    if (unitCountInside == unitCapacity)
                    {
                        m_Faction.ValueRW = capturingUnitFaction;
                    }
                    
                    unitFactionInside = capturingUnitFaction;
                }
            }
            // remove
            else
            {
                if (unitCountInside > 0)
                {
                    shouldConsumeUnit = true;
                    unitCountInside -= 1;
                    
                    if (unitCountInside == 0)
                    {
                        m_Faction.ValueRW = Faction.Neutral;
                    }
                }
            }

            
            var unitSpawner = m_UnitSpawner.ValueRO;
            unitSpawner.UnitCountInside = unitCountInside;
            unitSpawner.UnitFactionInside = unitFactionInside;
            m_UnitSpawner.ValueRW = unitSpawner;
        }
    }

    
}