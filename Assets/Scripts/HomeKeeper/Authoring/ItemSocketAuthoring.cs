﻿using HomeKeeper.Components;
using Unity.Entities;
using UnityEditor.SceneManagement;
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
            AddComponent(entity, new ItemSocket
            {
                AcceptedItemType = authoring.AcceptedItemType,
                DestroyedIfEmpty = authoring.DestroyedIfEmpty
            });
            if (authoring.HoldsItem)
            {
                AddComponent(entity, new Item()
                {
                    ItemId = authoring.HeldItemId,
                    ItemType = authoring.HeldItemType
                });
            }
        }
    }
}