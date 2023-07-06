using System;
using System.Collections.Generic;
using HomeKeeper.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace HomeKeeper.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class ItemGraphicsSystem : SystemBase
    {
        private readonly List<Matrix4x4> m_MagazineMatrices = new();
        
        protected override void OnUpdate()
        {
            Entities.ForEach((ref Item item, in LocalToWorld localToWorld) =>
            {
                switch (item.ItemType)
                {
                    case ItemType.Magazine:
                        m_MagazineMatrices.Add(localToWorld.Value);
                        break;
                    case ItemType.Resource:
                    case ItemType.All:
                    default:
                        break;
                }
            }).WithoutBurst().Run();

            //Graphics.DrawMeshInstanced(gameResourcesManaged.Magazine.Mesh.Result, 0, gameResourcesManaged.Magazine.Material.Result, m_MagazineMatrices);
            
            // create a simple cube mesh
            var mesh = new Mesh();
            mesh.vertices = new Vector3[]
            {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 0),
                new Vector3(0, 1, 0),
            };
            mesh.triangles = new int[]
            {
                0, 1, 2,
                0, 2, 3,
            };
            mesh.RecalculateNormals();
            
            // create a simple urp material
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.enableInstancing = true;
            
            Graphics.DrawMeshInstanced(mesh, 0, material, m_MagazineMatrices);
            
            m_MagazineMatrices.Clear();
        }
    }
}
    