using DefaultNamespace;
using Unity.Entities;

namespace DefenderGame.Scripts.Systems
{
    public partial class DeGameManager : SystemBase
    {
        // this class is the entry point object for the game
        
        public event System.Action GameFinished;
        
        protected override void OnCreate()
        {
            RequireForUpdate<DeGameManager>();
        }

        protected override void OnUpdate()
        {
            
        }

        public void CleanUp()
        {
            
        }

        //public void LoadLevel(. . .)
        //{
        //    
        //}
    }
}