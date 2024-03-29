﻿using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Components.Deployed
{
    public class OnTurretAuthoring : MonoBehaviour
    {
        public class OnTurretBaker : Baker<OnTurretAuthoring>
        {
            public override void Bake(OnTurretAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new OnTurret(){LastShot = new NetworkTick(1)});
            }
        }
    }
}