using System;
using System.Collections.Generic;
using DefenderGame.Scripts.Components;
using Unity.Entities;
using UnityEngine;

namespace DefenderGame.Scripts.Systems
{
    public partial class ItemGridSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<DeItemGrid>();
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
                switch (gridItem.Item)
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
                            itemGrid.TryGetGridObject<AmmoBox>(ammoBoxFillingMagazine.AmmoBoxPos, out var ammoBox) &&
                            itemGrid.TryGetGridObject<Magazine>(ammoBoxFillingMagazine.MagazinePos, out var magazine)
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
                        if (itemGrid.TryGetGridObject<Turret>(turretLoadingMagazine.TurretPos, out var turret))
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
}