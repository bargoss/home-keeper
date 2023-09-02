using _OnlyOneGame.Scripts.Components.Deployed;
using DefenderGame.Scripts.Components;
using DefenderGame.Scripts.GoViews;
using DefenderGame.Scripts.Systems;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace _OnlyOneGame.Scripts.Systems.ViewSystems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class OnTurretViewSystem : SystemBase
    {
        private PairMaintainer<ViewId, TurretGOView> m_TurretPairMaintainer =
            new(_ =>
                {
                    var turretView = Object.Instantiate(GameResources.Instance.TurretGOViewPrefab);
                    turretView.Restore(Vector3.forward, new Magazine(1, 1, 0, 0));
                    return turretView;
                },
                view => Object.Destroy(view.gameObject)
            );


        protected override void OnUpdate()
        {
            var random = new Random((uint)(SystemAPI.Time.ElapsedTime * 10000 + 1));
            foreach (var (onTurretViewRw, localTransform, entity) in SystemAPI.Query<RefRW<OnTurretView>, LocalTransform>().WithEntityAccess())
            {
                var onTurretView = onTurretViewRw.ValueRO;
                if(onTurretView.ViewId.Assigned == false)
                {
                    onTurretView.ViewId = new ViewId(random.NextInt());
                }
                
                var viewPair = m_TurretPairMaintainer.GetOrCreateView(onTurretView.ViewId);
                var viewPairTransform = viewPair.transform;
                viewPairTransform.position = localTransform.Position;
                viewPairTransform.rotation = localTransform.Rotation;
                
                viewPair.UpdateAimDirection(onTurretView.LookDirection);

                if(!onTurretView.LastShotDisplayed.IsValid)
                {
                    onTurretView.LastShotDisplayed = new NetworkTick(1);
                }
                
                if (onTurretView.LastShot.IsValid &&
                    onTurretView.LastShot.TicksSince(onTurretView.LastShotDisplayed) > 0)
                {
                    viewPair.AnimateShoot(1);
                    onTurretView.LastShotDisplayed = onTurretView.LastShot;
                }
                onTurretViewRw.ValueRW = onTurretView;
            }

            m_TurretPairMaintainer.DisposeAndClearUntouchedViews();
        }
    }
}