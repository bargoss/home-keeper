using Unity.Entities;
using UnityEngine;

namespace RunnerGame.Scripts.ECS.Components
{
    public class RgGateAuthoring : MonoBehaviour
    {
        public GateType GateType;
        public float Value;

        public class RgGateAuthoringBaker : Baker<RgGateAuthoring>
        {
            public override void Bake(RgGateAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Gate { GateType = authoring.GateType, Value = authoring.Value });
            }
        }
    }

    public struct Gate : IComponentData
    {
        public GateType GateType;
        public float Value;
    }

    public enum GateType
    {
        Multiply,
        Destroy,
        SpeedEffect,
        Money,
    }
}