using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace BulletCircle
{
    public struct Ball :IComponentData
    {
        public float Money;
        public int MaxBounces;
        public int Bounces;
        
        public static Ball Default => new Ball
        {
            Money = 1,
            MaxBounces = 4,
            Bounces = 0
        };
    }
    public struct BallBouncer :IComponentData
    {
        public float BounceForce;
        public float MoneyMultiplier;
        
        public static BallBouncer Default => new BallBouncer
        {
            BounceForce = 1,
            MoneyMultiplier = 1
        };
    }

    public struct BallShooter : IComponentData
    {
        public float BallMoney;
        public float FireRate;
        public float LastShootTime;
        public float3 ShootDirection;
        public bool Shooting;
        public int BallPrefabIndex;
        
        public static BallShooter Default => new BallShooter
        {
            BallMoney = 1,
            FireRate = 1,
            LastShootTime = 0,
            ShootDirection = float3.zero,
            Shooting = false,
            BallPrefabIndex = 0
        };
    }

    public struct GameState
    {
        public float EarnedMoney;
    }

    public struct GameResources : IComponentData
    {
        public FixedList512Bytes<Entity> BallPrefabs; // support 64 prefabs at most
        public Entity BallShooterPrefab;
    }
}