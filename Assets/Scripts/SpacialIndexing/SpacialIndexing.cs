using System;
using System.Collections.Generic;
using System.Linq;
using HomeKeeper.Systems;
using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using WaterGame.Components;

namespace SpacialIndexing
{
    
    public struct GridContent<T> where T : unmanaged, IEquatable<T>
    {
        //private FixedList4096Bytes<T>  m_Elements;
        private FixedList64Bytes<T> m_Elements;

        public void Add(T item)
        {
            
            if (m_Elements.Length < m_Elements.Capacity)
            {
                m_Elements.Add(item);
            }
            else
            {
#if UNITY_EDITOR
                //Debug.Log("GridContent is full");
#endif
            }
            
        }

        public void Remove(T item)
        {
            for (var i = 0; i < m_Elements.Length; i++)
            {
                if (m_Elements[i].Equals(item))
                {
                    m_Elements.RemoveAtSwapBack(i);
                    return;
                }
            }
        }

        public FixedList64Bytes<T> GetItems()
        {
            return m_Elements;
        }
        
        public int Length()
        {
            return m_Elements.Length;
        }
        
        public T Get(int index)
        {
            return m_Elements[index];
        }
    }

    public struct GridBoundingBox
    {
        public int2 StartCorner { get; }
        public int2 EndCorner { get; }

        public GridBoundingBox(int2 startCorner, int2 endCorner)
        {
            StartCorner = startCorner;
            EndCorner = endCorner;
        }
    }

    public struct SpacialPartitioning<T> : IDisposable where T : unmanaged, IEquatable<T>, IComparable<T>
    {
        private readonly float m_GridSize;

        private NativeHashMap<int2, GridContent<T>> m_Grids;
        private NativeHashMap<T, GridBoundingBox> m_ObjectGridBoundingBoxes;

        public SpacialPartitioning(float gridSze, Allocator allocator = Allocator.Temp)
        {
            m_GridSize = gridSze;
            m_Grids = new NativeHashMap<int2, GridContent<T>>(50, allocator);
            m_ObjectGridBoundingBoxes = new NativeHashMap<T, GridBoundingBox>(100, allocator);
        }

        public void GetGrids(ref NativeList<GridContent<T>> list)
        {
            foreach (var grid in m_Grids)
            {
                list.Add(grid.Value);
            }
        }

        public bool TryGetObject(T item, out GridBoundingBox gridBoundingBox)
        {
            return m_ObjectGridBoundingBoxes.TryGetValue(item, out gridBoundingBox);
        }

        public void AddPoint(T item, float3 position)
        {
            AddBox(item, position, position);
        }

        public void AddBox(T item, float3 startCorner, float3 endCorner)
        {
            if (m_ObjectGridBoundingBoxes.TryGetValue(item, out _))
            {
                RemoveWithId(item);
            }

            var g0 = GetGrid(startCorner);
            var g1 = GetGrid(endCorner);


            for (int i = g0.x; i <= g1.x; i++)
            {
                for (int j = g0.y; j <= g1.y; j++)
                {
                    //var gridKey = (i, j);
                    var gridKey = new int2(i, j);

                    m_Grids.TryGetValue(gridKey, out var gridContent);
                    gridContent.Add(item);
                    m_Grids[gridKey] = gridContent;
                }
            }

            //m_ObjectGridBoundingBoxes.Add(item, new GridBoundingBox((x0, y0), (x1, y1)));
            m_ObjectGridBoundingBoxes.Add(item, new GridBoundingBox(new int2(g0.x, g0.y), new int2(g1.x, g1.y)));
        }

        public void RemoveWithId(T item)
        {
            if (m_ObjectGridBoundingBoxes.TryGetValue(item, out var gridBoundingBox))
            {
                for (int i = gridBoundingBox.StartCorner.x; i <= gridBoundingBox.EndCorner.x; i++)
                {
                    for (int j = gridBoundingBox.StartCorner.y; j <= gridBoundingBox.EndCorner.y; j++)
                    {
                        //var gridKey = (i, j);
                        var gridKey = new int2(i, j);
                        if (m_Grids.TryGetValue(gridKey, out var gridContent))
                        {
                            gridContent.Remove(item);

                            if (gridContent.GetItems().Length == 0)
                            {
                                m_Grids.Remove(gridKey);
                            }
                            else
                            {
                                m_Grids[gridKey] = gridContent;
                            }
                        }
                    }
                }

                m_ObjectGridBoundingBoxes.Remove(item);
            }
        }

