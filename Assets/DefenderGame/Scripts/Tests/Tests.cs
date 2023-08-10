using System;
using DefaultNamespace;
using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.Systems;
using Unity.Collections;
using Unity.Entities;
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
        
        [MenuItem("DefenderGame/Tests/212412")]
        public static void MyTest()
        {
            var a = (new MyChild0().ToMyBase());
            switch (a.CurrentTypeId)
            {
                case MyBase.TypeId.MyChild0:
                    
                case MyBase.TypeId.MyChild1:
                case MyBase.TypeId.MyChild2:
                case MyBase.TypeId.MyChild3:
                default:
                    break;
            }
            
            DynamicBuffer<byte> buffer = new DynamicBuffer<byte>();
        }
        
        
    }
    
    [ValueVariant]
    public readonly partial struct SampleVariant: IValueVariant<SampleVariant, int, long, float> { }
    
    [ValueVariant]
    public readonly partial struct SampleVariant2: IValueVariant<SampleVariant2, SampleVariant, long> { }
    
    
    [PolymorphicStruct]
    public interface IMyBase{}
    
    public partial struct MyChild0 : IMyBase
    {
        public float MyFloat;
        public float MyInt;
    }
    public partial struct MyChild1 : IMyBase
    {
        public float MyInt0;
        public float MyInt1;
        public float MyInt2;
    }
    public partial struct MyChild2 : IMyBase
    {
        public float MyFloat;
    }
    public partial struct MyChild3 : IMyBase
    {
        public MyList MyList;
    }

    public struct MyList
    {
        public int MyInt0;
        public int MyInt1;
        public int MyInt2;
        public int MyInt3;
    }
    
    
#endif
}