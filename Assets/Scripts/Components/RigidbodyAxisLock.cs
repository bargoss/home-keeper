using Unity.Entities;

namespace Components
{
    public struct RigidbodyAxisLock : IComponentData
    {
        public bool LockX;
        public bool LockY;
        public bool LockZ;
    }
}