using System.Collections.Generic;
using System.Linq;
using Unity.Assertions;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;

namespace SpacialIndexing
{
    public struct GridContent<T> where T : unmanaged
    {
        private FixedList512Bytes<T> m_Elements;

        public void Add(T item)
        {
            m_Elements.Add(item);
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

        public FixedList512Bytes<T> GetItems()
        {
            return m_Elements;
        }
    }

    public struct GridBoundingBox
    {
        public (int, int) StartCorner { get; }
        public (int, int) EndCorner { get; }

        public GridBoundingBox((int, int) startCorner, (int, int) endCorner)
        {
            StartCorner = startCorner;
            EndCorner = endCorner;
        }
    }

    public class SpacialPartitioning<T> where T : unmanaged
    {
        private readonly float m_GridSize;
        private readonly Dictionary<(int, int), GridContent<T>> m_Grids;
        private readonly Dictionary<T, GridBoundingBox> m_ObjectGridBoundingBoxes;

        public SpacialPartitioning(float gridSze)
        {
            m_GridSize = gridSze;
            m_Grids = new Dictionary<(int, int), GridContent<T>>();
            m_ObjectGridBoundingBoxes = new Dictionary<T, GridBoundingBox>();
        }
        
        public List<GridContent<T>> GetGrids()
        {
            return m_Grids.Values.ToList();
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

            (int x0, int y0) = GetGrid(startCorner);
            (int x1, int y1) = GetGrid(endCorner);

            for (int i = x0; i <= x1; i++)
            {
                for (int j = y0; j <= y1; j++)
                {
                    var gridKey = (i, j);

                    m_Grids.TryGetValue(gridKey, out var gridContent);
                    gridContent.Add(item);
                    m_Grids[gridKey] = gridContent;
                }
            }
            
            m_ObjectGridBoundingBoxes.Add(item, new GridBoundingBox((x0, y0), (x1, y1)));
        }

