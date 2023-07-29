using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.GoViews;
using DG.Tweening;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DefenderGame.Scripts.Systems
{
    public partial class ItemGridViewSystem : SystemBase
    {
        private readonly PairMaintainer<DeItemGrid, DeItemGridView> m_ItemGridViews = new(
            (logical) => Object.Instantiate(GameResources.Instance.ItemGridViewPrefab),
            (view) => Object.Destroy(view.gameObject)
        );
        private readonly PairMaintainer<Turret, TurretGOView> m_TurretViews = new(
            (logical) =>
            {
                var turretView = Object.Instantiate(GameResources.Instance.TurretGOViewPrefab);
                turretView.Restore(logical.AimDirection);
                return turretView;
            },
            (view) => Object.Destroy(view.gameObject)
        );
        private readonly PairMaintainer<Magazine, MagazineGOView> m_MagazineViews = new(
            (logical) =>
            {
                var magazineView = Object.Instantiate(GameResources.Instance.MagazineGOViewPrefab);
                magazineView.Restore(logical.AmmoCount, logical.AmmoCapacity, logical.AmmoTier);
                return magazineView;
            },
            (view) => Object.Destroy(view.gameObject)
        );
        private readonly PairMaintainer<AmmoBox, AmmoBoxGOView> m_AmmoBoxViews = new(
            (logical) =>
            {
                var ammoBoxView = Object.Instantiate(GameResources.Instance.AmmoBoxGOViewPrefab);
                ammoBoxView.Restore(logical.AmmoCount, logical.AmmoCapacity, logical.AmmoTier);
                return ammoBoxView;
            },
            (view) => Object.Destroy(view.gameObject)
        );
        

        protected override void OnCreate()
        {
            RequireForUpdate<DeItemGrid>();
        }
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, DeItemGrid itemGrid, LocalToWorld gridLtw) =>
            {
                var view = m_ItemGridViews.GetOrCreateView(itemGrid);
                var viewTransform = view.transform; 
                viewTransform.position = gridLtw.Position;
                viewTransform.rotation = gridLtw.Rotation;
                viewTransform.localScale = Vector3.one;
                
                
                itemGrid.ItemGrid.ForEachItem((item, pos) =>
                {
                    switch (item.Item)
                    {
                        case AmmoBox ammoBox:
                            var ammoBoxView = m_AmmoBoxViews.GetOrCreateView(ammoBox);
                            ammoBoxView.transform.position = (Vector3)gridLtw.Position + new Vector3(pos.x, 0, pos.y);
                            ammoBoxView.transform.rotation = gridLtw.Rotation;

                            if (ammoBox.AmmoCountChangedTime.Equals((float)SystemAPI.Time.ElapsedTime))
                            {
                                ammoBoxView.SetAmmoCount(ammoBox.AmmoCount);
                            }
                            
                            break;
                        case Magazine magazine:
                            var magazineView = m_MagazineViews.GetOrCreateView(magazine);
                            magazineView.transform.position = (Vector3)gridLtw.Position + new Vector3(pos.x, 0, pos.y);
                            magazineView.transform.rotation = gridLtw.Rotation;
                            
                            if (magazine.AmmoCountChangedTime.Equals((float)SystemAPI.Time.ElapsedTime))
                            {
                                magazineView.SetAmmoCount(magazine.AmmoCount);
                            }
                            break;
                        case Turret turret:
                            var turretView = m_TurretViews.GetOrCreateView(turret);
                            turretView.transform.position = (Vector3)gridLtw.Position + new Vector3(pos.x, 0, pos.y);
                            turretView.transform.rotation = gridLtw.Rotation;
                            
                            if (turret.LastShotTime.Equals((float)SystemAPI.Time.ElapsedTime))
                            {
                                turretView.AnimateShoot();
                                turretView.UpdateAimDirection(turret.AimDirection);
                            }
                            break;
                        default:
                            Debug.LogError("Unknown item type");
                            break;
                    }
                });
                
                foreach (var onGoingAction in itemGrid.OngoingActions)
                {
                    switch (onGoingAction)
                    {
                        case AmmoBoxFillingMagazine ammoBoxFillingMagazine:
                            if (ammoBoxFillingMagazine.LastAmmoLoadedTime.Equals((float)SystemAPI.Time.ElapsedTime))
                            {
                                if (
                                    itemGrid.TryGetGridObject<AmmoBox>(ammoBoxFillingMagazine.AmmoBoxPos, out var ammoBox) &&
                                    itemGrid.TryGetGridObject<Magazine>(ammoBoxFillingMagazine.MagazinePos, out var magazine)
                                )
                                {
                                    var ammoBoxView = m_AmmoBoxViews.GetOrCreateView(ammoBox);
                                    var magazineView = m_MagazineViews.GetOrCreateView(magazine);
                                    
                                    //tween a bullet from ammo box to magazine
                                    
                                    var bulletView = PoolManager.Instance.BulletViewPool.Get();
                                    bulletView.Restore(ammoBox.AmmoTier);
                                    bulletView.transform.position = ammoBoxView.BulletFeed.transform.position;
                                    bulletView.transform.rotation = ammoBoxView.BulletFeed.transform.rotation;
                                    
                                    var jumpTargetPos = magazineView.BulletFeed.transform.position;
                                    var targetRotation = magazineView.BulletFeed.transform.rotation;
                                    var targetUp = targetRotation * Vector3.up;

                                    bulletView.transform.DOJump(jumpTargetPos, 0.5f, 1, 0.5f)
                                        .Join(bulletView.transform.DORotateQuaternion(targetRotation, 0.5f))
                                        .OnComplete(() =>
                                            {
                                                //PoolManager.Instance.PlaySmallShockEffect(bulletView.transform.position, targetUp);
                                                bulletView.HandleDestroy();
                                            }
                                        );
                                }
                                else Debug.LogError("shouldn't happen");
                            }
                                
                            break;
                        case TurretLoadingMagazine turretLoadingMagazine:
                            if (turretLoadingMagazine.StartTime.Equals((float)SystemAPI.Time.ElapsedTime))
                            {
                                if (
                                    itemGrid.TryGetGridObject<Turret>(turretLoadingMagazine.TurretPos, out var turret)
                                )
                                {
                                    var turretView = m_TurretViews.GetOrCreateView(turret);
                                    
                                    // create and pull out fake magazine
                                    if (turretLoadingMagazine.PreviousMagazine != null)
                                    {
                                        turretView.AnimateShake();
                                        
                                        var magView = PoolManager.Instance.MagazineViewPool.Get();
                                        magView.Restore(
                                            turretLoadingMagazine.PreviousMagazine.AmmoCount,
                                            turretLoadingMagazine.PreviousMagazine.AmmoCapacity,
                                            turretLoadingMagazine.PreviousMagazine.AmmoTier
                                        );
                                        var magViewTr = magView.transform;
                                        var magSlotTr = turretView.MagazineSlot.transform;
                                        magViewTr.position = magSlotTr.position;
                                        magViewTr.rotation = magSlotTr.rotation;
                                        
                                        var endPos = GridToWorldPos(turretLoadingMagazine.NewMagazinePositionBeforeLoad, gridLtw);
                                        var endRot = gridLtw.Rotation;

                                        var duration = turretLoadingMagazine.ActionDuration;

                                        magViewTr
                                            .DOJump(endPos, 0.5f, 1, 0.7f * duration)
                                            .Join(magViewTr.DORotateQuaternion(endRot, 0.6f * duration))
                                            .AppendCallback(() => magView.ShakeFromBottom(0.3f * duration))
                                            .AppendInterval(0.3f * duration)
                                            .OnComplete(() =>
                                                {
                                                    magView.HandleDestroy();
                                                }
                                            );
                                    }
                                    
                                    
                                    var magazine = turret.Magazine.MagazineTier
                                    
                                    
                                    
                                    // todo: do the magazine swap animation
                                }
                                else Debug.LogError("shouldn't happen");
                                
                            }

                            break;
                        default:
                            Debug.LogError("Unknown ongoing action type: " + onGoingAction.GetType());
                            break;
                    }
                }
            }).WithBurst().Run();
            
            
            // garbage collection pretty much
            m_ItemGridViews.DisposeAndClearUntouchedViews();
            m_TurretViews.DisposeAndClearUntouchedViews();
            m_MagazineViews.DisposeAndClearUntouchedViews();
            m_AmmoBoxViews.DisposeAndClearUntouchedViews();
        }
        
        private Vector3 GridToWorldPos(int2 gridPos, LocalToWorld itemGridLtw)
        {
            var gridPosAsF4 = new float4(gridPos.x, 0, gridPos.y,1);
            var transformed = math.mul(itemGridLtw.Value, gridPosAsF4);
            return transformed.xyz;
        }
        
        private int2 WorldToGridPos(Vector3 worldPos, LocalToWorld itemGridLtw)
        {
            var inv = math.inverse(itemGridLtw.Value);
            var worldPosAsF4 = new float4(worldPos.x, worldPos.y, worldPos.z, 1);
            var transformed = math.mul(inv, worldPosAsF4);
            return new int2((int)transformed.x, (int)transformed.z);
    }
}