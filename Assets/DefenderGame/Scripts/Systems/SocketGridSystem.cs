using DefenderGame.Scripts.Components;
using Unity.Entities;

namespace DefenderGame.Scripts.Systems
{
    public partial class SocketGridSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<ItemGrid>();
        }

        protected override void OnUpdate()
        {
            
        }
    }
}