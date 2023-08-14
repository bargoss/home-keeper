using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Components
{
    [GhostComponent(PrefabType=GhostPrefabType.AllPredicted)]
    public struct CubeInput : IInputComponentData
    {
        public float3 Value;
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
            foreach (var playerInputRw in SystemAPI.Query<RefRW<CubeInput>>().WithAll<GhostOwnerIsLocal>())
            {
                var playerInput = playerInputRw.ValueRO;
                playerInput.Value = new float3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                playerInputRw.ValueRW = playerInput;
            }
        }
    }
}