        public void AddCircle(T item, float3 center, float radius)
        {
            var halfSize = new float3(radius, radius, radius);
            var boxStartCorner = center - halfSize;
            var boxEndCorner = center + halfSize;

            AddBox(item, boxStartCorner, boxEndCorner);
        }


        public IEnumerable<T> GetNeighbours(float3 position)
        {
            //var (x, y) = GetGrid(position);
            var g = GetGrid(position);
            var x = g.x;
            var y = g.y;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    //if (m_Grids.TryGetValue((x + i, y + j), out var gridContent))
                    if (m_Grids.TryGetValue(new int2(x + i, y + j), out var gridContent))
                    {
                        foreach (var item in gridContent.GetItems())
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        public void OverlapCircle(float3 center, float radius, ref NativeList<T> buffer)
        {
            var halfSize = new float3(radius, radius, radius);
            var boxStartCorner = center - halfSize;
            var boxEndCorner = center + halfSize;

            OverlapBox(boxStartCorner, boxEndCorner, ref buffer);
        }

        public void OverlapBox(float3 startCorner, float3 endCorner, ref NativeList<T> buffer)
        {
            //var (x0, y0) = GetGrid(startCorner);
            var g0 = GetGrid(startCorner);
            var x0 = g0.x;
            var y0 = g0.y;
            //var (x1, y1) = GetGrid(endCorner);
            var g1 = GetGrid(endCorner);
            var x1 = g1.x;
            var y1 = g1.y;
            buffer.Clear();

            for (int i = x0; i <= x1; i++)
            {
                for (int j = y0; j <= y1; j++)
                {
                    //if (m_Grids.TryGetValue((i, j), out var gridContent))
                    if (m_Grids.TryGetValue(new int2(i, j), out var gridContent))
                    {
                        foreach (var item in gridContent.GetItems())
                        {
                            if (!buffer.Contains(item))
                            {
                                buffer.Add(item);
                            }
                        }
                    }
                }
            }

            buffer.Sort();
        }

        public int GetGridCount()
        {
            return m_Grids.Count();
        }

        public NativeArray<int2> GetGridKeyArray(Allocator allocator)
        {
            var keys = m_Grids.GetKeyArray(allocator);
            return keys;
        }



        public void GetAllNeighbours(ref NativeList<MyPair<T>> buffer)
        {
            var neighbourDeltas = new FixedList512Bytes<MyPair<int>>
            {
                new(1, 0),
                new(1, 1),
                new(0, 1),
                new(-1, 1)
            };

            buffer.Clear();
            var keys = m_Grids.GetKeyArray(Allocator.Temp);
            foreach (var myGridKey in keys)
            {
                if (m_Grids.TryGetValue(myGridKey, out var myGridContent))
                {
                    for (int i = 0; i < myGridContent.GetItems().Length; i++)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            buffer.Add(new MyPair<T>(myGridContent.GetItems()[i], myGridContent.GetItems()[j]));
                        }
                    }

                    foreach (var pair in neighbourDeltas)
                    {
                        var i = pair.A;
                        var j = pair.B;

                        var neighbourGridKey = new int2(myGridKey.x + i, myGridKey.y + j);
                        if (m_Grids.TryGetValue(neighbourGridKey, out var neighbourGridContent))
                        {
                            foreach (var myElement in myGridContent.GetItems())
                            {
                                foreach (var neighbourElement in neighbourGridContent.GetItems())
                                {
                                    buffer.Add(new MyPair<T>(myElement, neighbourElement));
                                }
                            }
                        }
                    }
                }
            }

            keys.Dispose();
        }

        public void Clear()
        {
            m_Grids.Clear();
            m_ObjectGridBoundingBoxes.Clear();
        }

        private int2 GetGrid(float3 position)
        {
            int x = (int)math.floor(position.x / m_GridSize);
            int y = (int)math.floor(position.y / m_GridSize);
            return new int2(x, y);
        }

        public void Dispose()
        {
            m_Grids.Dispose();
            m_ObjectGridBoundingBoxes.Dispose();
        }

