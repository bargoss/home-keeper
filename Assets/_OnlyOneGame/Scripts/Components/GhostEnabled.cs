using JetBrains.Annotations;
using Unity.Entities;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Components
{
    public struct DestroyableGhost : IComponentData
    {
        public NetworkTick m_DestroyedTime;

        public void SetDestroyed(NetworkTick tick)
        {
            m_DestroyedTime = tick;
        }

        [Pure]
        public bool GetDestroyed(NetworkTick tick)
        {
            if (m_DestroyedTime.IsValid)
            {
                var ticksSinceDestroyed = tick.TicksSince(m_DestroyedTime);
                if (ticksSinceDestroyed >= 0 && ticksSinceDestroyed < 50 * 1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}