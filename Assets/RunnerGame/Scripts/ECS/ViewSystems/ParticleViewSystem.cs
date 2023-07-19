using System.Collections.Generic;
using System.Linq;
using RunnerGame.Scripts.ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace RunnerGame.Scripts.ECS.ViewSystems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class ItemViewSystem : SystemBase
    {
        private Mesh m_Mesh;
        private List<Vector3> m_Vertices = new List<Vector3>();
        private List<int> m_Triangles = new List<int>();
        private List<Vector3> m_Normals = new List<Vector3>();
        private List<Color> m_Colors = new List<Color>();
        
        private List<Vector3>  m_ChunkVertices = new List<Vector3>();
        private List<int>  m_ChunkTriangles = new List<int>();
        private List<Vector3>  m_ChunkNormals = new List<Vector3>();
        private List<Color>  m_ChunkColors = new List<Color>();
        private void DrawTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            var normal = Vector3.Cross(b - a, c - a).normalized;
            DrawTriangle(a, b, c, normal, normal, normal, Color.white);
        }
        private void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 normalA, Vector3 normalB, Vector3 normalC, Color color)
        {
            var index = m_Vertices.Count;
            m_Vertices.Add(a);
            m_Vertices.Add(b);
            m_Vertices.Add(c);
            m_Normals.Add(normalA);
            m_Normals.Add(normalB);
            m_Normals.Add(normalC);
            m_Triangles.Add(index);
            m_Triangles.Add(index + 1);
            m_Triangles.Add(index + 2);
            m_Colors.Add(color);
            m_Colors.Add(color);
            m_Colors.Add(color);
        }
        private void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 normalA, Vector3 normalB, Vector3 normalC)
        {
            DrawTriangle(a, b, c, normalA, normalB, normalC, Color.white);
        }
        private void Draw4FacedPyramid(Vector3 center, float size)
        {
            var index = m_Vertices.Count;
            m_Vertices.Add(center + new Vector3(-size, -size, -size));
            m_Vertices.Add(center + new Vector3(size, -size, -size));
            m_Vertices.Add(center + new Vector3(size, -size, size));
            m_Vertices.Add(center + new Vector3(-size, -size, size));
            m_Vertices.Add(center + new Vector3(0, size, 0));
            m_Triangles.Add(index);
            m_Triangles.Add(index + 1);
            m_Triangles.Add(index + 2);
            m_Triangles.Add(index);
            m_Triangles.Add(index + 2);
            m_Triangles.Add(index + 3);
            m_Triangles.Add(index);
            m_Triangles.Add(index + 4);
            m_Triangles.Add(index + 1);
            m_Triangles.Add(index + 1);
            m_Triangles.Add(index + 4);
            m_Triangles.Add(index + 2);
            m_Triangles.Add(index + 2);
            m_Triangles.Add(index + 4);
            m_Triangles.Add(index + 3);
            m_Triangles.Add(index + 3);
            m_Triangles.Add(index + 4);
            m_Triangles.Add(index);
            m_Triangles.Add(index + 3);
            m_Triangles.Add(index);
            m_Triangles.Add(index + 2);
        }

        private void DrawQuad(Vector3 center, float edgeLen, Quaternion rotation, Vector3 normal, Color color)
        {
            DrawTriangle(center + rotation * new Vector3(-edgeLen, -edgeLen, 0),center + rotation * new Vector3(edgeLen, -edgeLen, 0),center + rotation * new Vector3(edgeLen, edgeLen, 0), normal, normal, normal, color);
            DrawTriangle(center + rotation * new Vector3(-edgeLen, -edgeLen, 0),center + rotation * new Vector3(edgeLen, edgeLen, 0),center + rotation * new Vector3(-edgeLen, edgeLen, 0), normal, normal, normal, color);
        }
        
        private void DrawCone(Vector3 center, Vector3 tipOffset, float baseRadius, int baseEdgeCount)
        {
            var direction = (center - (center + tipOffset)).normalized;
            var rotation = Quaternion.LookRotation(direction);
            var baseCircleVertices = new List<Vector3>();
            var tipPosition = center + tipOffset;

            // Calculate the vertices of the base circle
            for (int i = 0; i < baseEdgeCount; i++)
            {
                float angle = 2 * Mathf.PI * i / baseEdgeCount;
                float x = baseRadius * Mathf.Cos(angle);
                float y = baseRadius * Mathf.Sin(angle);
                baseCircleVertices.Add(center + rotation * new Vector3(x, y, 0));
            }

            // Draw triangles connecting the tip to the base circle vertices
            for (int i = 0; i < baseCircleVertices.Count; i++)
            {
                var v0 = baseCircleVertices[i];
                var v1 = tipPosition;
                var v2 = baseCircleVertices[(i + 1) % baseCircleVertices.Count];
                var n0 = (v0 - center).normalized;
                var n1 = (v1 - center).normalized;
                var n2 = (v2 - center).normalized;
                
                DrawTriangle(v0, v1, v2, n0, n1, n2);
            }
        }

        private void Draw()
        {
            int maxVerticesPerMesh = 65535;
            int maxTrianglesPerMesh = maxVerticesPerMesh / 3;
            
            int triangleCount = 0;
            int vertexIndexOffset = 0;


            m_Mesh.Clear();
            while (triangleCount < m_Triangles.Count/3)
            {
                int triangleCountInThisMesh = Mathf.Min(maxTrianglesPerMesh, m_Triangles.Count/3 - triangleCount);
                m_ChunkVertices.Clear();
                m_ChunkTriangles.Clear();
                m_ChunkNormals.Clear();
                m_ChunkColors.Clear();
                for (int i = 0; i < triangleCountInThisMesh; i += 1)
                {
                    m_ChunkVertices.Add(m_Vertices[triangleCount + i * 3]);
                    m_ChunkVertices.Add(m_Vertices[triangleCount + i * 3 + 1]);
                    m_ChunkVertices.Add(m_Vertices[triangleCount + i * 3 + 2]);
                    m_ChunkTriangles.Add(i * 3);
                    m_ChunkTriangles.Add(i * 3 + 1);
                    m_ChunkTriangles.Add(i * 3 + 2);
                    m_ChunkNormals.Add(m_Normals[triangleCount + i * 3]);
                    m_ChunkNormals.Add(m_Normals[triangleCount + i * 3 + 1]);
                    m_ChunkNormals.Add(m_Normals[triangleCount + i * 3 + 2]);
                    m_ChunkColors.Add(m_Colors[triangleCount + i * 3]);
                    m_ChunkColors.Add(m_Colors[triangleCount + i * 3 + 1]);
                    m_ChunkColors.Add(m_Colors[triangleCount + i * 3 + 2]);
                }
                
                //var posSum = vertices.Aggregate(Vector3.zero, (current, vertex) => current + vertex);
                //var center = posSum / vertices.Count;
                var center = Camera.main.transform.position;
                for (var i = 0; i < m_ChunkVertices.Count; i++)
                {
                    m_ChunkVertices[i] -= center;
                }
                
                m_Mesh.SetVertices(m_ChunkVertices);
                //for (var i = 0; i < vertices.Count / 3; i++)
                //{
                //    Debug.DrawLine(vertices[i * 3], vertices[i * 3 + 1], Color.red);
                //    Debug.DrawLine(vertices[i * 3 + 1], vertices[i * 3 + 2], Color.red);
                //    Debug.DrawLine(vertices[i * 3 + 2], vertices[i * 3], Color.red);
                //}
                m_Mesh.SetTriangles(m_ChunkTriangles, 0);
                m_Mesh.SetNormals(m_ChunkNormals);
                m_Mesh.SetColors(m_ChunkColors);
                m_Mesh.RecalculateBounds();
                
                Graphics.DrawMesh(m_Mesh, Matrix4x4.TRS(center, quaternion.identity, Vector3.one), GameResources.Instance.MagazineMaterial, 0);
                
                triangleCount += triangleCountInThisMesh;
            }
            
            //for (int i = 0; i < m_Triangles.Count; i += 3)
            //{
            //    vertices.Add(m_Vertices[m_Triangles[i]]);
            //    vertices.Add(m_Vertices[m_Triangles[i + 1]]);
            //    vertices.Add(m_Vertices[m_Triangles[i + 2]]);
            //    triangles.Add(vertexIndexOffset);
            //    triangles.Add(vertexIndexOffset + 1);
            //    triangles.Add(vertexIndexOffset + 2);
            //    normals.Add(m_Normals[m_Triangles[i]]);
            //    normals.Add(m_Normals[m_Triangles[i + 1]]);
            //    normals.Add(m_Normals[m_Triangles[i + 2]]);
            //    triangleCount++;
            //    vertexIndexOffset += 3;
            //}
            //
            //m_Mesh.SetVertices(vertices);
            //m_Mesh.SetTriangles(triangles, 0);
            //m_Mesh.SetNormals(normals);
            //Graphics.DrawMesh(m_Mesh, Matrix4x4.identity, GameResources.Instance.MagazineMaterial, 0);
        }

        protected override void OnCreate()
        {
            m_Mesh = new Mesh();
        }

        protected override void OnUpdate()
        {
            m_Vertices.Clear();
            m_Triangles.Clear();
            m_Normals.Clear();

            var cam = Camera.main;
            if (cam != null)
            {
                var camPos = cam.transform.position;
                var camForward = cam.transform.forward;

                Entities.ForEach((in ParticleView particleView, in LocalToWorld localToWorld) =>
                {
                    var normal = ((Vector3)localToWorld.Position - camPos).normalized;
                    if (((Vector3)localToWorld.Position - Vector3.zero).sqrMagnitude < 10000 * 10000)
                    {
                        DrawQuad(localToWorld.Position, 0.8f, Quaternion.LookRotation(-camForward), normal, Color.cyan);
                        //DrawCone((Vector3)localToWorld.Position + normal * 0.8f * 1.5f, -normal * 0.8f *3f, 0.8f, 4);
                        
                    }
                }).WithoutBurst().Run();
            }

            Draw();
        }
    }
}
    