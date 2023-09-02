using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine.Scripting;

namespace _OnlyOneGame.Scripts.Other
{
    [Preserve]
    [GhostComponentVariation(typeof(LocalTransform), "Transform - 3D - extrapolated")]
    [GhostComponent(PrefabType=GhostPrefabType.All, SendTypeOptimization=GhostSendType.AllClients)]
    public struct TransformDefaultVariant
    {
        /// <summary>
        /// The position value is replicated with a default quantization unit of 1000 (so roughly 1mm precision per component).
        /// The replicated position value support both interpolation and extrapolation
        /// </summary>
        [GhostField(Quantization=1000, Smoothing=SmoothingAction.InterpolateAndExtrapolate, MaxSmoothingDistance = 100000)]
        public float3 Position;

        /// <summary>
        /// The scale value is replicated with a default quantization unit of 1000.
        /// The replicated scale value support both interpolation and extrapolation
        /// </summary>
        [GhostField(Quantization=1000, Smoothing=SmoothingAction.InterpolateAndExtrapolate)]
        public float Scale;

        /// <summary>
        /// The rotation quaternion is replicated and the resulting floating point data use for replication the rotation is quantized with good precision (10 or more bits per component)
        /// </summary>
        [GhostField(Quantization=1000, Smoothing=SmoothingAction.InterpolateAndExtrapolate)]
        public quaternion Rotation;
    }
}