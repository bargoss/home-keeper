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
            var gameResourcesManaged = SystemAPI.ManagedAPI.GetSingleton<GameResourcesManaged>();

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

            Graphics.DrawMeshInstanced(gameResourcesManaged.Magazine.Mesh, 0, gameResourcesManaged.Magazine.Material,
                m_MagazineMatrices);
        }
    }
}
    