using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Components.Deployed
{
    public struct MeleeAttacker : IComponentData
    {
        public NetworkTick LastAttack;
        public float3 LookDirection;
        public bool AttackInput;
    }
    public struct RangedAttacker : IComponentData
    {
        public NetworkTick LastAttack;
        public float3 LookDirection;
        public bool AttackInput;
    }
    public struct OnMinion : IComponentData
    {
        // state:
        public NetworkTick LastAttack;
        public int AttackCooldown;
        
        // input
        public float3 LookDirection;
        public bool AttackInput;
    }

    public struct OnMinionAI : IComponentData
    {
        public float3 TargetPosition;
        
        // const:
        public float CanAttackRange;
    }
}