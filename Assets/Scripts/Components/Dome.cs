using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct Dome : IComponentData
    {
        // stats
        public float MaxHitPoints;
        public float ProjectileBaseDamage;
        public float ProjectileSpeed;
        public float BestAccuracyDegrees; // 0 degree
        public float WorstAccuracyDegrees; // 15 degreee
        public float RecoilPerShot;
        public float RecoilPerSecond;
        public float FireRate;
        public Entity ProjectilePrefab;
        
        // state
        public float HitPoints;
        public float LastShootTime;
        public float Recoil; //recoil 0: no recoil, keeps increasing as you shoot until it reaches 1
        
        // input 
        public float2 AimDirection;
        public bool ShootInput;
    }
}