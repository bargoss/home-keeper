using Unity.Entities;
using Unity.Mathematics;

namespace DefenderGame.Scripts.Components
{
    public struct DeGameData : IComponentData
    {
        public PlayerInput PlayerInput;
    }

    public readonly struct PlayerInput
    {
        public readonly float3 MousePos;
        public readonly bool Up;
        public readonly bool Down;
        public readonly bool Pressing;
        
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