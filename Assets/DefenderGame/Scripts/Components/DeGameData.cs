using Unity.Entities;
using Unity.Mathematics;

namespace DefenderGame.Scripts.Components
{
    public class DeGameData : IComponentData
    {
        // stats:
        public float EnemySpawnRate;
        
        // state:
        public PlayerInput PlayerInput;
        public float LastEnemySpawnTime;
    }

    public struct PlayerInput
    {
        public float3 MousePos;
        public bool Up;
        public bool Down;
        public bool Pressing;
        
        // ctor
        public PlayerInput(float3 mousePos, bool up, bool down, bool pressing)
        {
            MousePos = mousePos;
            Up = up;
            Down = down;
            Pressing = pressing;
        }
        
        public PlayerInput GetUpdated(float3 mousePos, bool pressing)
        {
            var up = false;
            var down = false;

            if (Pressing && !pressing)
            {
                up = true;
            }
            else if (!Pressing && pressing)
            {
                down = true;
            }
            
            return new PlayerInput(mousePos, up, down, pressing);
        }
        
    }
}