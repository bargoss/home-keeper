using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Components.Deployed
{
    public struct OnTurret : IComponentData
    {
        public bool ShootInput { get; set; }
        public float3 LookDirection { get; set; }
        public NetworkTick LastShot { get; set; }
    }
}