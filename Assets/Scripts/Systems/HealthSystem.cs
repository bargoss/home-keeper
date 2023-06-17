using Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct HealthSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var health in SystemAPI.Query<RefRW<Health>>())
            {
                var h = health.ValueRO;
                if (h.HitPoints > 0)
                {
                    h.HitPoints += h.RegenerationRate * SystemAPI.Time.fixedDeltaTime;
                    h.HitPoints = math.clamp(h.HitPoints, 0, h.MaxHitPoints);
                    health.ValueRW = h;
                }
            }
        }
    }
}