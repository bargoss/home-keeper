using HomeKeeper.Components;
using Unity.Entities;
using UnityEngine;

namespace HomeKeeper.Authoring
{
    public class ItemSocketAuthoring : MonoBehaviour
    {
        public ItemType AcceptedItemType;
        public bool DestroyedIfEmpty;

        public bool HoldsItem = false;
        public int HeldItemId = 0;
        public ItemType HeldItemType = ItemType.Resource;
    }
    
    public class ItemSocketBaker : Baker<ItemSocketAuthoring> 
    {
        public override void Bake(ItemSocketAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            
            
            
            
            
            
            
            
            
            
            
            
        }
    }
}