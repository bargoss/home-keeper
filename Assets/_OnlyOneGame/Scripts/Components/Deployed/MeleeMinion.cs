using Unity.Entities;
using Unity.Mathematics;

namespace _OnlyOneGame.Scripts.Components.Deployed
{
    public struct MeleeMinion : IComponentData
    {
        public float3 LookDirection;
        public float LastAttack;
        public bool AttackInput;
    }
}