using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace _StrategyGame.Scripts.Components
{
    public struct StUnit : IComponentData
    {
        [GhostField] public float2 TargetPosition;
    }
    public struct StSelectable : IComponentData
    {
        
    }
    public struct StSelected : IComponentData
    {
        
    }
}