using System.Collections.Generic;
using HomeKeeper.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace HomeKeeper.Systems
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
        
        
        protected override void OnUpdate()
        {
            m_Vertices.Clear();
            m_Triangles.Clear();

            Entities.ForEach((ref Item item, in LocalToWorld localToWorld) =>
            {
                switch (item.ItemType)
                {
                    case ItemType.Magazine:
                        //DrawTriangle(
                        //    localToWorld.Position + new float3(0.5f, -0.5f, 0),
                        //    localToWorld.Position + new float3(-0.5f, -0.5f, 0),
                        //    c: localToWorld.Position + new float3(0, 0.5f, 0)
                        //    );
                        Draw4FacedPyramid(localToWorld.Position, 1f);
                        break;
                    case ItemType.Resource:
                    case ItemType.All:
                    default:
                        break;
                }
            }).WithoutBurst().Run();
            
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
    