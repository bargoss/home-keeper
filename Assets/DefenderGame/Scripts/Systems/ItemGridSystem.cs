using System.Collections.Generic;
using DefenderGame.Scripts.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DefenderGame.Scripts.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ItemGridSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<DeItemGrid>();
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((DeItemGrid itemGrid) =>
            {
                HandleUpdateItemGrid(itemGrid, (float)SystemAPI.Time.ElapsedTime);
            }).WithoutBurst().Run();
        }

        private static void HandleUpdateItemGrid(DeItemGrid itemGrid, float time)
        {
            itemGrid.ItemGrid.ForEachItem((gridItem, _) =>
            {
                switch (gridItem)
                {
                    //case AmmoBox ammoBox:
                    //    break;
                    //case Magazine magazine:
                    //    break;
                    case Turret turret:
                        var shot = turret.TryShoot(time);
                        if (shot)
                        {
                            // todo create bullet entity
                        }
                        break;
                    default:
                        Debug.LogError($"Unknown grid item type: {gridItem.GetType()}");
                        break;
                }
            });

            var completedActions = new List<OngoingAction>();
            var newActions = new List<OngoingAction>();
            
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
                            else if(ammoBoxFillingMagazine.LastAmmoLoadedTime + ammoBoxFillingMagazine.TimePerAmmoLoad > time)
                            {
                                // transfer ammo
                                ammoBox.SetAmmoCount(ammoBox.AmmoCount - 1, time);
                                magazine.SetAmmoCount(magazine.AmmoCount + 1, time);
                                ammoBoxFillingMagazine.LastAmmoLoadedTime = time;
                            }
                        }
                        else{ completedActions.Add(ongoingAction); }
                        
                        break;
                    case GridEffect gridEffect:
                        if (time > gridEffect.StartTime + gridEffect.Duration)
                        {
                            completedActions.Add(gridEffect);
                        } 
                        break;
                    case Moving moving:
                        if(moving.StartTime + moving.Duration > time)
                        {
                            var placedToTarget = itemGrid.ItemGrid.TryPlaceItem(moving.TargetPosition, moving.MovingObject);
                            if (placedToTarget)
                            {
                                //public GridEffect(float startTime, int2[] gridPositions, EnMsg msg) : base(startTime)
                                newActions.Add(new GridEffect(
                                    time,
                                    ItemGridUtils.GetGridsFromPivotAndOffsets(
                                        moving.TargetPosition, moving.MovingObject.Occupations
                                    ),
                                    GridEffect.EnMsg.Neutral, 0f)
                                );
                                break;
                            }

                            newActions.Add(new GridEffect(
                                time,
                                ItemGridUtils.GetGridsFromPivotAndOffsets(
                                    moving.TargetPosition, moving.MovingObject.Occupations
                                ),
                                GridEffect.EnMsg.Negative, 0f)
                            );
                            
                            var placedBackToOriginalPos = itemGrid.ItemGrid.TryPlaceItem(moving.OriginalPosition, moving.MovingObject);
                            
                            // todo handle further cases if it fails to place back to original position

                            completedActions.Add(moving);
                        }
                        
                        break;
                    case Selection selection:
                        // ignored
                        break;
                    case TurretLoadingMagazine turretLoadingMagazine:
                        if (itemGrid.TryGetGridObject<Turret>(turretLoadingMagazine.TurretPos, out var turret))
                        {
                            // todo
                            if (turretLoadingMagazine.StartTime + turretLoadingMagazine.ActionDuration > time) 
                            {
                                turret.SetMagazine(turretLoadingMagazine.NewMagazine, time);
                                if (itemGrid.ItemGrid.TryPlaceItem(turretLoadingMagazine.NewMagazinePositionBeforeLoad, turretLoadingMagazine.PreviousMagazine))
                                {
                                    // OK
                                }
                                else
                                {
                                    // todo handle this case (low priority)
                                    Debug.LogError("Failed to place magazine back to grid because its occupied, we should probably place it somewhere else or something");
                                }
                                completedActions.Add(ongoingAction);
                            }
                        }
                        else { completedActions.Add(ongoingAction); }
                        break;
                    
                    default:
                        Debug.LogError($"Unknown ongoing action type: {ongoingAction.GetType()}");
                        break;
                }
            }
            
            completedActions.ForEach(action => itemGrid.OngoingActions.Remove(action));
            newActions.ForEach(action => itemGrid.OngoingActions.Add(action));
        }
    }
}