using System;
using System.Linq;
using DefaultNamespace;
using DefenderGame.Scripts.Components;
using HomeKeeper.Components;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Magazine = DefenderGame.Scripts.Components.Magazine;

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
            var gameData = SystemAPI.ManagedAPI.GetSingleton<DeGameData>();
            var playerInput = gameData.PlayerInput.GetUpdated(Utility.GetMousePositionInWorldSpaceXZ(), Input.GetMouseButton(0));

            var itemGridEntity = SystemAPI.ManagedAPI.GetSingletonEntity<DeItemGrid>();
            var itemGrid = SystemAPI.ManagedAPI.GetComponent<DeItemGrid>(itemGridEntity);
            var itemGridLtw = SystemAPI.GetComponent<LocalToWorld>(itemGridEntity);

            var time = (float)SystemAPI.Time.ElapsedTime;

            if (!m_Initialized)
            {
                itemGrid.ItemGrid.TryPlaceItem(new int2(0, 0), new Magazine(5, 10, 0, 0));
                itemGrid.ItemGrid.TryPlaceItem(new int2(1, 0), new Magazine(5, 10, 0, 0));
                itemGrid.ItemGrid.TryPlaceItem(new int2(0, 1), new AmmoBox(200, 200, 0, 0));
                itemGrid.ItemGrid.TryPlaceItem(new int2(1, 1), new AmmoBox(3, 10, 0, 0));
                itemGrid.ItemGrid.TryPlaceItem(new int2(2, 1), new AmmoBox(8, 10, 0, 0));
                itemGrid.ItemGrid.TryPlaceItem(new int2(0, 2), new Turret(0.5f, 5.65f, new Magazine(10,10,0,0), 0));
                itemGrid.ItemGrid.TryPlaceItem(new int2(1, 2), new Turret(0.5f, 5, new Magazine(10,10,0,0), 0));

                m_Initialized = true;
            }

            //SystemAPI.Query<>()
            
            
            
            HandleEnemySpawning(EntityManager);

            HandleItemGridControl(playerInput, itemGridLtw, itemGrid, time);
            
            
            
            gameData.PlayerInput = playerInput;
        }

        private void HandleEnemySpawning(EntityManager entityManager)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var enemySpawnerEntity = SystemAPI.GetSingletonEntity<DeEnemySpawnPosition>();
            var enemySpawnerLtw = SystemAPI.GetComponent<LocalToWorld>(enemySpawnerEntity);
            var gameData = SystemAPI.ManagedAPI.GetSingleton<DeGameData>();
            var prefabs = SystemAPI.GetSingleton<DeGamePrefabs>();
            
            
            var spawnCooldown = gameData.EnemySpawnRate;
            var time = (float)SystemAPI.Time.ElapsedTime; 
            if (time > gameData.LastEnemySpawnTime + spawnCooldown)
            {
                var enemy = ecb.Instantiate(prefabs.Enemy0Prefab);
                ecb.SetLocalPositionRotation(enemy, enemySpawnerLtw.Position, quaternion.LookRotation(new float3(0,0,-1), new float3(0,1,0)));
                gameData.LastEnemySpawnTime = time;
            }
            
            ecb.Playback(entityManager);
        }
        
        private static void HandleItemGridControl(PlayerInput playerInput, LocalToWorld itemGridLtw, DeItemGrid itemGrid, float time)
        {
            if (playerInput.Down)
            {
                var blockedGrids = itemGrid.GetBlockedGrids();
                
                var targetGridPos = ItemGridUtils.WorldToGridPos(playerInput.MousePos, itemGridLtw, itemGrid.GridLength);
                if (targetGridPos.x < itemGrid.ItemGrid.Width && targetGridPos.y < itemGrid.ItemGrid.Height == false)
                {
                    // out of bounds
                    return;
                }
                
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