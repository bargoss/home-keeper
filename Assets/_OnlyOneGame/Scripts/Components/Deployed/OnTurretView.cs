using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Components.Deployed
{
    public struct OnTurretView : IComponentData
    {
        [GhostField] public float3 LookDirection;
        [GhostField] public NetworkTick LastShot;
    }
}