using System.Collections.Generic;
using RunnerGame.Scripts.ECS.Components;
using Unity.Entities;
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
        private void DrawTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            var normal = Vector3.Cross(b - a, c - a).normalized;
            DrawTriangle(a, b, c, normal, normal, normal);
        }
        private void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 normalA, Vector3 normalB, Vector3 normalC)
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

        private void DrawQuad(Vector3 center, float edgeLen, Quaternion rotation)
        {
            var index = m_Vertices.Count;
            m_Vertices.Add(center + rotation * new Vector3(-edgeLen, -edgeLen, 0));
            m_Vertices.Add(center + rotation * new Vector3(edgeLen, -edgeLen, 0));
            m_Vertices.Add(center + rotation * new Vector3(edgeLen, edgeLen, 0));
            m_Vertices.Add(center + rotation * new Vector3(-edgeLen, edgeLen, 0));
            m_Triangles.Add(index);
            m_Triangles.Add(index + 1);
            m_Triangles.Add(index + 2);
            m_Triangles.Add(index);
            m_Triangles.Add(index + 2);
            m_Triangles.Add(index + 3);
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
                DrawTriangle(baseCircleVertices[i], tipPosition, baseCircleVertices[(i + 1) % baseCircleVertices.Count]);
            }
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
                    if (((Vector3)localToWorld.Position - Vector3.zero).sqrMagnitude < 1000)
                    {
                        //DrawQuad(localToWorld.Position, 0.8f, Quaternion.LookRotation(-camForward));
                        DrawCone(localToWorld.Position, -normal * 0.8f, 0.8f, 4);
                    }
                }).WithoutBurst().Run();
            }

            if (m_Mesh == null)
            {
                m_Mesh = new Mesh();
            }
            else
            {
                m_Mesh.Clear();
            }
            
            m_Mesh.SetVertices(m_Vertices);
            m_Mesh.SetTriangles(m_Triangles, 0);
            m_Mesh.SetNormals(m_Normals);
            
            var material = GameResources.Instance.MagazineMaterial;
            Graphics.DrawMesh(m_Mesh, Matrix4x4.identity, material, 0);
        }
    }
}
    