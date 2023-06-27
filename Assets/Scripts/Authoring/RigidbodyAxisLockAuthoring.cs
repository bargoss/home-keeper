using Components;
using HomeKeeper.Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class RigidbodyAxisLockAuthoring : MonoBehaviour
    {
        public bool LockLinearX = false;
        public bool LockLinearY = false;
        public bool LockLinearZ = true;
    }
    
    public class RigidbodyAxisLockBaker : Baker<RigidbodyAxisLockAuthoring>
    {
        public override void Bake(RigidbodyAxisLockAuthoring authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent(entity, new RigidbodyAxisLock()
            {
                LockLinearX = authoring.LockLinearX,
                LockLinearY = authoring.LockLinearY,
                LockLinearZ = authoring.LockLinearZ,
            });
        }
    }
}