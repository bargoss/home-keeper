using System.Collections.Generic;
using DefaultNamespace;
using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.Systems;
using OneOf;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;

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

        [MenuItem("DefenderGame/Tests/OneOfTest")]
        public static void OneOfTests()
        {
            var grids = new List<Grid>();
            grids.Add(new Grid()
            {
                Item = new TestMagazine()
            });
        }
    }

    public struct TestTurret
    {
        public float3 Look;
        public bool Shoot;
    }
    public struct TestMagazine
    {
        public int Capacity;
        public int AmmoCount;
    }
    public struct None
    {
        
    }

    public struct Grid
    {
        public int2 GridPos;
        public OneOf<None, TestTurret, TestMagazine> Item;
    }
    
    struct Shape
    {
    }
    
#endif
}