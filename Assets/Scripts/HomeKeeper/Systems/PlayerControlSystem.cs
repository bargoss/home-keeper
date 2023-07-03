using HomeKeeper.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

namespace HomeKeeper.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PlayerControlSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var playerInputRw in SystemAPI.Query<RefRW<PlayerAction>>())
            {
                var playerInput = playerInputRw.ValueRO;
                if (SystemAPI.Exists(playerInput.GrabbedEntityOpt))
                {
                    // drop
                    if (playerInput.Drop)
                    {
                        playerInput.GrabbedEntityOpt = Entity.Null;
                    }
                    
                    // drag
                }
                else
                {
                    if (playerInput.Grab)
                    {
                        if (GetFirstGrabObject(playerInput.CameraPosition, playerInput.MouseDirection, out var grabObject))
                        {
                            
                        }
                    }
                }
            }
        }
        private bool GetFirstGrabObject(float3 origin, float3 end, out Entity grabObject)
        {
            grabObject = Entity.Null;
            var collisionWorld = SystemAPI.GetSingleton<BuildPhysicsWorldData>().PhysicsData.PhysicsWorld.CollisionWorld;
            var collisionFilter = CollisionFilter.Default;
            collisionFilter.BelongsTo = CollisionTags.GrabObject;
            var raycastInput = new RaycastInput
            {
                Start = origin,
                End = end,
                Filter = collisionFilter
            };
            if (collisionWorld.CastRay(raycastInput, out var hit))
            {
                grabObject = hit.Entity;
                return true;
            }

            return false;
        }
    }

}