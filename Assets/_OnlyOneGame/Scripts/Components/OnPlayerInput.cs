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
        [GhostField] public InputEvent Action0;
        [GhostField] public InputEvent Action1;
        [GhostField] public InputEvent Action2;
        
        [GhostField] public InputEvent DropButtonTap;
        [GhostField] public InputEvent DropButtonHolding;
        
        [GhostField] public InputEvent PickupButtonTap;
        [GhostField] public InputEvent PickupButtonHolding;
    }
    
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class OnPlayerInputSystem : SystemBase
    {
        private TapAndHoldTracker m_DropButtonTracker = new TapAndHoldTracker(0.2f);
        private TapAndHoldTracker m_PickupButtonTracker = new TapAndHoldTracker(0.2f);
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
                playerInput = default;
                
                playerInput.MovementInput = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                playerInput.LookInput = new float2(0,0);
                
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    playerInput.Action0.Set();
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    playerInput.Action1.Set();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    playerInput.Action2.Set();
                }
                
                m_DropButtonTracker.Update(Input.GetKeyDown(KeyCode.Q), Input.GetKey(KeyCode.Q), Input.GetKeyUp(KeyCode.Q), (float)SystemAPI.Time.ElapsedTime, out var holdingDrop, out var tappedDrop);
                m_PickupButtonTracker.Update(Input.GetKeyDown(KeyCode.E), Input.GetKey(KeyCode.E), Input.GetKeyUp(KeyCode.E), (float)SystemAPI.Time.ElapsedTime, out var holdingPickup, out var tappedPickup);
                
                if (tappedDrop)
                {
                    playerInput.DropButtonTap.Set();
                }
                else if (holdingDrop)
                {
                    playerInput.DropButtonHolding.Set();
                }
                
                if (tappedPickup)
                {
                    playerInput.PickupButtonTap.Set();
                }
                else if (holdingPickup)
                {
                    playerInput.PickupButtonHolding.Set();
                }
                
                onPlayerInputRw.ValueRW = playerInput;
            }
        }
    }
    public struct TapAndHoldTracker
    {
        private float m_TapDurationThreshold;
        private float m_TapStartTime;
        private bool m_IsHolding;

        public TapAndHoldTracker(float tapThreshold)
        {
            m_TapDurationThreshold = tapThreshold;
            m_TapStartTime = 0f;
            m_IsHolding = false;
        }

        public void Update(bool isDown, bool isPressed, bool isUp, float currentTime, out bool holding, out bool tapped)
        {
            tapped = false;
            holding = false;
            
            
            if (isDown)
            {
                m_TapStartTime = currentTime;
                m_IsHolding = true;
            }

            if (isPressed)
            {
                if (m_IsHolding && currentTime - m_TapStartTime >= m_TapDurationThreshold)
                {
                    // Holding logic
                    Debug.Log("Holding");
                    holding = true;
                }
            }

            if (isUp)
            {
                if (m_IsHolding)
                {
                    if (currentTime - m_TapStartTime < m_TapDurationThreshold)
                    {
                        // Tap logic
                        Debug.Log("Tapped");
                        tapped = true;
                    }

                    m_IsHolding = false;
                }
            }
        }
    }
}