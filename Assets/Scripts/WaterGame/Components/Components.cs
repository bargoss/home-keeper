using SpacialIndexing;
using Unity.Entities;

namespace WaterGame.Components
{
    public struct SpacialPartitioningSingleton : IComponentData
    {
        public SpacialPartitioning<Entity> Partitioning;
    }
    public struct SpacialPartitioningEntry : IComponentData { }
    public struct Particle : IComponentData { }
    
    public struct WaterGameConfig : IComponentData
    {
        public float PushForce;
        public float Viscosity;
        public float InnerRadius;
        public float OuterRadius;
    }
    
    public struct MyPair<T> where T: unmanaged
    {
        public T A;
        public T B;

        public MyPair(T a, T b)
        {
            A = a;
            B = b;
        }
    }
}