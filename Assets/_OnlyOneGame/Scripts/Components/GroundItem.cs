using DefaultNamespace;
using Unity.Entities;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Components
{
    public struct GroundItem : IComponentData
    {
        [GhostField] public BytesAs<Item, Data16Bytes> Item;

        //ctor
        public GroundItem(Item item)
        {
            Item = item;
        }
    }

    public struct ActivatedGroundItem : IComponentData
    {
        
    }

    public struct DeployedItem : IComponentData { }
}