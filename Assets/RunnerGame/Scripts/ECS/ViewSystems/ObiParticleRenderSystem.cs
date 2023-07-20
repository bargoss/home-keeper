using System.Collections.Generic;
using Obi;
using RunnerGame.Scripts.ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RunnerGame.Scripts.ECS.ViewSystems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class ObiParticleRenderFixedUpdateSystem : SystemBase
    {
        private ParticleImpostorRendering m_Impostors;
        private MyObiParticleCollection m_ParticleCollection;
        private Material ParticleMaterial => GameResources.Instance.ObiParticleMaterial;

        protected override void OnCreate()
        {
            m_ParticleCollection = new MyObiParticleCollection();
            m_Impostors = new ParticleImpostorRendering();
        }
        protected override void OnDestroy()
        {
            m_ParticleCollection = null;
            m_Impostors = null;
        }
        protected override void OnUpdate()
        {
            //var particleRenderer = Object.FindObjectOfType<ObiParticleRenderer>();
            var impostors = m_Impostors;
            m_ParticleCollection.Positions.Clear();
            
            Entities.ForEach((in ParticleView particleView, in LocalToWorld localToWorld) =>
            {
                m_ParticleCollection.Positions.Add(localToWorld.Position);
            }).WithoutBurst().Run();
            impostors.UpdateMeshes(m_ParticleCollection);
            DrawParticles(impostors.Meshes);
        }

        private void DrawParticles(IEnumerable<Mesh> meshes)
        {
            //ParticleMaterial.SetFloat("_RadiusScale", 0.45f);
            //ParticleMaterial.SetColor("_Color", Color.cyan);
            
            ParticleMaterial.SetFloat("_RadiusScale", 1);
            ParticleMaterial.SetColor("_Color", Color.white);
            
            foreach (Mesh mesh in meshes)
                Graphics.DrawMesh(mesh, Matrix4x4.identity, ParticleMaterial, 0);
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



