using DefaultNamespace;
using Unity.Entities;

namespace DefenderGame.Scripts.Systems
{
    public partial class DeGameManager : SystemBase
    {
        public event System.Action GameFinished;
        
        
        protected override void OnUpdate()
        {
            
        }
    }
}