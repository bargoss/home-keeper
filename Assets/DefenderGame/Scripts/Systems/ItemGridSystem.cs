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

            var completedActions = new List<OngoingAction>();
            
            foreach (var ongoingAction in itemGrid.OngoingActions)
            {
                switch (ongoingAction)
                {
                    case AmmoBoxFillingMagazine ammoBoxFillingMagazine:
                        if (
                            itemGrid.ItemGrid.TryGetGridItem<AmmoBox>(ammoBoxFillingMagazine.AmmoBoxPos, out var ammoBox) &&
                            itemGrid.ItemGrid.TryGetGridItem<Magazine>(ammoBoxFillingMagazine.MagazinePos, out var magazine)
                        )
                        {
                            if(ammoBox.AmmoCount == 0){ completedActions.Add(ongoingAction); }
                            else if(magazine.AmmoCount == magazine.AmmoCapacity){ completedActions.Add(ongoingAction); }
                            else
                            {
                                // transfer ammo
                                ammoBox.SetAmmoCount(ammoBox.AmmoCount - 1, time);
                                magazine.SetAmmoCount(magazine.AmmoCount + 1, time);
                            }
                        }
                        else{ completedActions.Add(ongoingAction); }
                        
                        break;
                    case TurretLoadingMagazine turretLoadingMagazine:
                        if (itemGrid.ItemGrid.TryGetGridItem<Turret>(turretLoadingMagazine.TurretPos, out var turret))
                        {
                            // todo
                        }
                        else { completedActions.Add(ongoingAction); }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(ongoingAction));
                }
            }
            
            completedActions.ForEach(action => itemGrid.OngoingActions.Remove(action));
        }
    }

    public partial class ItemGridViewSystem : SystemBase
    {
        public Dictionary<ItemGrid, DeItemGridView> ItemGridViews = new();
        public Dictionary<Turret, TurretGOView> TurretViews = new();
        public Dictionary<Magazine, MagazineGOView> MagazineViews = new();
        public Dictionary<AmmoBox, AmmoBoxGOView> AmmoBoxViews = new();

        protected override void OnCreate()
        {
            RequireForUpdate<ItemGrid>();
        }
        protected override void OnUpdate()
        {
            var itemGridViewPrefab = GameResources.Instance.ItemGridViewPrefab;

            HandleViewCreation();
            HandleViewDestroying();
            HandleViewUpdating();
        }

        private void HandleViewCreation()
        {
            Entities.ForEach((Entity entity, ItemGrid itemGrid) =>
            {
                // todo
                
                
            }).WithBurst().Run();
        }
        private void HandleViewDestroying()
        {
            Entities.ForEach((Entity entity, ItemGrid itemGrid) =>
            {
                // todo
            }).WithBurst().Run();
        }
        private void HandleViewUpdating()
        {
            Entities.ForEach((Entity entity, ItemGrid itemGrid) =>
            {
                // todo
            }).WithBurst().Run();
        }
    }
}