using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class RigidbodyAxisLockAuthoring : MonoBehaviour
    {
        public bool LockX = false;
        public bool LockY = false;
        public bool LockZ = true;
    }
    
    public class RigidbodyAxisLockBaker : Baker<RigidbodyAxisLockAuthoring>
    {
        public override void Bake(RigidbodyAxisLockAuthoring authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent(entity, new RigidbodyAxisLock()
            {
                LockX = authoring.LockX,
                LockY = authoring.LockY,
                LockZ = authoring.LockZ,
            });
        }
    }
}