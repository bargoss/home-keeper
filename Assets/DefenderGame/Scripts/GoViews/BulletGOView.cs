using UnityEngine;

namespace DefenderGame.Scripts.GoViews
{
    public class BulletGOView : MonoBehaviour
    {
        public void Restore(int levelIndex)
        {
            //todo
        }

        public void HandleDestroy()
        {
            PoolManager.Instance.BulletViewPool.Release(this);
        }
    }
}