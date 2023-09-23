using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.VisualScripting;
using UnityEngine;

namespace _StrategyGame.Scripts.Components
{
    public struct StUnit : IComponentData
    {
        [GhostField] public float2 TargetPosition;
    }
    
    public struct StSelectable : IComponentData
    {
        
    }
    public struct StSelected : IComponentData
    {
        
    }

    public class StUI : MonoBehaviour
    {
        public event Action<BoxSelectionCommandData> BoxSelectionCommand;
        public event Action<MoveCommandData> MoveCommand;

        public readonly List<Vector3> HighlightPositions = new();
        
        
        public struct BoxSelectionCommandData
        {
            public Vector3 Start;
            public Vector3 End;
        }

        public struct MoveCommandData
        {
            public Vector3 TargetPosition;
        }
    }
    
    public class 
}