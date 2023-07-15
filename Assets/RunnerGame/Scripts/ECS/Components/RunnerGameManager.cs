using Unity.Entities;
using UnityEngine;

namespace RunnerGame.Scripts.ECS.Components
{
    public class RunnerGameManagerAuthoring : MonoBehaviour
    {
        // constants
        public float PlayerForwardSpeed = 1;
        public float PlayerSidewaysP = 1;
        public float PlayerSidewaysD = 0.1f;
        
        // input
        public float PlayerHorizontalTarget = 0; // between 0, 1
    }
}