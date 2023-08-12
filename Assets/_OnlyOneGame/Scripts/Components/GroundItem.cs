using Unity.Entities;
using Unity.Mathematics;

namespace _OnlyOneGame.Scripts.Components
{
    public struct GroundItem : IComponentData
    {
        public Item Item;
        public float PlacementTime;
        
        //ctor
        public GroundItem(Item item, float placementTime)
        {
            Item = item;
            PlacementTime = placementTime;
        }
    }

    public struct DeployingItem : IComponentData
    {
        public float DeploymentStartedTime;
        public float DeployDuration;
        public DeployableItemType Item;
        public float3 DeploymenmtDirection;
        
        //ctor
        public DeployingItem(DeployableItemType item, float3 deploymenmtDirection, float deployDuration, float time)
        {
            Item = item;
            DeployDuration = deployDuration;
            DeploymentStartedTime = 0;
            DeploymenmtDirection = deploymenmtDirection;
        }
    }
    public struct DeployedItem : IComponentData { }
}