using Components;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct TestSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            int count = 0;
            foreach (var (testComponent, entity) in SystemAPI.Query<TestComponent>().WithEntityAccess())
            {
                Debug.Log($"Seen TestComponent");
                count++;
            }
            
            Debug.Log($"TestComponent count: {count}");
        }
    }
}