using Unity.Entities;
using UnityEngine;

namespace RunnerGame.Scripts.ECS.Components
{
    public class RgGameManagerAuthoring : MonoBehaviour
    {
        // constants
        public float PlayerForwardSpeed = 1;
        public float PlayerSidewaysP = 1;
        public float PlayerSidewaysD = 0.1f;
        public float RoadWidth = 10;
        
        // prefabs
        public GameObject PlayerPrefab;
        public GameObject ParticlePrefab;
        
        
        public class RgGameManagerAuthoringBaker : Baker<RgGameManagerAuthoring>
        {
            public override void Bake(RgGameManagerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponentObject(entity,
                    new RgGameManager
                    {
                        PlayerForwardSpeed = authoring.PlayerForwardSpeed,
                        PlayerSidewaysP = authoring.PlayerSidewaysP,
                        PlayerSidewaysD = authoring.PlayerSidewaysD,
                        PlayerPrefab = GetEntity(authoring.PlayerPrefab, TransformUsageFlags.Dynamic),
                        ParticlePrefab = GetEntity(authoring.ParticlePrefab, TransformUsageFlags.Dynamic),
                        RoadWidth = authoring.RoadWidth,
                    }
                );
            }
        }
    }

    public class RgGameManager : IComponentData
    {
        public float PlayerForwardSpeed;
        public float PlayerSidewaysP;
        public float PlayerSidewaysD;
        public Entity PlayerPrefab;
        public Entity ParticlePrefab;

        public float RoadWidth;

    }
}