        public void GetNeighboursFromGridRange(int startGridIndex, int endGridIndex, ref NativeList<MyPair<T>> buffer,
            ref NativeArray<int2> gridKeys)
        {
            var neighbourDeltas = new FixedList512Bytes<MyPair<int>>
            {
                new(1, 0),
                new(1, 1),
                new(0, 1),
                new(-1, 1)
            };

            foreach (var myGridKey in gridKeys)
            {
                if (m_Grids.TryGetValue(myGridKey, out var myGridContent))
                {
                    for (int i = 0; i < myGridContent.GetItems().Length; i++)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            buffer.Add(new MyPair<T>(myGridContent.GetItems()[i], myGridContent.GetItems()[j]));
                        }
                    }

                    foreach (var pair in neighbourDeltas)
                    {
                        var i = pair.A;
                        var j = pair.B;

                        var neighbourGridKey = new int2(myGridKey.x + i, myGridKey.y + j);
                        if (m_Grids.TryGetValue(neighbourGridKey, out var neighbourGridContent))
                        {
                            foreach (var myElement in myGridContent.GetItems())
                            {
                                foreach (var neighbourElement in neighbourGridContent.GetItems())
                                {
                                    buffer.Add(new MyPair<T>(myElement, neighbourElement));
                                }
                            }
                        }
                    }
                }
            }
        }

        public struct SolveCollisionsJob : IJobParallelFor
        {
            [WriteOnly] public NativeParallelMultiHashMap<Entity, float3>.ParallelWriter Forces;
            [ReadOnly] public NativeArray<int2> GridKeys;
            [ReadOnly] public int GridKeysCount;
            [ReadOnly] public SpacialPartitioning<Entity> SpacialPartitioning;
            [ReadOnly] public ComponentLookup<PhysicsVelocity> PhysicsVelocityLookup;
            [ReadOnly] public ComponentLookup<LocalToWorld> LocalToWorldLookup;
            [ReadOnly] public WaterGameConfig Config;
            [ReadOnly] public float DeltaTime;

            private void SolveCollision(MyPair<Entity> pair)
            {
                var posA = LocalToWorldLookup[pair.A].Position;
                var velA = PhysicsVelocityLookup[pair.A].Linear;
                var posB = LocalToWorldLookup[pair.B].Position;
                var velB = PhysicsVelocityLookup[pair.B].Linear;

                var force = ParticleCollisionSolverSystem.CalculateCollisionForce(
                    new ParticleCollisionSolverSystem.ForcePoint()
                    {
                        Position = posA,
                        Velocity = velA,
                        OuterRadius = Config.OuterRadius,
                        InnerRadius = Config.InnerRadius
                    },
                    new ParticleCollisionSolverSystem.ForcePoint()
                    {
                        Position = posB,
                        Velocity = velB,
                        OuterRadius = Config.OuterRadius,
                        InnerRadius = Config.InnerRadius
                    },
                    DeltaTime,
                    Config.PushForce,
                    Config.Viscosity
                );

                Forces.Add(pair.A, force);
                Forces.Add(pair.B, -force);
            }

            public void Execute(int index)
            {
                var neighbourDeltas = new FixedList512Bytes<MyPair<int>>
                {
                    new(1, 0),
                    new(1, 1),
                    new(0, 1),
                    new(-1, 1)
                };

                var myGridKey = GridKeys[index];
                var myGridContent = SpacialPartitioning.m_Grids[myGridKey];
                {
                    for (int i = 0; i < myGridContent.Length(); i++)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            SolveCollision(new MyPair<Entity>(myGridContent.Get(i), myGridContent.Get(j)));
                        }
                    }

                    foreach (var pair in neighbourDeltas)
                    {
                        var i = pair.A;
                        var j = pair.B;

                        var neighbourGridKey = new int2(myGridKey.x + i, myGridKey.y + j);
                        if (SpacialPartitioning.m_Grids.TryGetValue(neighbourGridKey, out var neighbourGridContent))
                        {
                            for (var myElementIndex = 0; myElementIndex < myGridContent.Length(); myElementIndex++)
                            {
                                var myElement = myGridContent.Get(myElementIndex);
                                for (var neighbourElementIndex = 0;
                                     neighbourElementIndex < neighbourGridContent.Length();
                                     neighbourElementIndex++)
                                {
                                    var neighbourElement = neighbourGridContent.Get(neighbourElementIndex);
                                    SolveCollision(new MyPair<Entity>(myElement, neighbourElement));
                                }
                            }
                        }
                    }
                }
            }
        }
    }




#if UNITY_EDITOR

