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
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
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
            //var customUpdater = Object.FindObjectOfType<ObiCustomUpdater>();


            //customEmitter.EmitParticle(new Vector3(1,2,0), new Vector3(0,0,0), out var solverIndex);
            
            Entities.ForEach((ref ParticleView particleView, in LocalToWorld localToWorld, in PhysicsVelocity physicsVelocity) =>
            {
                if (!particleView.IsInObiSolver)
                {
                    customEmitter.EmitParticle(new Vector3(0,2,0), new Vector3(0,0,0), out var solverIndex);
                    particleView.ObiSolverIndex = solverIndex;
                    particleView.IsInObiSolver = true;
                }
                //customEmitter.SetPosition(particleView.ObiSolverIndex, localToWorld.Position);
                //customEmitter.SetVelocity(particleView.ObiSolverIndex, physicsVelocity.Linear);
                //customEmitter.SetAngularVelocity(particleView.ObiSolverIndex, physicsVelocity.Angular);
            }).WithoutBurst().Run();
            
            //customUpdater.HandleFixedUpdate();
            //customUpdater.HandleUpdate();
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



