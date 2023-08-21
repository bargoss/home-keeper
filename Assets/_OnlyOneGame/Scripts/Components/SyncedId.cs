using Unity.Entities;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Components
{
    public struct SyncedId : IComponentData
    {
        [GhostField] public int Value; // 0 means not assigned
        
        public SyncedId(int value)
        {
            Value = value;
        }
    }
}