// Unit tests
    public static class SpacialPartitioningTests
    {
        private static int GetCount(SpacialPartitioning<int> spacialPartitioning)
        {
            var list = new NativeList<GridContent<int>>(Allocator.Temp);
            spacialPartitioning.GetGrids(ref list);
            
            var len = list.Length;
            list.Dispose();
            return len;
        }
        private static List<GridContent<int>> GetGrids(SpacialPartitioning<int> spacialPartitioning)
        {
            var nativeList = new NativeList<GridContent<int>>(Allocator.Temp);
            spacialPartitioning.GetGrids(ref nativeList);
            
            var list = new List<GridContent<int>>();
            for (int i = 0; i < nativeList.Length; i++)
            {
                list.Add(nativeList[i]);
            }
            nativeList.Dispose();
            return list;
        }
        
        
        [MenuItem("Tests/23151231")]
        public static void TestAdd()
        {
            var spacialPartitioning = new SpacialPartitioning<int>(10.0f);
            spacialPartitioning.AddPoint(1, new float3(0.0f, 0.0f,0.0f));
            spacialPartitioning.AddPoint(2, new float3(0.0f, 0.0f,0.0f));
            spacialPartitioning.AddPoint(2, new float3(9.9f, 0.0f,0.0f));

            Assert.AreEqual(1, GetGrids(spacialPartitioning).Count);
            Assert.AreEqual(2, GetGrids(spacialPartitioning)[0].GetItems().Length);

            spacialPartitioning.AddPoint(3, new float3(10.0f, 0.0f,0.0f));
            Assert.AreEqual(2, GetGrids(spacialPartitioning).Count);
            Assert.AreEqual(2, GetGrids(spacialPartitioning)[0].GetItems().Length);
            Assert.AreEqual(1, GetGrids(spacialPartitioning)[^1].GetItems().Length);
        }

        [MenuItem("Tests/6345312")]
        public static void TestSpacialPartitioning()
        {
            var partitioning = new SpacialPartitioning<int>(10.0f);
            partitioning.AddPoint(1, new float3(5.0f, 5.0f,0.0f));
            partitioning.AddPoint(2, new float3(25.0f, 25.0f,0.0f));
            partitioning.AddPoint(3, new float3(-5.0f, -5.0f,0.0f));
            partitioning.AddPoint(4, new float3(-25.0f, -25.0f,0.0f));

            var buffer = new NativeList<int>();
            partitioning.OverlapBox(new float3(0.0f, 0.0f,0.0f), new float3(11.0f, 11.0f,0.0f), ref buffer);
            Assert.AreEqual(1, buffer.Length);
            Assert.AreEqual(1, buffer[0]);

            partitioning.OverlapBox(new float3(-11.0f, -11.0f,0.0f), new float3(11.0f, 11.0f,0.0f), ref buffer);
            Assert.AreEqual(2, buffer.Length);

            buffer.Sort();
            Assert.AreEqual(1, buffer[0]);
            Assert.AreEqual(3, buffer[1]);
        }

        [MenuItem("Tests/72334532")]
        public static void TestAddBox()
        {
            var partitioning = new SpacialPartitioning<int>(10.0f);
            partitioning.AddBox(1, new float3(9.0f, 23.0f,0.0f), new float3(21.0f, 25.0f,0.0f));

            var buffer = new NativeList<int>();
            partitioning.OverlapBox(new float3(0.0f, 0.0f,0.0f), new float3(11.0f, 11.0f,0.0f), ref buffer);
            Assert.AreEqual(0, buffer.Length);

            partitioning.OverlapBox(new float3(0.0f, 0.0f,0.0f), new float3(21.0f, 25.0f,0.0f), ref buffer);
            Assert.AreEqual(1, buffer.Length);
            Assert.AreEqual(1, buffer[0]);
        }

        [MenuItem("Tests/8324674")]
        public static void TestAddSameKeyDifferentPosition()
        {
            var spacialPartitioning = new SpacialPartitioning<int>(10.0f);
            spacialPartitioning.AddPoint(1, new float3(0.0f, 0.0f, 0.0f));
            spacialPartitioning.AddPoint(2, new float3(0.0f, 0.0f, 0.0f));

            spacialPartitioning.AddPoint(2, new float3(9.9f, 0.0f, 0));

            Assert.AreEqual(1, GetGrids(spacialPartitioning).Count);
            Assert.AreEqual(2, GetGrids(spacialPartitioning)[0].GetItems().Length);

            spacialPartitioning.AddPoint(3, new float3(10.0f, 0.0f, 0));
            Assert.AreEqual(2, GetGrids(spacialPartitioning).Count);
            Assert.AreEqual(2, GetGrids(spacialPartitioning)[0].GetItems().Length);
            Assert.AreEqual(1, GetGrids(spacialPartitioning)[^1].GetItems().Length);
        }
    }
#endif
}
