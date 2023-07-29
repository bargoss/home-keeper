using System;
using System.Linq;
using DefaultNamespace;
using DefenderGame.Scripts.Components;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DefenderGame.Scripts.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(ItemGridSystem))]
    public partial class DeGameManager : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<DeGameData>();
        }

        protected override void OnUpdate()
        {
            var gameDataRw =  SystemAPI.GetSingletonRW<DeGameData>();
            var gameData = gameDataRw.ValueRO;
            var playerInput = gameData.PlayerInput.GetUpdated(Utility.GetMousePositionInWorldSpaceXZ(), Input.GetMouseButton(0));
            
            var itemGridEntity = SystemAPI.ManagedAPI.GetSingletonEntity<DeItemGrid>();
            var itemGrid = SystemAPI.ManagedAPI.GetComponent<DeItemGrid>(itemGridEntity);
            var itemGridLtw = SystemAPI.GetComponent<LocalToWorld>(itemGridEntity);

            var time = (float)SystemAPI.Time.ElapsedTime;
            
            HandleItemGridControl(playerInput, itemGridLtw, itemGrid, time);
            
            
            
            gameData.PlayerInput = playerInput;
            gameDataRw.ValueRW = gameData;
        }

        private static void HandleItemGridControl(PlayerInput playerInput, LocalToWorld itemGridLtw, DeItemGrid itemGrid, float time)
        {
            if (playerInput.Down)
            {
                var targetGridPos = ItemGridUtils.WorldToGridPos(playerInput.MousePos, itemGridLtw, itemGrid.GridLength);

                if // if there's a valid selection 
                (
                    itemGrid.OngoingActions.FirstOrDefault(action => action is Selection) is Selection ongoingAction &&
                    itemGrid.ItemGrid.TryGetGridItem(ongoingAction.SelectedObjectPos, out _)
                )
                {
                    itemGrid.HandleMove(ongoingAction.SelectedObjectPos, targetGridPos, time);
                }
                else // selecting now
                {
                    if (itemGrid.IsPositionOccupied(targetGridPos))
                    {
                        itemGrid.OngoingActions.Add(new Selection(time, targetGridPos));
                    }
                }

                itemGrid.OngoingActions.RemoveWhere(action => action is Selection);
            }
        }

        public void CleanUp()
        {
            
        }

        //public void LoadLevel(. . .)
        //{
        //    
        //}
    }
}