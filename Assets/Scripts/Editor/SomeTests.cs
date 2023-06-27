using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;


namespace Editor
{
    public static class SomeTests
    {
        // create a menu item
        [MenuItem("Tools/SomeTests/asda2313dxa")]
        public static void Test0()
        {
            //var a = new MyStructA();
            //a.State = null;
        }
    }

    public struct CircularArea
    {
        public float2 Position;
        public float Radius;
    }

    public struct TileCoordinate
    {
        public int X, Y;
    }
public interface IPawnTask
{
    public class Mine : IPawnTask
    {
        public CircularArea Area;
    }
    public class Haul : IPawnTask
    {
        public CircularArea SourceArea;
        public CircularArea DestinationArea;
    }
    public class HaulToChest : IPawnTask
    {
        public CircularArea SourceArea;
        public Entity DestinationChest;
    }
    public class CraftItem : IPawnTask
    {
        public int RecipeId;
        public int Quantity;
    }
    public class WanderAround : IPawnTask
    {
        public float Duration;
        FixedList128Bytes<TileCoordinate> a;
        
        public void Test()
        {
            
        }
    }

    public class MyManagedComponent : IComponentData
    {
        public List<int> MyStuff = new();
    }
    public struct MyUnmanagedComponent : IComponentData
    {
        public int A;
        public int B;
        public float C;
    }
    
    public partial struct EditorTestSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (myManagedComponentRO, myUnmanagedComponent) in SystemAPI.Query<MyManagedComponent, MyUnmanagedComponent>())
            {
                
            }
        }
    }
}
    
}