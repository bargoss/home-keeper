using _OnlyOneGame.Scripts.Components.Tank.Actions;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace _OnlyOneGame.Scripts.Components.Tank
{
    public struct OnTank : IComponentData
    {
        
    }
    public struct OnTankSkills : IComponentData
    {
        public FixedList64Bytes<TankActionCard> ActionCards;
        public Option<OnGoingTankAction> OnGoingAction;
        public int Energy;
        public NetworkTick SkillsBlockedUntil;
    }
    
}