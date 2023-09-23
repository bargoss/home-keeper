using System;
using System.Collections.Generic;
using _OnlyOneGame.Scripts.Components.Tank.Actions;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;

namespace _OnlyOneGame.Scripts.Components
{
    public class CardsUIData : IComponentData
    {
        // controlled from Game:
        public List<TankActionCard> Cards;
        [CanBeNull] public DroppedEventInfo DroppedEventEvent;
        
        // controlled from UI and maybe Game:
        public ActionState State;

        public class DroppedEventInfo
        {
            public int CardIndexWas;
            public TankActionCard Card;
            public float3 WorldPos;
            public float3 CharacterWorldPos;
        }
        public abstract class ActionState
        {
            public class Idle : ActionState
            {
                
            }

            public class SelectedDragging : ActionState
            {
                public int SelectedCardIndex;
                public float3 TargetingWorldPos;
                public float3 CharacterWorldPos;
                
                public SelectedDragging(int selectedCardIndex, float3 targetingWorldPos, float3 characterWorldPos)
                {
                    SelectedCardIndex = selectedCardIndex;
                    TargetingWorldPos = targetingWorldPos;
                    CharacterWorldPos = characterWorldPos;
                }
            }
        }
    }
    
    
}