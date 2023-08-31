using Unity.Entities;
using Unity.Mathematics;

namespace _OnlyOneGame.Scripts.Components
{
    public struct OnPrefabs : IComponentData
    {
        public EntWithComponent<GroundItem> GroundItemPrefab;
        //public EntWithComponent<DeployingItem> DeployingItemPrefab;
        //public Entity SimplePlayerPrefab;
        public Entity PlayerPrefab;
        public Entity PlayerCharacterPrefab;
        public Entity ProjectilePrefab;
        
        public Entity TurretPrefab;
    }
}