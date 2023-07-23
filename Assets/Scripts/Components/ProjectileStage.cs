using System.Collections.Generic;
using Unity.Entities;

namespace Components
{
    public class HkProjectileStage : IComponentData
    {
        public ProjectileActivationType ActivationType = new ProjectileActivationType.None();
        public List<ProjectileActivationAction> ActivationActions = new();
        
        public List<ProjectileContinuousAction> ContinuousActions = new();
    }

    public struct HkProjectile : IComponentData
    {
        public float TravelledDistance;
        public float SpentTime;
    }
    public struct HkHomingProjectile : IComponentData
    {
        
    }
    
    public struct ForwardThruster : IComponentData
    {
        public float ForcePerSecond;
    }
    
    

    public abstract class ProjectileContinuousAction
    {
        public abstract class Thrust : ProjectileContinuousAction
        {
            public float ForcePerSecond;
        }
        public abstract class Homing : ProjectileContinuousAction
        {
            
        }
    }
    public abstract class ProjectileActivationAction
    {
        public class ActivateStages : ProjectileActivationAction
        {
            public List<HkProjectileStage> Stages = new();
        }
        public class AreaDamage : ProjectileActivationAction
        {
            public float Radius;
            public float Damage;
        }
        public class ForwardImpulse : ProjectileActivationAction
        {
            public float Impulse;
        }
    }

    public abstract class ProjectileActivationType
    {
        public class None : ProjectileActivationType
        {
            
        }
        public class Instant : ProjectileActivationType
        {
            
        }
        public class Duration : ProjectileActivationType
        {
            public float Seconds;
        }
        
        public class Distance : ProjectileActivationType
        {
            public float Meters;
        }
        
        public class Collision : ProjectileActivationType
        {
            
        }
    }
}