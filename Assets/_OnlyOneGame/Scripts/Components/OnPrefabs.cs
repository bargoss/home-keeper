using Unity.Entities;

namespace _OnlyOneGame.Scripts.Components
{
    public struct OnPrefabs : IComponentData
    {
        public EntWithComponent<GroundItem> GroundItemPrefab;
        public EntWithComponent<DeployingItem> DeployingItemPrefab;
        public Entity SimplePlayerPrefab;
    }
}