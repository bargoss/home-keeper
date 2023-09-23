using System;
using System.Runtime.InteropServices;
using _OnlyOneGame.Scripts.Components;
using DefaultNamespace;
using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.NetCode.Generators;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using ValueVariant;

namespace DefenderGame.Scripts.Tests
{
#if UNITY_EDITOR
    public static class Tests
    {
        //test BytesAs<Optional<ActionCommand>, Data32Bytes>, None and Some works weirdly. It seems to appear as None but has a correct value, maybe 32 bytes are not enough to hold the thing

        //[MenuItem("DefenderGame/Tests/sdfag89sd")]
        //public static void ViewIdTest()
        //{
        //    var viewId = new ViewId();
        //    viewId.Assign(42);
        //    
        //    var characterView = new CharacterView();
        //    characterView.ViewId.Assign(42);
        //    int a = 3;
        //}
        
        [MenuItem("DefenderGame/Tests/251321321690fkdf")]
        public static void BytesAsVVSerializationTest()
        {
            var bytesAs = new BytesAs<Opt<ActionCommand>, Data32Bytes>(
                Opt<ActionCommand>.Some(new ActionCommand(new CommandMeleeAttack(new float3(0.5f, 0.5f, 0.5f))))
            );

            var innerValue = bytesAs.Get();
            innerValue.TryGet(out var innerValue2);
            innerValue2.TryGetValue(out CommandMeleeAttack innerValue3);

            var a = 3;
        }
        
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
            
            //playerCharacter.Events.Edit((ref FixedList128Bytes<PlayerEvent> value) => value.Add(new EventMeleeAttackStarted() { Direction = new float3(0.15f, 0.25f, 0.35f) }));
            playerCharacter.Events.Add(new EventMeleeAttackStarted(new float3(0.15f, 0.25f, 0.35f)));
            
            //playerCharacter.Events.Edit((ref FixedList128Bytes<PlayerEvent> value) => value.Add(new EventThrownItem() { Item = new Item() }));
            playerCharacter.Events.Add(new EventThrownItem(new Item(), new float3(0.99f, 0.925f, 0.935f)));
            
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
        
        
        [MenuItem("DefenderGame/Tests/ss321fasd")]
        public static void SerializationTests()
        {
            var bytes = new FixedString64Bytes();
            bytes.Add(1);
            bytes.Add(2);
            bytes.Add(3);
            bytes.Add(4);
            bytes.Add(5);
            bytes.Add(6);
            bytes.Add(7);
            bytes.Add(8);
            bytes.Add(9);
            bytes.Add(10);

            
            
            
            var data  = new MyReplicatedData()
            {
                MyInnerStruct = new MyInnerStruct(){A = 999, B = -999},
                Data = bytes,
                ActionCommand = new ActionCommand(new CommandThrowItem(new float3(99.99f,99.98f, 99.97f)))
            };
            
            // interop size
            var size = Marshal.SizeOf(data);
            var size2 = Marshal.SizeOf(data.ActionCommand);
            var size3 = Marshal.SizeOf(data.Data);
            var size4 = Marshal.SizeOf(data.MyInnerStruct);
            
            var size5 = Marshal.SizeOf(new float3());

            var serializedBytes = new Data512Bytes();
            SerializationUtils.Serialize(data, ref serializedBytes);
            
            /*
                public static unsafe T Deserialize<T, TDataBytes>(TDataBytes serializedData) 
                    where T : struct where TDataBytes : unmanaged, IDataBytes
                {
                    var ptr = UnsafeUtility.AddressOf(ref serializedData);
                    UnsafeUtility.CopyPtrToStructure(ptr, out T result);
                    return result;
                }
             */
            
            var data2 = SerializationUtils.Deserialize<MyReplicatedData, Data512Bytes>(ref serializedBytes);
            // Q: why do I have to explicitly specify the type "Data512Bytes" here?
            // A: because the compiler can't infer the type from the method signature
            // Q: why don't I already provide that information via ref serializedBytes paramater alone?
            // A: well, I do, but the compiler doesn't know that the type of serializedBytes is Data512Bytes
            
            
            int a = 3;
        }

        [MenuItem("DefenderGame/Tests/3125asd132asd")]
        public static void FixedListToDataBytesTest()
        {
            FixedList32Bytes<int> fixedList = new FixedList32Bytes<int>();
            fixedList.Add(1);
            fixedList.Add(2);
            fixedList.Add(3);
            fixedList.Add(4);
            fixedList.Add(5);
            fixedList.Add(6);
            
            
            var dataBytes = new Data32Bytes();
            
            
            SerializationUtils.Serialize(fixedList, ref dataBytes);
            
            var fixedList2 = SerializationUtils.Deserialize<FixedList32Bytes<int>, Data32Bytes>(ref dataBytes);

            var a = 3;
        }
    }
    
    public struct MyReplicatedData
    {
        public MyInnerStruct MyInnerStruct;
        public FixedString64Bytes Data;

        public ActionCommand ActionCommand;
        //[GhostField] public SampleVariant SampleVariant;
        //[GhostField] public MyUnion MyUnion;
    }

    [GhostComponent]
    public struct MyReplicatedComponent : IComponentData
    {
        [GhostField] public MyInnerStruct MyInnerStruct;
        [GhostField] public FixedString64Bytes Data;
        //[GhostField] public SampleVariant SampleVariant;
        //[GhostField] public MyUnion MyUnion;
    }
    
    public struct MyInnerStruct
    {
        public int A;
        public int B;
        //public MyUnion MyUnion;
    }
    
    [StructLayout(LayoutKind.Explicit)]
    public struct MyUnion
    {
        [FieldOffset(0)] public int Item1;
        [FieldOffset(0)] public float3 Item2;
        [FieldOffset(0)] public FixedBytes16 Item3;
    }
    
    
    [ValueVariant]
    public readonly partial struct SampleVariant: IValueVariant<SampleVariant, int> { }
    
    [ValueVariant]
    public readonly partial struct SampleVariant2: IValueVariant<SampleVariant2, SampleVariant, long> { }
#endif
}
