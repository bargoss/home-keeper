using System;
using _OnlyOneGame.Scripts.Components;
using _OnlyOneGame.Scripts.GoViewScripts;
using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.GoViews;
using DefenderGame.Scripts.Systems;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _OnlyOneGame.Scripts.Systems.ViewSystems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ServerSimulation)]
    public partial class GroundItemViewSystem : SystemBase
    {
        private PairMaintainer<ViewId, ItemGoView> m_ItemPairMaintainer =
            new(_ =>
                {
                    var turretView = Object.Instantiate(GameResources.Instance.ItemGOViewPrefab);
                    turretView.Restore("");
                    return turretView;
                },
                view => Object.Destroy(view.gameObject)
            );

        protected override void OnCreate()
        {
            RequireForUpdate<GroundItem>();
        }

        protected override void OnStopRunning()
        {
            m_ItemPairMaintainer.DisposeAndClearUntouchedViews();
        }

        protected override void OnUpdate()
        {
            var random = new Unity.Mathematics.Random((uint)(SystemAPI.Time.ElapsedTime * 10000 + 1));
            foreach (var (groundItemRw, localTransform, entity) in SystemAPI.Query<RefRW<GroundItem>, LocalTransform>().WithEntityAccess())
            {
                
                // its assigned once only
                if(groundItemRw.ValueRO.ViewId.Assigned == false)
                {
                    var groundItem = groundItemRw.ValueRO;
                    groundItem.ViewId = new ViewId(random.NextInt());
                    groundItemRw.ValueRW = groundItem;
                    
                    var groundItemView = m_ItemPairMaintainer.GetOrCreateView(groundItemRw.ValueRO.ViewId);
                    
                    var itemText = "?";

                    groundItemRw.ValueRO.Item.Get().Switch(deployable =>
                        {
                            itemText = Enum.GetName(typeof(ItemTypeDeployable), deployable);
                        },
                        minion =>
                        {
                            itemText = Enum.GetName(typeof(ItemTypeMinion), minion);
                        },
                        throwable =>
                        {
                            itemText = Enum.GetName(typeof(ItemTypeThrowable), throwable);
                        },
                        resource =>
                        {
                            itemText = Enum.GetName(typeof(ItemTypeResource), resource);
                        }
                    );
                
                    groundItemView.Restore(itemText);
                }
                
                var groundItemViewTransform = m_ItemPairMaintainer.GetOrCreateView(groundItemRw.ValueRO.ViewId).transform;
                var offsetForServer = World.Flags.HasFlag(WorldFlags.GameServer) ? new float3(0, 2f, 0) : float3.zero;
                
                groundItemViewTransform.position = localTransform.Position + offsetForServer;
                //Debug.DrawRay(groundItemViewTransform.position, Vector3.down * 10);
                var lineEnd = groundItemViewTransform.position;
                lineEnd.y = -5;
                Debug.DrawLine(groundItemViewTransform.position, lineEnd, Color.red);
                
                
                
                
                groundItemViewTransform.rotation = localTransform.Rotation;
            }
            
            m_ItemPairMaintainer.DisposeAndClearUntouchedViews();
        }
    }
}