using System.Collections.Generic;
using Obi;
using Obi.MyScenes;
using RunnerGame.Scripts.ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RunnerGame.Scripts.ECS.ViewSystems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ObiParticleRenderFixedUpdateSystem : SystemBase
    {
        protected override void OnCreate()
        {
            
        }
        protected override void OnDestroy()
        {
            
        }
        protected override void OnUpdate()
        {
            var customEmitter = Object.FindObjectOfType<ObiCustomEmitter>();
            var customUpdater = Object.FindObjectOfType<ObiCustomUpdater>();


            //customEmitter.EmitParticle(new Vector3(1,2,0), new Vector3(0,0,0), out var solverIndex);
            
            customEmitter.KillAll();
            
            List<ObiCustomEmitter.ParticleInfo> particleInfos = new();
            
            Entities.ForEach((ref ParticleView particleView, ref LocalTransform localTransform, ref PhysicsVelocity physicsVelocity) =>
            {
                particleInfos.Add(new ObiCustomEmitter.ParticleInfo(localTransform.Position, physicsVelocity.Linear));
                
            }).WithoutBurst().Run();
            
            customEmitter.PushParticles(particleInfos);
            customUpdater.HandleFixedUpdate();
            customUpdater.HandleUpdate();
            customEmitter.PullParticles(particleInfos);
            
            int i = 0;
            Entities.ForEach((ref ParticleView particleView, ref LocalTransform localTransform, ref PhysicsVelocity physicsVelocity) =>
            {
                var particleInfo = particleInfos[i];
                //localTransform.Position = particleInfo.Position;
                //var positionChange = (float3)particleInfo.Position - localTransform.Position;
                //physicsVelocity.Linear = (float3)particleInfo.Velocity + (positionChange / SystemAPI.Time.DeltaTime);
                physicsVelocity.Linear = (float3)particleInfo.Velocity;
                physicsVelocity.Linear*=0.95f;
                physicsVelocity.Angular = float3.zero;
                i++;
            }).WithoutBurst().Run();

            var particleRenderer = Object.FindObjectOfType<ObiParticleRenderer>();
            var meshes = particleRenderer.ParticleMeshes;

            var fluidRenderer = Object.FindObjectOfType<ObiFluidRenderer>();
            var material = fluidRenderer.Fluid_Material;
            
            foreach (var mesh in meshes)
            {
                Graphics.DrawMesh(mesh, Matrix4x4.Translate(new Vector3(0,3,0)), material, 0);
            }



        }
    }

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class ObiParticleRenderUpdateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            
        }
    }


    public class MyObiParticleCollection : IObiParticleCollection
    {
        public List<Vector3> Positions = new();

        const float Radius = 0.5f;
        
        public int particleCount => Positions.Count; 
            
        public int activeParticleCount => Positions.Count;
        public bool usesOrientedParticles => false;
        public int GetParticleRuntimeIndex(int index)
        {
            return index;
        }

        public Vector3 GetParticlePosition(int index)
        {
            return Positions[index];
        }

        public Quaternion GetParticleOrientation(int index)
        {
            return quaternion.identity;
        }

        public void GetParticleAnisotropy(int index, ref Vector4 b1, ref Vector4 b2, ref Vector4 b3)
        {
            //b1 = new Vector4(1, 0, 0, Radius);
            //b2 = new Vector4(0, 1, 0, Radius);
            //b3 = new Vector4(0, 0, 1, Radius);

            b1 = (Vector4)Random.insideUnitSphere + new Vector4(0, 0, 0, Random.Range(0f, 0.99f));
            b2 = (Vector4)Random.insideUnitSphere + new Vector4(0, 0, 0, Random.Range(0f, 0.99f));
            b3 = (Vector4)Random.insideUnitSphere + new Vector4(0, 0, 0, Random.Range(0f, 0.99f));
        }

        public float GetParticleMaxRadius(int index)
        {
            return Radius;
        }

        public Color GetParticleColor(int index)
        {
            return Color.cyan;
        }
    }
}



