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
    public partial class ItemViewSystem : SystemBase
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
            
            var mesh = GameResources.Instance.MagazineMesh; //new Mesh();
            
            var material = GameResources.Instance.MagazineMaterial; //new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.enableInstancing = true;
            
            Graphics.DrawMeshInstanced(mesh, 0, material, m_MagazineMatrices);
            
            m_MagazineMatrices.Clear();
        }
    }
}
    