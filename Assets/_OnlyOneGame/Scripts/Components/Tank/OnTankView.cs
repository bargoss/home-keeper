using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Components.Tank
{
    public struct OnTankView : IComponentData
    {
        [GhostField] public bool Aiming;
        [GhostField] public float3 AimingDelta;
        [GhostField] public float3 MovementVelocity;
        [GhostField] public NetworkTick LastShotTick;
        [GhostField] public NetworkTick LastDamageTick;
    }
}