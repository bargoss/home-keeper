using System.Collections.Generic;
using _OnlyOneGame.Scripts.Components;
using DefaultNamespace;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using ValueVariant;

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
            var prefabs = SystemAPI.GetSingleton<OnPrefabs>();
            var playerCharacterPrefab = prefabs.PlayerCharacterPrefab;
            var netIdToEntityMap = SystemAPI.GetSingleton<NetIdToEntityMap>();

            var random = Random.CreateFromIndex((uint)(SystemAPI.Time.ElapsedTime * 1000));

            var ecb = new EntityCommandBuffer();
            
            foreach (var (onPlayerRw, entity) in SystemAPI.Query<RefRW<OnPlayer>>().WithEntityAccess())
            {
                var onPlayer = onPlayerRw.ValueRO;
                var controlledCharacterNetId = onPlayer.ControlledCharacterOpt;
                
                if(netIdToEntityMap.TryGet(controlledCharacterNetId, out var controlledCharacterEntity))
                {
                    // already spawned
                    continue;
                }
                else
                {
                    var spawnPos = random.NextFloat3Direction() * random.NextFloat(1, 3);
                    
                    var spawnedCharacter = EntityManager.Instantiate(playerCharacterPrefab);
                    ecb.SetLocalPositionRotation(spawnedCharacter, spawnPos, quaternion.identity);
                    
                    onPlayer.ControlledCharacterOpt = new NetworkId(EntityManager.GetComponentData<NetworkId>(spawnedCharacter).Value);
                    
                }
                
            }
        }
    }
}