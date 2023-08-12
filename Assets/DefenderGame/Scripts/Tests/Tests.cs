using System;
using _OnlyOneGame.Scripts.Components;
using DefaultNamespace;
using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using ValueVariant;

namespace DefenderGame.Scripts.Tests
{
#if UNITY_EDITOR
    public static class Tests
    {
        [MenuItem("DefenderGame/Tests/EntityMappingValueVariant")]
        public static void EntityMappingValueVariant()
        {
            PlayerEvent myVv = new EventMeleeAttackStarted()
            {
                Direction = new float3(0.5f, 0.5f, 0.5f)
            };

            var worldSource = new World("world a");
            var worldDestination = new World("world b");

            var eSource = worldSource.EntityManager.CreateEntity();
            worldSource.EntityManager.AddComponent<OnPlayerCharacter>(eSource);
            var playerCharacter = new OnPlayerCharacter();
            playerCharacter.Events = new FixedList128Bytes<PlayerEvent>();
            playerCharacter.Events.Add(new EventItemPickedUp(){Item = MinionType.Tank });
            playerCharacter.Events.Add(new EventMeleeAttackStarted() { Direction = new float3(0.15f, 0.25f, 0.35f) });
            //playerCharacter.OnGoingAction = Option<OnGoingAction>.Some(new ItemPickedUpEvent(){Item = new Item()});
            worldSource.EntityManager.SetComponentData(eSource, playerCharacter);

            var sourceEntitiesToCopy = new NativeArray<Entity>(1, Allocator.Temp);
            sourceEntitiesToCopy[0] = eSource;

            worldDestination.EntityManager.CopyEntitiesFrom(worldSource.EntityManager, sourceEntitiesToCopy);
            
            // m_ReadonlyTestQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<OcclusionTest>());
            worldDestination.EntityManager.CreateEntityQuery(typeof(OnPlayerCharacter));
            
            // get the first entity
            var eDestination = worldDestination.EntityManager.CreateEntityQuery(typeof(OnPlayerCharacter)).GetSingletonEntity();
            var playerCharacterDestination = worldDestination.EntityManager.GetComponentData<OnPlayerCharacter>(eDestination);
            var playerEvent = playerCharacterDestination.Events[0];
            
            var a = 3;
        }
        
        // menu item
        [MenuItem("DefenderGame/Tests/TestTurretUpdate")]
        public static void TestTurretUpdate()
        {
            var itemGrid = new DeItemGrid();
            itemGrid.ItemGrid.TryPlaceItem(new int2(0, 0), new Magazine(5, 10, 0, 0));
            itemGrid.ItemGrid.TryPlaceItem(new int2(1, 0), new Magazine(5, 10, 0, 0));
            itemGrid.ItemGrid.TryPlaceItem(new int2(0, 1), new AmmoBox(200, 200, 0, 0));
            itemGrid.ItemGrid.TryPlaceItem(new int2(1, 1), new AmmoBox(3, 10, 0, 0));
            itemGrid.ItemGrid.TryPlaceItem(new int2(2, 1), new AmmoBox(8, 10, 0, 0));

            var turret0 = new Turret(0.5f, 0.5f, new Magazine(10, 10, 0, 0), 0);
            var turret1 = new Turret(0.5f, 0.0f, new Magazine(10, 10, 0, 0), 0);
            turret0.AimDirection = Utility.Forward;
            turret1.AimDirection = Utility.Forward;
            
            itemGrid.ItemGrid.TryPlaceItem(new int2(0, 2), turret0);
            itemGrid.ItemGrid.TryPlaceItem(new int2(1, 2), turret1);
            
            //public static EntityCommandBuffer HandleTurretUpdate(DeItemGrid itemGrid, LocalToWorld gridLocalToWorld,
            //  float3 closestEnemyPosition, bool enemyPresent, float time, float deltaTime, EntityCommandBuffer ecb, DeGamePrefabs gamePrefabs)

            ItemGridTurretSystem.HandleTurretUpdate(
                itemGrid,
                new LocalToWorld(),
                new float3(0, 0, 1000),
                true,
                2.01f,
                0.02f,
                new EntityCommandBuffer(Unity.Collections.Allocator.Temp),
                new DeGamePrefabs(),
                true
            );
            int a = 3;
        }

        /*
        [MenuItem("DefenderGame/Tests/212412")]
        public static void MyTest()
        {
            SampleVariant2 b = new SampleVariant2(new SampleVariant(2));
            // Q: how to get size of a struct
            // A: yo
            System.Runtime.InteropServices.Marshal.SizeOf(typeof(SampleVariant2));
            Debug.Log("size : " + System.Runtime.InteropServices.Marshal.SizeOf(typeof(SampleVariant2)));
            //b.Accept(new Visitor0());

            //SampleVariant2.DefaultConverter.Instance.Visit()
            //b.Accept();
            
            //SampleVariant2.IActionVisitor<SampleVariant, bool>
            b.Switch(
                (SampleVariant bb) =>
                {
                    bb.Switch(
                        (int i) =>
                        {
                            var a = i;
                        },
                        (long l) => { 
                        
                        },
                        (float f) =>
                        {
                            
                        }
                    );
                },
                (long b) =>
                {
                    
                }
            );
        }
        */
        
        
    }
    
    [ValueVariant]
    public readonly partial struct SampleVariant: IValueVariant<SampleVariant, int> { }
    
    [ValueVariant]
    public readonly partial struct SampleVariant2: IValueVariant<SampleVariant2, SampleVariant, long> { }
#endif
}