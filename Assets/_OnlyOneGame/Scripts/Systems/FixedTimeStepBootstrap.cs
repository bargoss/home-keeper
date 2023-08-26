using Unity.Entities;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct FixedTimeStepBootstrap : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            var tickRate = new ClientServerTickRate
            {
                SimulationTickRate = 10,
                NetworkTickRate = 10,
            };

            SystemAPI.SetSingleton(tickRate);
        }

        public void OnUpdate(ref SystemState state)
        {
            var tickRate = new ClientServerTickRate
            {
                SimulationTickRate = 10,
                NetworkTickRate = 10,
            };

            SystemAPI.SetSingleton(tickRate);
        }

        public void OnDestroy(ref SystemState state)
        {
        }
    }

}