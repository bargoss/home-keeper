using DefaultNamespace;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Components.Deployed
{
    public class OnTurretViewAuthoring : MonoBehaviour
    {
        public class OnTurretViewBaker : Baker<OnTurretViewAuthoring>
        {
            public override void Bake(OnTurretViewAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new OnTurretView(){LookDirection = Utility.Forward, LastShot = new NetworkTick(1), LastShotDisplayed = new NetworkTick(1)});
            }
        }
    }
}