using DefaultNamespace;
using Unity.Entities;
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
                AddComponent(entity, new OnTurretView(){LookDirection = Utility.Forward});
            }
        }
    }
}