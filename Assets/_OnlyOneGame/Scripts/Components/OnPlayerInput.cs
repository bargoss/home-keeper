using DefaultNamespace;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace _OnlyOneGame.Scripts.Components
{
    [GhostComponent(PrefabType=GhostPrefabType.AllPredicted, OwnerSendType = SendToOwnerType.SendToOwner)]
    public struct OnPlayerInput : IInputComponentData
    {
        [GhostField] public float2 MovementInput;
        [GhostField] public float2 LookInput;
        [GhostField] public BytesAs<Option<ActionCommand>, Data32Bytes> ActionCommandOpt;
        
    }
    
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
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
                playerInput.LookInput = new float2(0,1);
                playerInput.ActionCommandOpt = Input.GetKeyDown(KeyCode.Space)
                    ? Option<ActionCommand>.Some(new CommandMeleeAttack(Utility.Forward))
                    : Option<ActionCommand>.None();

                onPlayerInputRw.ValueRW = playerInput;
                
                Debug.Log("input set: " + playerInput.MovementInput);
            }
        }
    }
}