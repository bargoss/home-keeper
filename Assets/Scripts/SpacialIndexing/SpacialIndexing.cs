﻿using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using WaterGame.Components;

namespace SpacialIndexing
{
    
    public struct GridContent
    {
        //private FixedList4096Bytes<Entity>  m_Elements;
        private FixedList64Bytes<Entity> m_Elements;

        public void Add(Entity item)
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

        public void Remove(Entity item)
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

        public FixedList64Bytes<Entity> GetItems()
        {
            return m_Elements;
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
    
    public struct SpacialPartitioning
    {
        private readonly float m_GridSize;
        
        private NativeHashMap<int2, GridContent> m_Grids;
        private NativeHashMap<Entity, GridBoundingBox> m_ObjectGridBoundingBoxes;

        public SpacialPartitioning(float gridSze, Allocator allocator = Allocator.Temp)
        {
            m_GridSize = gridSze;
            m_Grids = new NativeHashMap<int2, GridContent>(50, allocator);
            m_ObjectGridBoundingBoxes = new NativeHashMap<Entity, GridBoundingBox>(100, allocator);
        }
        
        public void GetGrids(ref NativeList<GridContent> list)
        {
            foreach (var grid in m_Grids)
            {
                list.Add(grid.Value);
            }
        }

        public bool TryGetObject(Entity item, out GridBoundingBox gridBoundingBox)
        {
            return m_ObjectGridBoundingBoxes.TryGetValue(item, out gridBoundingBox);
        }

        public void AddPoint(Entity item, float3 position)
        {
            AddBox(item, position, position);
        }

        public void AddBox(Entity item, float3 startCorner, float3 endCorner)
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

        public void RemoveWithId(Entity item)
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

        public void AddCircle(Entity item, float3 center, float radius)
        {
            var halfSize = new float3(radius, radius, radius);
            var boxStartCorner = center - halfSize;
            var boxEndCorner = center + halfSize;

            AddBox(item, boxStartCorner, boxEndCorner);
        }

        
        public IEnumerable<Entity> GetNeighbours(float3 position)
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

        public void OverlapCircle(float3 center, float radius, ref NativeList<Entity> buffer)
        {
            var halfSize = new float3(radius, radius, radius);
            var boxStartCorner = center - halfSize;
            var boxEndCorner = center + halfSize;

            OverlapBox(boxStartCorner, boxEndCorner, ref buffer);
        }

        public void OverlapBox(float3 startCorner, float3 endCorner, ref NativeList<Entity> buffer)
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


        public void GetAllNeighbours(ref NativeList<EntityPair> buffer)
        {
            
            //var neighbourDeltas = new[] { (1, 0), (1, 1), (0, 1), (-1, 1) };
            //var neighbourDeltas = new NativeArray<MyPair<int>>(new MyPair<int>[] { new(1, 0), new(1, 1), new(0, 1), new(-1, 1) }, Allocator.Temp);
            var neighbourDeltas = new FixedList512Bytes<IntPair>
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
                            buffer.Add(new EntityPair(myGridContent.GetItems()[i], myGridContent.GetItems()[j]));
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
                                    buffer.Add(new EntityPair(myElement, neighbourElement));
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
    }


#if UNITY_EDITOR

// Unit tests
    /*
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
    */
#endif
}
