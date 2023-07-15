using Unity.Entities;
using UnityEngine;

namespace WaterGame.Authoring
{
    public class WaterGameConfigAuthoring : MonoBehaviour
    {
        public float PushForce = 193.6f;
        public float Viscosity = 0.16f;
        public float InnerRadius = 0.85f;
        public float OuterRadius = 1.0f;
        public float MaxForcePerFrame = 100.0f;
        
        
        class WaterGameConfigBaker : Baker<WaterGameConfigAuthoring>
        {
            public override void Bake(WaterGameConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new WaterGameConfig()
                {
                    PushForce = authoring.PushForce,
                    Viscosity = authoring.Viscosity,
                    InnerRadius = authoring.InnerRadius,
                    OuterRadius = authoring.OuterRadius,
                    MaxForcePerFrame = authoring.MaxForcePerFrame
                });
            }
        }
    }

    public struct WaterGameConfig : IComponentData
    {
        public float PushForce;
        public float Viscosity;
        public float InnerRadius;
        public float OuterRadius;
        public float MaxForcePerFrame;
    }
    
}