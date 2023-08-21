using Unity.Entities;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Components
{
    public struct OnPlayer : IComponentData
    {
        [GhostField] public int ControlledCharacterGhostIdOpt;
    }
}