using System;
using System.Linq;
using DefaultNamespace;
using DefenderGame.Scripts.Components;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DefenderGame.Scripts.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(ItemGridSystem))]
    public partial class DeGameManager : SystemBase
    {
        private bool m_Initialized = false;
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

            if (!m_Initialized)
            {
                itemGrid.ItemGrid.TryPlaceItem(new int2(0, 0), new Magazine(5, 10, 0, 0));
                itemGrid.ItemGrid.TryPlaceItem(new int2(1, 0), new Magazine(5, 10, 0, 0));
                itemGrid.ItemGrid.TryPlaceItem(new int2(0, 1), new AmmoBox(10, 10, 0, 0));
                itemGrid.ItemGrid.TryPlaceItem(new int2(1, 1), new AmmoBox(3, 10, 0, 0));
                itemGrid.ItemGrid.TryPlaceItem(new int2(2, 1), new AmmoBox(8, 10, 0, 0));
                itemGrid.ItemGrid.TryPlaceItem(new int2(0, 2), new Turret(1, 0, null, 0));

                m_Initialized = true;
            }
            
            
            HandleItemGridControl(playerInput, itemGridLtw, itemGrid, time);
            
            
            
            gameData.PlayerInput = playerInput;
            gameDataRw.ValueRW = gameData;
        }

        private static void HandleItemGridControl(PlayerInput playerInput, LocalToWorld itemGridLtw, DeItemGrid itemGrid, float time)
        {
            if (playerInput.Down)
            {
                var blockedGrids = itemGrid.GetBlockedGrids();
                
                var targetGridPos = ItemGridUtils.WorldToGridPos(playerInput.MousePos, itemGridLtw, itemGrid.GridLength);
                Debug.Log("clicked grid pos: " + targetGridPos);

                if // if there's a valid selection 
                (
                    itemGrid.OngoingActions.FirstOrDefault(action => action is Selection) is Selection ongoingAction &&
                    itemGrid.ItemGrid.TryGetGridItem(ongoingAction.SelectedObjectPos, out _) &&
                    !blockedGrids.Contains(targetGridPos)
                )
                {
                    itemGrid.HandleMove(ongoingAction.SelectedObjectPos, targetGridPos, time);
                    itemGrid.OngoingActions.RemoveWhere(action => action is Selection);
                }
                else // selecting now
                {
                    if (itemGrid.IsPositionOccupied(targetGridPos) && !blockedGrids.Contains(targetGridPos))
                    {
                        itemGrid.OngoingActions.Add(new Selection(time, targetGridPos));
                    }
                }
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