        public void RemoveWithId(T item)
        {
            if (m_ObjectGridBoundingBoxes.TryGetValue(item, out var gridBoundingBox))
            {
                for (int i = gridBoundingBox.StartCorner.Item1; i <= gridBoundingBox.EndCorner.Item1; i++)
                {
                    for (int j = gridBoundingBox.StartCorner.Item2; j <= gridBoundingBox.EndCorner.Item2; j++)
                    {
                        var gridKey = (i, j);
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
            var (x, y) = GetGrid(position);
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (m_Grids.TryGetValue((x + i, y + j), out var gridContent))
                    {
                        foreach (var item in gridContent.GetItems())
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        public void OverlapCircle(float3 center, float radius, List<T> buffer)
        {
            var halfSize = new float3(radius, radius, radius);
            var boxStartCorner = center - halfSize;
            var boxEndCorner = center + halfSize;

            OverlapBox(boxStartCorner, boxEndCorner, buffer);
        }

        public void OverlapBox(float3 startCorner, float3 endCorner, List<T> buffer)
        {
            var (x0, y0) = GetGrid(startCorner);
            var (x1, y1) = GetGrid(endCorner);
            buffer.Clear();

            for (int i = x0; i <= x1; i++)
            {
                for (int j = y0; j <= y1; j++)
                {
                    if (m_Grids.TryGetValue((i, j), out var gridContent))
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

        public void GetAllNeighbours(List<(T, T)> buffer)
        {
            var neighbourDeltas = new[] { (1, 0), (1, 1), (0, 1), (-1, 1) };
            buffer.Clear();

            foreach (var myGridKey in m_Grids.Keys)
            {
                if (m_Grids.TryGetValue(myGridKey, out var myGridContent))
                {
                    for (int i = 0; i < myGridContent.GetItems().Length; i++)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            buffer.Add((myGridContent.GetItems()[i], myGridContent.GetItems()[j]));
                        }
                    }

                    foreach (var (i, j) in neighbourDeltas)
                    {
                        var neighbourGridKey = (myGridKey.Item1 + i, myGridKey.Item2 + j);
                        if (m_Grids.TryGetValue(neighbourGridKey, out var neighbourGridContent))
                        {
                            foreach (var myElement in myGridContent.GetItems())
                            {
                                foreach (var neighbourElement in neighbourGridContent.GetItems())
                                {
                                    buffer.Add((myElement, neighbourElement));
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            m_Grids.Clear();
            m_ObjectGridBoundingBoxes.Clear();
        }

        private (int, int) GetGrid(float3 position)
        {
            int x = (int)math.floor(position.x / m_GridSize);
            int y = (int)math.floor(position.y / m_GridSize);
            return (x, y);
        }
    }


#if UNITY_EDITOR

// Unit tests
    public static class SpacialPartitioningTests
    {
        [MenuItem("Tests/23151231")]
        public static void TestAdd()
        {
            var spacialPartitioning = new SpacialPartitioning<int>(10.0f);
            spacialPartitioning.AddPoint(1, new float3(0.0f, 0.0f,0.0f));
            spacialPartitioning.AddPoint(2, new float3(0.0f, 0.0f,0.0f));
            spacialPartitioning.AddPoint(2, new float3(9.9f, 0.0f,0.0f));

            Assert.AreEqual(1, spacialPartitioning.GetGrids().Count);
            Assert.AreEqual(2, spacialPartitioning.GetGrids()[0].GetItems().Length);

            spacialPartitioning.AddPoint(3, new float3(10.0f, 0.0f,0.0f));
            Assert.AreEqual(2, spacialPartitioning.GetGrids().Count);
            Assert.AreEqual(2, spacialPartitioning.GetGrids()[0].GetItems().Length);
            Assert.AreEqual(1, spacialPartitioning.GetGrids()[^1].GetItems().Length);
        }

        [MenuItem("Tests/6345312")]
        public static void TestSpacialPartitioning()
        {
            var partitioning = new SpacialPartitioning<int>(10.0f);
            partitioning.AddPoint(1, new float3(5.0f, 5.0f,0.0f));
            partitioning.AddPoint(2, new float3(25.0f, 25.0f,0.0f));
            partitioning.AddPoint(3, new float3(-5.0f, -5.0f,0.0f));
            partitioning.AddPoint(4, new float3(-25.0f, -25.0f,0.0f));

            var buffer = new List<int>();
            partitioning.OverlapBox(new float3(0.0f, 0.0f,0.0f), new float3(11.0f, 11.0f,0.0f), buffer);
            Assert.AreEqual(1, buffer.Count);
            Assert.AreEqual(1, buffer[0]);

            partitioning.OverlapBox(new float3(-11.0f, -11.0f,0.0f), new float3(11.0f, 11.0f,0.0f), buffer);
            Assert.AreEqual(2, buffer.Count);

            buffer.Sort();
            Assert.AreEqual(1, buffer[0]);
            Assert.AreEqual(3, buffer[1]);
        }

        [MenuItem("Tests/72334532")]
        public static void TestAddBox()
        {
            var partitioning = new SpacialPartitioning<int>(10.0f);
            partitioning.AddBox(1, new float3(9.0f, 23.0f,0.0f), new float3(21.0f, 25.0f,0.0f));

            var buffer = new List<int>();
            partitioning.OverlapBox(new float3(0.0f, 0.0f,0.0f), new float3(11.0f, 11.0f,0.0f), buffer);
            Assert.AreEqual(0, buffer.Count);

            partitioning.OverlapBox(new float3(0.0f, 0.0f,0.0f), new float3(21.0f, 25.0f,0.0f), buffer);
            Assert.AreEqual(1, buffer.Count);
            Assert.AreEqual(1, buffer[0]);
        }

        [MenuItem("Tests/8324674")]
        public static void TestAddSameKeyDifferentPosition()
        {
            var spacialPartitioning = new SpacialPartitioning<int>(10.0f);
            spacialPartitioning.AddPoint(1, new float3(0.0f, 0.0f, 0.0f));
            spacialPartitioning.AddPoint(2, new float3(0.0f, 0.0f, 0.0f));

            spacialPartitioning.AddPoint(2, new float3(9.9f, 0.0f, 0));

            Assert.AreEqual(1, spacialPartitioning.GetGrids().Count);
            Assert.AreEqual(2, spacialPartitioning.GetGrids()[0].GetItems().Length);

            spacialPartitioning.AddPoint(3, new float3(10.0f, 0.0f, 0));
            Assert.AreEqual(2, spacialPartitioning.GetGrids().Count);
            Assert.AreEqual(2, spacialPartitioning.GetGrids()[0].GetItems().Length);
            Assert.AreEqual(1, spacialPartitioning.GetGrids()[^1].GetItems().Length);
        }
    }

}
#endif