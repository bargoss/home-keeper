using DefaultNamespace;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
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
        [GhostField] public InputEvent DropButtonReleasedFromHold;
        
        [GhostField] public InputEvent PickupButtonTap;
        [GhostField] public InputEvent PickupButtonReleasedFromHold;
    }
    
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class OnPlayerInputSystem : SystemBase
    {
        private TapAndHoldTracker m_DropButtonTracker = new TapAndHoldTracker(0.3f, 0.3f);
        private TapAndHoldTracker m_PickupButtonTracker = new TapAndHoldTracker(0.3f, 0.3f);
        protected override void OnCreate()
        {
            RequireForUpdate<NetworkStreamInGame>();
            RequireForUpdate<SyncedIdToEntityMap>();
            //state.RequireForUpdate<NetCubeSpawner>();
        }

        protected override void OnUpdate()
        {
            var localTransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
            var syncedIdToEntityMap = SystemAPI.ManagedAPI.GetSingleton<SyncedIdToEntityMap>();

            foreach (var (onPlayerRw, onPlayerInputRw) in SystemAPI.Query<RefRW<OnPlayer>, RefRW<OnPlayerInput>>()
                         .WithAll<GhostOwnerIsLocal>())
            {
                var playerInput = onPlayerInputRw.ValueRO;
                playerInput = default;

                if (syncedIdToEntityMap.TryGet(onPlayerRw.ValueRO.ControlledCharacterSyncedId,
                        out var controlledCharacterEntity))
                {
                    var controlledCharacterPosition = localTransformLookup[controlledCharacterEntity].Position;

                    var mousePosInWorld = Utility.GetMousePositionInWorldSpaceXZ();
                    var delta= mousePosInWorld - controlledCharacterPosition;
                    var deltaNormalized = math.normalizesafe(delta);
                    

                    playerInput.MovementInput = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                    playerInput.LookInput = deltaNormalized.xz;

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

                    m_DropButtonTracker.Update(Input.GetKeyDown(KeyCode.Q), Input.GetKey(KeyCode.Q),
                        Input.GetKeyUp(KeyCode.Q), (float)SystemAPI.Time.ElapsedTime, out var holdingDrop,
                        out var tappedDrop, out var releasedAfterHoldDrop);
                    m_PickupButtonTracker.Update(Input.GetKeyDown(KeyCode.E), Input.GetKey(KeyCode.E),
                        Input.GetKeyUp(KeyCode.E), (float)SystemAPI.Time.ElapsedTime, out var holdingPickup,
                        out var tappedPickup, out var releasedAfterHoldPickup);

                    if (tappedDrop)
                    {
                        playerInput.DropButtonTap.Set();
                    }
                    else if (releasedAfterHoldDrop)
                    {
                        playerInput.DropButtonReleasedFromHold.Set();
                    }

                    if (tappedPickup)
                    {
                        playerInput.PickupButtonTap.Set();
                    }
                    else if (releasedAfterHoldPickup)
                    {
                        playerInput.PickupButtonReleasedFromHold.Set();
                    }


                    onPlayerInputRw.ValueRW = playerInput;
                }
            }
        }
    }
    public struct TapAndHoldTracker
    {
        private float m_TapDurationThreshold;
        private float m_ReleasedAfterHoldThreshold;
        private float m_TapStartTime;
        private bool m_IsHolding;

        public TapAndHoldTracker(float tapThreshold, float releasedAfterHoldThreshold)
        {
            m_TapDurationThreshold = tapThreshold;
            m_ReleasedAfterHoldThreshold = releasedAfterHoldThreshold;
            m_TapStartTime = 0f;
            m_IsHolding = false;
        }

        public void Update(bool isDown, bool isPressed, bool isUp, float currentTime, out bool holding, out bool tapped, out bool releasedAfterHold)
        {
            tapped = false;
            holding = false;
            releasedAfterHold = false;

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

                    if (currentTime - m_TapStartTime >= m_ReleasedAfterHoldThreshold)
                    {
                        // Released after hold logic
                        Debug.Log("Released after hold");
                        releasedAfterHold = true;
                    }

                    m_IsHolding = false;
                }
            }
        }
    }
}