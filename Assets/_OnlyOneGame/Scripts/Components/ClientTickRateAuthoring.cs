using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Components
{ 
    public class ClientTickRateAuthoring : MonoBehaviour
    {
        public uint InterpolationTimeNetTicks = 2;
        public uint InterpolationTimeMS;
        public uint MaxExtrapolationTimeSimTicks = 20;
        public uint MaxPredictAheadTimeMS = 500;
        public uint TargetCommandSlack = 2;
        public int MaxPredictionStepBatchSizeRepeatedTick;
        public int MaxPredictionStepBatchSizeFirstTimeTick;
        public float InterpolationDelayJitterScale = 1.25f;
        public float InterpolationDelayMaxDeltaTicksFraction = 0.1f;
        public float InterpolationDelayCorrectionFraction = 0.1f;
        public float InterpolationTimeScaleMin = 0.85f;
        public float InterpolationTimeScaleMax = 1.1f;
        public float CommandAgeCorrectionFraction = 0.1f;
        public float PredictionTimeScaleMin = 0.9f;
        public float PredictionTimeScaleMax = 1.1f;

        public class ClientTickRateBaker : Baker<ClientTickRateAuthoring>
        {
            public override void Bake(ClientTickRateAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new ClientTickRate
                        {
                            InterpolationTimeNetTicks = authoring.InterpolationTimeNetTicks,
                            InterpolationTimeMS = authoring.InterpolationTimeMS,
                            MaxExtrapolationTimeSimTicks = authoring.MaxExtrapolationTimeSimTicks,
                            MaxPredictAheadTimeMS = authoring.MaxPredictAheadTimeMS,
                            TargetCommandSlack = authoring.TargetCommandSlack,
                            MaxPredictionStepBatchSizeRepeatedTick = authoring.MaxPredictionStepBatchSizeRepeatedTick,
                            MaxPredictionStepBatchSizeFirstTimeTick = authoring.MaxPredictionStepBatchSizeFirstTimeTick,
                            InterpolationDelayJitterScale = authoring.InterpolationDelayJitterScale,
                            InterpolationDelayMaxDeltaTicksFraction = authoring.InterpolationDelayMaxDeltaTicksFraction,
                            InterpolationDelayCorrectionFraction = authoring.InterpolationDelayCorrectionFraction,
                            InterpolationTimeScaleMin = authoring.InterpolationTimeScaleMin,
                            InterpolationTimeScaleMax = authoring.InterpolationTimeScaleMax,
                            CommandAgeCorrectionFraction = authoring.CommandAgeCorrectionFraction,
                            PredictionTimeScaleMin = authoring.PredictionTimeScaleMin,
                            PredictionTimeScaleMax = authoring.PredictionTimeScaleMax
                        });
            }
        }
    }
}