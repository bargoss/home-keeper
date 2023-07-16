using System.Collections.Generic;
using HomeKeeper.Components;
using RunnerGame.Scripts.ECS.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace RunnerGame.Scripts.ECS.ViewSystems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class ItemViewSystem : SystemBase
    {
        //private readonly List<Matrix4x4> m_MagazineMatrices = new();
        private Mesh m_Mesh;
        private List<Vector3> m_Vertices = new List<Vector3>();
        private List<int> m_Triangles = new List<int>();
        private void DrawTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            var index = m_Vertices.Count;
            m_Vertices.Add(a);
            m_Vertices.Add(b);
            m_Vertices.Add(c);
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

        private void DrawQuad(Vector3 center, float edgeLen, Vector3 normal)
        {
            normal *= -1;
            var index = m_Vertices.Count;
            m_Vertices.Add(center + new Vector3(-edgeLen, -edgeLen, 0));
            m_Vertices.Add(center + new Vector3(edgeLen, -edgeLen, 0));
            m_Vertices.Add(center + new Vector3(edgeLen, edgeLen, 0));
            m_Vertices.Add(center + new Vector3(-edgeLen, edgeLen, 0));
            m_Triangles.Add(index);
            m_Triangles.Add(index + 1);
            m_Triangles.Add(index + 2);
            m_Triangles.Add(index);
            m_Triangles.Add(index + 2);
            m_Triangles.Add(index + 3);
        }
        
        
        protected override void OnUpdate()
        {
            m_Vertices.Clear();
            m_Triangles.Clear();

            var cam = Camera.main;
            if (cam != null)
            {
                var camPos = cam.transform.position;

                Entities.ForEach((in ParticleView particleView, in LocalToWorld localToWorld) =>
                {
                    var normal = ((Vector3)localToWorld.Position - camPos).normalized;
                    if (((Vector3)localToWorld.Position - Vector3.zero).sqrMagnitude < 1000)
                    {
                        DrawQuad(localToWorld.Position, 2, normal);
                        Draw4FacedPyramid(localToWorld.Position, 1f);
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
            m_Mesh.RecalculateNormals();
            
            //var mesh = GameResources.Instance.MagazineMesh;
            var material = GameResources.Instance.MagazineMaterial;
            //material.enableInstancing = true;
            
            Graphics.DrawMesh(m_Mesh, Matrix4x4.identity, material, 0);
            //Graphics.DrawMeshInstanced(mesh, 0, material, m_MagazineMatrices);
            
            //m_MagazineMatrices.Clear();
        }
    }
}
    