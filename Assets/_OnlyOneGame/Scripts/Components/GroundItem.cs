using DefaultNamespace;
using DefenderGame.Scripts.Components;
using Unity.Entities;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Components
{
    public struct GroundItem : IComponentData
    {
        [GhostField] public BytesAs<Item, Data16Bytes> Item;
        public ViewId ViewId;

        //ctor
        public GroundItem(Item item)
        {
            Item = item;
            ViewId = new ViewId();
        }
    }
    
    //public struct ActivatedGroundItem : IComponentData
    //{
    //    
    //}

    public struct DeployedItem : IComponentData { }
}