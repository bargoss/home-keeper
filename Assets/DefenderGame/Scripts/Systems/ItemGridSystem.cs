using System;
using System.Collections.Generic;
using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.GoViews;
using Unity.Entities;
using UnityEngine;

namespace DefenderGame.Scripts.Systems
{
    public partial class ItemGridSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<ItemGrid>();
        }

        protected override void OnUpdate()
        {
            
            //Entities.ForEach((Entity entity, ref ParticleView particleView, in LocalToWorld localToWorld) =>
            Entities.ForEach((Entity entity, DeItemGrid itemGrid) =>
            {
                
            }).WithoutBurst().Run();
        }

        private static void HandleUpdateItemGrid(DeItemGrid itemGrid, float time)
        {
            itemGrid.ItemGrid.ForEachItem((gridItem, pos) =>
            {
                switch (gridItem)
                {
                    case AmmoBox ammoBox:
                        break;
                    case Magazine magazine:
                        break;
                    case Turret turret:
                        var shot = turret.TryShoot(time);
                        // create bullet and stuff
                        break;
                    default:
                        Debug.LogError($"Unknown grid item type: {gridItem.GetType()}");
                        break;
                }
            });
            
            itemGrid.OngoingActions.ForEach(action =>
            {
                
            });
        }
    }

    public partial class ItemGridViewSystem : SystemBase
    {
        public Dictionary<ItemGridSystem, DeItemGridView> ItemGridViews = new();
        
        protected override void OnCreate()
        {
            RequireForUpdate<ItemGrid>();
        }
        protected override void OnUpdate()
        {
            var itemGridViewPrefab = GameResources.Instance.ItemGridViewPrefab;
            
        }
    }
}