using Unity.Entities;

namespace DefenderGame.Scripts.Components
{
    public struct DeGamePrefabs : IComponentData
    {
        public Entity ProjectilePrefab;
        public Entity Enemy0Prefab;
    }
}