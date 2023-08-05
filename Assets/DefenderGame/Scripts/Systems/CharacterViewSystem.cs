using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.GoViews;
using Unity.Entities;

namespace DefenderGame.Scripts.Systems
{
    public partial class CharacterViewSystem : SystemBase
    {
        private PairMaintainer<CharacterView, CharacterGOView> m_PairMaintainer; // init here

        protected override void OnCreate()
        {
            RequireForUpdate<CharacterView>();
        }
        protected override void OnUpdate()
        {
            
        }
    }
}