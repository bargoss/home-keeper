using Unity.Entities;

namespace Components
{
    public struct Faction : IComponentData
    {
        public int Value;
        
        public bool IsNeutral => Value == 0;
        
        public static Faction Neutral => new Faction {Value = 0};
    }
}