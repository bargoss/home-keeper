using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.GoViews;
using DG.Tweening;
using JetBrains.Annotations;
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
            (logical) =>
            {
                var view = Object.Instantiate(GameResources.Instance.ItemGridViewPrefab);
                view.Restore(logical.ItemGrid.Width, logical.ItemGrid.Height);
                return view;
            },
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
            Entities.ForEach((DeItemGrid itemGrid, LocalToWorld gridLtw) =>
            {
                var view = m_ItemGridViews.GetOrCreateView(itemGrid);
                var viewTransform = view.transform;
                viewTransform.position = gridLtw.Position;
                viewTransform.rotation = gridLtw.Rotation;
                viewTransform.localScale = Vector3.one;


                itemGrid.ItemGrid.ForEachItem((item, pos) =>
                {
                    switch (item)
                    {
                        case AmmoBox ammoBox:
                            var ammoBoxView = m_AmmoBoxViews.GetOrCreateView(ammoBox);
                            var ammoBoxViewTr = ammoBoxView.transform;
                            ammoBoxViewTr.position = (Vector3)gridLtw.Position + new Vector3(pos.x, 0, pos.y);
                            ammoBoxViewTr.rotation = gridLtw.Rotation;

                            if (ammoBox.AmmoCountChangedTime.Equals((float)SystemAPI.Time.ElapsedTime))
                            {
                                ammoBoxView.SetAmmoCount(ammoBox.AmmoCount);
                            }

                            break;
                        case Magazine magazine:
                            var magazineView = m_MagazineViews.GetOrCreateView(magazine);
                            var magazineViewTr = magazineView.transform;
                            magazineViewTr.position = (Vector3)gridLtw.Position + new Vector3(pos.x, 0, pos.y);
                            magazineViewTr.rotation = gridLtw.Rotation;

                            if (magazine.AmmoCountChangedTime.Equals((float)SystemAPI.Time.ElapsedTime))
                            {
                                magazineView.SetAmmoCount(magazine.AmmoCount);
                            }

                            break;
                        case Turret turret:
                            var turretView = m_TurretViews.GetOrCreateView(turret);
                            var turretViewTr = turretView.transform;
                            turretViewTr.position = (Vector3)gridLtw.Position + new Vector3(pos.x, 0, pos.y);
                            turretViewTr.rotation = gridLtw.Rotation;

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
                                    itemGrid.TryGetGridObject<AmmoBox>(ammoBoxFillingMagazine.AmmoBoxPos,
                                        out var ammoBox) &&
                                    itemGrid.TryGetGridObject<Magazine>(ammoBoxFillingMagazine.MagazinePos,
                                        out var magazine)
                                )
                                {
                                    AnimateAmmoBoxToMagazine(ammoBox, magazine, m_AmmoBoxViews, m_MagazineViews);
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
                                    StartTurretMagLoadingAnimation(turret, turretLoadingMagazine, gridLtw,
                                        m_TurretViews);
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

        private static void AnimateAmmoBoxToMagazine(
            AmmoBox ammoBox,
            Magazine magazine,
            PairMaintainer<AmmoBox, AmmoBoxGOView> ammoBoxViews,
            PairMaintainer<Magazine, MagazineGOView> magazineViews
        )
        {
            var ammoBoxView = ammoBoxViews.GetOrCreateView(ammoBox);
            var ammoBoxViewTr = ammoBoxView.transform;
            var magazineView = magazineViews.GetOrCreateView(magazine);
            var magazineViewBulletFeedTr = magazineView.BulletFeed.transform;

            //tween a bullet from ammo box to magazine

            var bulletView = PoolManager.Instance.BulletViewPool.Get();
            var bulletViewTr = bulletView.transform;
            bulletView.Restore(ammoBox.AmmoTier);
            
            
            bulletViewTr.position = ammoBoxViewTr.position;
            bulletViewTr.rotation = ammoBoxViewTr.rotation;

            var jumpTargetPos = magazineViewBulletFeedTr.position;
            var targetRotation = magazineViewBulletFeedTr.rotation;

            bulletView.transform.DOJump(jumpTargetPos, 0.5f, 1, 0.5f)
                .Join(bulletView.transform.DORotateQuaternion(targetRotation, 0.5f))
                .OnComplete(() =>
                    {
                        //PoolManager.Instance.PlaySmallShockEffect(bulletView.transform.position, targetUp);
                        bulletView.HandleDestroy();
                    }
                );
        }

        private static void StartTurretMagLoadingAnimation(
            Turret turret,
            TurretLoadingMagazine turretLoadingMagazine,
            LocalToWorld gridLtw,
            PairMaintainer<Turret, TurretGOView> turretViews

        )
        {
            var turretView = turretViews.GetOrCreateView(turret);

            var magSlotTr = turretView.MagazineSlot.transform;

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
                    .OnComplete(() => { magView.HandleDestroy(); }
                    );
            }


            var newMagView = PoolManager.Instance.MagazineViewPool.Get();
            newMagView.Restore(
                turretLoadingMagazine.NewMagazine.AmmoCount,
                turretLoadingMagazine.NewMagazine.AmmoCapacity,
                turretLoadingMagazine.NewMagazine.AmmoTier
            );
            var newMagViewTr = newMagView.transform;
            newMagViewTr.position = GridToWorldPos(turretLoadingMagazine.NewMagazinePositionBeforeLoad, gridLtw);
            newMagViewTr.rotation = gridLtw.Rotation;


            var newMagEndPos = magSlotTr.position;
            var newMagEndRot = magSlotTr.rotation;

            var newMagDuration = turretLoadingMagazine.ActionDuration;

            newMagViewTr
                .DOJump(newMagEndPos, 0.5f, 1, 0.7f * newMagDuration)
                .Join(newMagViewTr.DORotateQuaternion(newMagEndRot, 0.6f * newMagDuration))
                .AppendCallback(() => newMagView.ShakeFromTop(0.3f * newMagDuration))
                .AppendInterval(0.3f * newMagDuration)
                .OnComplete(() => { newMagView.HandleDestroy(); }
                );
        }

        private static Vector3 GridToWorldPos(int2 gridPos, LocalToWorld itemGridLtw)
        {
            var gridPosAsF4 = new float4(gridPos.x, 0, gridPos.y, 1);
            var transformed = math.mul(itemGridLtw.Value, gridPosAsF4);
            return transformed.xyz;
        }

        [UsedImplicitly]
        private static int2 WorldToGridPos(Vector3 worldPos, LocalToWorld itemGridLtw)
        {
            var inv = math.inverse(itemGridLtw.Value);
            var worldPosAsF4 = new float4(worldPos.x, worldPos.y, worldPos.z, 1);
            var transformed = math.mul(inv, worldPosAsF4);
            return new int2((int)transformed.x, (int)transformed.z);
        }
    }
}