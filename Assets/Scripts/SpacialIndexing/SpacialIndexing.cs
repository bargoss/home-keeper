using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using RunnerGame.Scripts.ECS.Components;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using WaterGame.Authoring;
using WaterGame.Components;
using ParticleCollisionSolverSystem = WaterGame.Systems.ParticleCollisionSolverSystem;

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
        
        private float GetAxis0(float3 position)
        {
            return position.x;
        }
        private float GetAxis1(float3 position)
        {
            return position.z;
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

        //public void OverlapCircle(float3 center, float radius, ref FixedList4096Bytes<T> buffer)
        public void OverlapCircle(float3 center, float radius, ref NativeArraySlice<T> buffer)
        {
            var halfSize = new float3(radius, radius, radius);
            var boxStartCorner = center - halfSize;
            var boxEndCorner = center + halfSize;

            OverlapBox(boxStartCorner, boxEndCorner, ref buffer);
        }

        public void OverlapBox(float3 startCorner, float3 endCorner, ref NativeArraySlice<T> buffer)
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
                        var items = gridContent.GetItems();
                        for (var index = 0; index < items.Length; index++)
                        {
                            var item = items[index];
                            buffer.Add(item);
                        }
                    }
                }
            }

            // remove duplicate items with for loop
            for (int i = 0; i < buffer.Length; i++)
            {
                var item = buffer[i];
                for (int j = i + 1; j < buffer.Length; j++)
                {
                    if (buffer[j].Equals(item))
                    {
                        buffer.RemoveAtSwapBack(j);
                        j--;
                    }
                }
            }
            
            
            
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
            //int x = (int)math.floor(position.x / m_GridSize);
            int x = (int)math.floor(GetAxis0(position) / m_GridSize);
            //int y = (int)math.floor(position.y / m_GridSize);
            int y = (int)math.floor(GetAxis1(position) / m_GridSize);
            
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
    }

    public struct NativeArraySlice<T> where T : unmanaged
    {
        private int m_Start;
        public int Capacity { get; private set; }
        public int Length { get; private set; }
        private NativeArray<T> m_Array;
        
        public NativeArraySlice(NativeArray<T> array, int start, int capacity)
        {
            m_Start = start;
            Length = 0;
            Capacity = capacity;
            m_Array = array;
        }
        
        public T this[int index]
        {
            get => m_Array[m_Start + index];
            set => m_Array[m_Start + index] = value;
        }
        
        public void Add(T item)
        {
            m_Array[m_Start + Length] = item;
            Length++;
        }
        
        public void Clear()
        {
            Length = 0;
        }
        
        //RemoveAtSwapBack
        public void RemoveAtSwapBack(int index)
        {
            m_Array[m_Start + index] = m_Array[m_Start + Length - 1];
            Length--;
        }
    }
    
    public partial struct SolveCollisionsJob : IJobEntity
    {
        [ReadOnly] public NativeHashMap<Entity, float3> Positions;
        [ReadOnly] public NativeHashMap<Entity, float3> Velocities;
        [ReadOnly] public WaterGameConfig Config;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public SpacialPartitioning<Entity> SpacialPartitioning;
        
        
        [NativeSetThreadIndex] public int ThreadIndex;
        
        [NativeDisableContainerSafetyRestriction] 
        [NativeDisableParallelForRestriction] 
        [NativeDisableUnsafePtrRestriction] 
        public NativeArray<Entity> EntityBuffer;
        
        private float3 SolveCollision(MyPair<Entity> pair)
        {
            var posA = Positions[pair.A];
            var velA = Velocities[pair.A];
            var posB = Positions[pair.B];
            var velB = Velocities[pair.B];

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

            force = force.ClampMagnitude(10);
            return force;
        }

        public void Execute(Entity entity, ref PhysicsVelocity physicsVelocity, in LocalToWorld localToWorld, in Particle particle)
        {
            //var overlapCircleBuffer = new NativeList<Entity>(Allocator.Temp);
            var overlapCircleBuffer = new NativeArraySlice<Entity>(EntityBuffer, ThreadIndex * 1024, 1024);
            overlapCircleBuffer.Clear();
            
            SpacialPartitioning.OverlapCircle(localToWorld.Position, Config.OuterRadius, ref overlapCircleBuffer);
            for (int i = 0; i < overlapCircleBuffer.Length; i++)
            {
                var other = overlapCircleBuffer[i];
                if (other != entity)
                {
                    var force = SolveCollision(new MyPair<Entity>(entity, other));
                    physicsVelocity.Linear += force;    
                }
            }
        }
    }
}