using Unity.Entities;

namespace _OnlyOneGame.Scripts.Components
{
    public struct GroundItem : IComponentData
    {
        public Item Item;
        public float PlacementTime;
    }

    public struct DeployingItem : IComponentData
    {
        public float DeploymentStartedTime;
        public float DeployDuration;
        public DeployableItemType Item;
    }
    public struct DeployedItem : IComponentData { }
}