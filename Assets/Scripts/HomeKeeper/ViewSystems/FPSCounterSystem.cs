using Unity.Entities;

namespace HomeKeeper.ViewSystems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class FPSCounterSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            
        }
    }

}