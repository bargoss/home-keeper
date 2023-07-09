using System.Collections.Generic;
using BulletCircle.GoViews;
using HomeKeeper.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace HomeKeeper.ViewSystems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class ShooterViewSystem : SystemBase
    {
        private readonly Dictionary<Entity, ShooterGOView> m_ShooterViews = new();
        
        protected override void OnUpdate()
        {
            return;
            var shooterPrefab = GameResources.Instance.shooterGoViewPrefab;
            
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            
            // creation
            Entities.ForEach((Entity entity, in LocalToWorld localToWorld, in ShooterView shooterView) =>
            {
                if (!m_ShooterViews.ContainsKey(entity))
                {
                    var shooterGoView = GameObject.Instantiate(shooterPrefab);
                    var goViewTransform = shooterGoView.transform;
                    goViewTransform.position = localToWorld.Position;
                    goViewTransform.rotation = localToWorld.Rotation;
                    goViewTransform.localScale = Vector3.one;
                    
                    m_ShooterViews.Add(entity, shooterGoView);
                }
            }).WithoutBurst().Run();
            
            // update
            Entities.ForEach((Entity entity, in LocalToWorld localToWorld, in ShooterView shooterView, in Shooter shooter) =>
            {
                if (m_ShooterViews.TryGetValue(entity, out var goView))
                {
                    var goViewTransform = goView.transform;
                    goViewTransform.position = localToWorld.Position;
                    goViewTransform.rotation = localToWorld.Rotation;
                    if (shooter.ShotThisFrame)
                    {
                        goView.ShootAnimation(1f / shooter.Stats.FireRate);
                    }
                    goView.UpdateLookDirection( math.normalizesafe(shooter.Look));
                }
            }).WithoutBurst().Run();
            
            // destruction
            Entities.ForEach((Entity entity, in ShooterView shooterView) =>
            {
                if (m_ShooterViews.TryGetValue(entity, out var goView))
                {
                    m_ShooterViews.Remove(entity);
                    commandBuffer.RemoveComponent<ShooterView>(entity);
                    GameObject.Destroy(goView.gameObject);
                }
            }).WithNone<Shooter>().WithoutBurst().Run();
            
            commandBuffer.Playback(EntityManager);
        }
    }
}