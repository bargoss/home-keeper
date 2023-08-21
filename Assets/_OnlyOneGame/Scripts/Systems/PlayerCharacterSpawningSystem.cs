using System.Collections.Generic;
using _OnlyOneGame.Scripts.Components;
using DefaultNamespace;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;
using ValueVariant;
using Random = Unity.Mathematics.Random;

namespace _OnlyOneGame.Scripts.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class PlayerCharacterSpawningSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<OnPlayer>();
            RequireForUpdate<OnPrefabs>();
        }

        protected override void OnUpdate()
        {
            //return;
            var prefabs = SystemAPI.GetSingleton<OnPrefabs>();
            var playerCharacterPrefab = prefabs.PlayerCharacterPrefab;
            var ghostIdToEntityMap = SystemAPI.ManagedAPI.GetSingleton<SyncedIdToEntityMap>();

            var random = Random.CreateFromIndex((uint)(SystemAPI.Time.ElapsedTime * 1000));

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            

            var onPlayerAndNewCharacter = new List<(Entity, Entity)>();
            
            foreach (var (onPlayerRw, ghostOwner, entity) in SystemAPI.Query<RefRW<OnPlayer>, GhostOwner>().WithEntityAccess())
            {
                var onPlayer = onPlayerRw.ValueRO;

                if (!ghostIdToEntityMap.TryGet(onPlayer.ControlledCharacterSyncedId, out _))
                {
                    var spawnPos = random.NextFloat3Direction() * random.NextFloat(0, 1f);

                    var spawnedCharacter = EntityManager.Instantiate(playerCharacterPrefab);
                    ecb.SetLocalPositionRotation(spawnedCharacter, spawnPos, quaternion.identity);
                    ecb.SetComponent(spawnedCharacter, ghostOwner);
                    var syncedId = new SyncedId(random.NextInt());
                    ecb.SetComponent(spawnedCharacter, syncedId);
                    onPlayerAndNewCharacter.Add((entity, spawnedCharacter));
                    
                    onPlayer.ControlledCharacterSyncedId = syncedId;
                }
            }
            
            ecb.Playback(EntityManager);

            //return;
            foreach (var (onPlayerEntity, onPlayerCharacterEntity) in onPlayerAndNewCharacter)
            {
                var onPlayerRw = SystemAPI.GetComponentRW<OnPlayer>(onPlayerEntity);
                var characterSyncedId = EntityManager.GetComponentData<SyncedId>(onPlayerCharacterEntity);

                var onPlayer = onPlayerRw.ValueRO;
                onPlayer.ControlledCharacterSyncedId = characterSyncedId;
                onPlayerRw.ValueRW = onPlayer;
                
                Debug.Log("Spawned new character for player " + onPlayerEntity + " with syncedId " + characterSyncedId.Value);
            }
        }
    }
}