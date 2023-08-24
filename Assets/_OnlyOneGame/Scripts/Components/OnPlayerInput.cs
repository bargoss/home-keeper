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
    public partial class OnPlayerInputSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<NetworkStreamInGame>();
            //state.RequireForUpdate<NetCubeSpawner>();
        }
        
        protected override void OnUpdate()
        {
            foreach (var (onPlayerRw, onPlayerInputRw) in SystemAPI.Query<RefRW<OnPlayer>, RefRW<OnPlayerInput>>().WithAll<GhostOwnerIsLocal>())
            {
                var playerInput = onPlayerInputRw.ValueRO;
                playerInput.MovementInput = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                playerInput.LookInput = new float2(0,0);
                playerInput.ActionCommandOpt.Set(Input.GetKeyDown(KeyCode.Space) // todo getkeydown
                    ? Option<ActionCommand>.Some(new CommandMeleeAttack(playerInput.MovementInput.X0Y()))
                    : Option<ActionCommand>.None()
                );
                
                onPlayerInputRw.ValueRW = playerInput;
            }
        }
    }
}