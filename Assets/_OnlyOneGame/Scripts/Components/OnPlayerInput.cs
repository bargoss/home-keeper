using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Components
{
    [GhostComponent(PrefabType=GhostPrefabType.AllPredicted)]
    public struct OnPlayerInput : IInputComponentData
    {
        public float2 MovementInput;
        
    }
    
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial class SampleCubeInput : SystemBase
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkStreamInGame>();
            //state.RequireForUpdate<NetCubeSpawner>();
        }
        
        protected override void OnUpdate()
        {
            foreach (var (onPlayerRw, onPlayerInputRw) in SystemAPI.Query<RefRW<OnPlayer>, RefRW<OnPlayerInput>>().WithAll<GhostOwnerIsLocal>())
            {
                var playerInput = onPlayerInputRw.ValueRO;
                playerInput.MovementInput = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                onPlayerInputRw.ValueRW = playerInput;
            }
        }
    }
}