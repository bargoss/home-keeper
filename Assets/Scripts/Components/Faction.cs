using Unity.Entities;
using Unity.NetCode;

namespace Components
{
    public struct Faction : IComponentData
    {
        [GhostField] public int Value;
        
        public bool IsNeutral => Value == 0;
        
        public static Faction Neutral => new Faction {Value = 0};
        
        public Faction(int value)
        {
            Value = value;
        }
    }
}