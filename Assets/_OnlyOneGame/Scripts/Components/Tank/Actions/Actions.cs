using _OnlyOneGame.Scripts.Components.Data;
using Components;
using DefaultNamespace;
using DefenderGame.Scripts.Components;
using HomeKeeper.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace _OnlyOneGame.Scripts.Components.Tank.Actions
{
    public struct Aiming
    {
        public float3 TargetPosition;
        public TankActionCard TankActionCard;
    }
    
    public struct OnGoingTankAction
    {
        public TankActionCard ActionCard;
        public Data ActionData;
        
        public struct Data
        {
            public NetworkTick StartTick;
            public float3 TargetPosition;
            public int Stage;
        }
    }

    public enum TankActionCard
    {
        Jump,
        Move,
        
        LandMine,
        Deflect,
        Shield,
        Barricade,
        
        HeavyShot,
        SpreadShot,
        LightShot,
        BurstShot,
        ClusterShot,
        
        DeployFastBots,
        DeployHeavyBots,
        DeployRangedBots,
    }

    public struct ActionHandlerContext
    {
        public OnGoingTankAction.Data ActionData {get; set;}
        public CharacterMovement CharacterMovement {get; set;}
        public EntityCommandBuffer Ecb {get; set;}

        public OnPrefabs Prefabs {get;}
        public BuildPhysicsWorldData Physics {get;}
        public LocalTransform LocalTransform {get;}
        public NetworkTick Tick {get;}
        public int FixedTickRate {get;}
        public bool IsFinished {get; set; }

        public ComponentLookup<PhysicsVelocity> PhysicsVelocityLookup {get;}
        // . . .
        
        // ctor:
        public ActionHandlerContext(
            OnGoingTankAction.Data actionData,
            CharacterMovement characterMovement,
            EntityCommandBuffer ecb,
            OnPrefabs prefabs,
            BuildPhysicsWorldData physics,
            LocalTransform localTransform,
            NetworkTime networkTime,
            ClientServerTickRate clientServerTickRate,
            ComponentLookup<PhysicsVelocity> physicsVelocityLookup
        )
        {
            ActionData = actionData;
            CharacterMovement = characterMovement;
            Ecb = ecb;
            Physics = physics;
            Prefabs = prefabs;
            LocalTransform = localTransform;
            Tick = networkTime.ServerTick;
            FixedTickRate = clientServerTickRate.SimulationTickRate;
            PhysicsVelocityLookup = physicsVelocityLookup;
            
            IsFinished = false;
        }


        public void ApplyBack(ref OnGoingTankAction.Data data, ref CharacterMovement characterMovement, ref EntityCommandBuffer ecb)
        {
            ActionData = data;
            CharacterMovement = characterMovement;
            Ecb = ecb;
        }
    };
    
    public static class ActionHandlers
    {
        public static void HowToGetTickRate()
        {
            var tickRate = new ClientServerTickRate().SimulationTickRate;
        }

        private static bool CheckFinished(in ActionHandlerContext context, float durationSeconds)
        {
            var durationInTicks =  (int)math.ceil(context.FixedTickRate * durationSeconds);
            var finishTick = context.ActionData.StartTick;
            finishTick.Add((uint)durationInTicks);
            
            return context.Tick.TicksSince(finishTick) >= 0;
        }
        
        public static void Update_Move(
            ref ActionHandlerContext context
        )
        {
            var characterMovement = context.CharacterMovement;
            characterMovement.MovementInputAsXZ = (context.ActionData.TargetPosition - context.LocalTransform.Position).ClampMagnitude(0.5f);
            context.CharacterMovement = characterMovement;
            
            if(CheckFinished(context, 0.5f))
            {
                context.IsFinished = true;
            }
        }
        
    }
}