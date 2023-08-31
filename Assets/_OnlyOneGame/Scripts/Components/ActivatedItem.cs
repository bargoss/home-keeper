using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Components
{
    public struct ActivatedItem : IComponentData
    {
        [GhostField] public NetworkTick ActivatedTick;
        [GhostField] public uint ActivationDurationTicks;
        [GhostField(Quantization = 1000)] public float3 Direction;
        
        //ctor
        public ActivatedItem(float3 direction, uint activationDurationTicks, NetworkTick activatedTick)
        {
            Direction = direction;
            ActivationDurationTicks = activationDurationTicks;
            ActivatedTick = activatedTick;
        }
    }
}