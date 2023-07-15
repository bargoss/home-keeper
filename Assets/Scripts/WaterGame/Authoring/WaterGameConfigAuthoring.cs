using Unity.Entities;
using UnityEngine;

namespace WaterGame.Authoring
{
    public class WaterGameConfigAuthoring : MonoBehaviour
    {
        public float PushForce = 10;
        public float Viscosity = 1;
        public float InnerRadius = 2.5f;
        public float OuterRadius = 3f;
        
        
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
                    OuterRadius = authoring.OuterRadius
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
    }
    
}