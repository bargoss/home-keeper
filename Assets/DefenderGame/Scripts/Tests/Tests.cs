using DefaultNamespace;
using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.Systems;
using dotVariant;
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

        [MenuItem("DefenderGame/Tests/TestTurretUpdate2")]
        public static void DotVariantTests()
        {
            var variant = new dotVariant._G.DefenderGame.Scripts.Tests.MyGrid()

        }
        
        [Variant]
        public partial struct MyGrid
        {
            static partial void VariantOf(Turret turret, Magazine magazine, AmmoBox ammoBox);
        }
        
        public struct Turret1
        {
            public float LastShot;
            public float3 AimDirection;
            public bool HasMagazine;
            public bool MagazineChanged;
            public Magazine1 Magazine;
        }
        
        public struct Magazine1
        {
            public int CurrentAmmo;
            public int MaxAmmo;
            public bool AmmoCountChanged;
        }
        
        public struct AmmoBox1
        {
            public int AmmoCount;
            public bool AmmoCountChanged;
        }
    }
#endif
}