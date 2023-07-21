using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Obi;
using Obi.MyScenes;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObiExperiment : MonoBehaviour
{
    public ObiEmitter Emitter;

    public ObiSolver Solver;
    public ObiParticleRenderer ObiParticleRenderer;
    public ObiCustomEmitter ObiCustomEmitter;
    
    public ObiCustomUpdater ObiCustomUpdater;
    // Start is called before the first frame update
    void Start()
    {
        //ObiParticleRenderer.ParticleMaterial = GameResources.Instance.ObiParticleMaterial;
        //m_ObiCustomUpdater = Solver.gameObject.AddComponent<ObiCustomUpdater>();
    }

    // Update is called once per frame

    private List<int> particleIds = new();

    private List<ObiCustomEmitter.ParticleInfo> ParticleInfos = new();
    private float SpawnedCount = 0;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            //ObiCustomEmitter.EmitParticle(0.2f);
            ObiCustomEmitter.EmitParticle(Random.insideUnitSphere * 0.01f, Vector3.zero, out var particleId );
        }
        
    }

    void FixedUpdate()
    {
        return;
        //ObiCustomEmitter.PullParticles(ParticleInfos);

        if (Input.GetKey(KeyCode.Space))
        {
            //ObiCustomEmitter.EmitParticle(-Vector3.right * 3, Vector3.up * 0, out var particleId );
            //particleIds.Add(particleId);

            ParticleInfos.Add(new ObiCustomEmitter.ParticleInfo()
            {
                Position = -Vector3.right * SpawnedCount * 10.11f + Vector3.up * Time.time * 0.1f,
                Velocity = Vector3.up * 0,
            });

            SpawnedCount++;
        }

        ObiCustomEmitter.PushParticles(ParticleInfos);

        if (ObiCustomUpdater != null)
        {
            ObiCustomUpdater.HandleFixedUpdate();
            ObiCustomUpdater.HandleUpdate();
        }

        ObiCustomEmitter.PullParticles(ParticleInfos);

        //ObiCustomEmitter.PushParticles(ParticleInfos);
    }

    void FixedUpdate2()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ObiCustomEmitter.EmitParticle(-Vector3.right * 3, Vector3.up * 0, out var particleId );
            particleIds.Add(particleId);
        }

        if (Input.GetKey(KeyCode.A))
        {
            for (var i = 0; i < particleIds.Count; i++)
            {
                var particleId = particleIds[i];
                //ObiCustomEmitter.SetPosition(i, (Vector3.up * (Time.time % 3f))  +Vector3.right * ((i * 0.1f) % 2));
                //ObiCustomEmitter.SetVelocity(i, Vector3.zero);
                
                //ObiCustomEmitter.SetVelocity(particleId, Vector3.zero);
            }
        }
            
        
        
        
    }
}