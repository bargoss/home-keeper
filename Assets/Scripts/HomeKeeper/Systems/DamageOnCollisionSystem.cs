using Components;
using Unity.Entities;

namespace HomeKeeper.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct DamageOnCollisionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            //SystemAPI.Query<DamageOnCollision, DynamicBuffer<>>()
        }
    }
}