using Unity.Entities;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Components
{
    public struct DestroyableGhost : IComponentData
    {
        [GhostField] public bool Destroyed;
    }
}