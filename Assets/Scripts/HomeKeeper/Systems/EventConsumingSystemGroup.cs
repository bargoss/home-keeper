using Unity.Entities;
using UnityEngine.Scripting;

namespace HomeKeeper.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial class EventConsumingSystemGroup : ComponentSystemGroup
    {
        [Preserve]
        public EventConsumingSystemGroup()
        {
        }
    }
}