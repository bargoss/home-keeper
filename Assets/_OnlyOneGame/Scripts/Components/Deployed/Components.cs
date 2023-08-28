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
    public struct OnTurretView : IComponentData
    {
        [GhostField] public float3 LookDirection;
        [GhostField] public NetworkTick LastShot;
    }
    
    public struct MeleeMinion : IComponentData
    {
        public float3 LookDirection;
        public float LastAttack;
        public bool AttackInput;
    }
}