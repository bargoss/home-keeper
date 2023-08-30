using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Components
{
    public struct DeployingItem : IComponentData
    {
        [GhostField] public NetworkTick DeploymentStartedTick;
        [GhostField] public int DeployDurationTicks;
        [GhostField] public DeployableItemType Item;
        [GhostField(Quantization = 1000)] public float3 DeploymentDirection;
        
        //ctor
        public DeployingItem(DeployableItemType item, float3 deploymentDirection, int deployDurationTicks, NetworkTick deploymentStartedTick)
        {
            Item = item;
            DeploymentDirection = deploymentDirection;
            DeployDurationTicks = deployDurationTicks;
            DeploymentStartedTick = deploymentStartedTick;
        }
    }
}