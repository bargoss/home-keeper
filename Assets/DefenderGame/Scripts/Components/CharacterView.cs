using Unity.Entities;

namespace DefenderGame.Scripts.Components
{
    public struct CharacterView : IComponentData
    {
        public bool ViewIdAssigned { get; private set; }
        public int ViewId { get; private set; }
        
        public void AssignViewId(int viewId)
        {
            ViewId = viewId;
            ViewIdAssigned = true;
        }
    }
}