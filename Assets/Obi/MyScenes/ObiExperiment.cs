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
            for (int i = 0; i < 10; i++)
            {
                ForeachPointOnUnitCircle(Vector3.zero, Vector3.forward, 10, point =>
                {
                    ObiCustomEmitter.EmitParticle(point * 1.0f, Vector3.zero, out var particleId );
                });
                //ObiCustomEmitter.EmitParticle(Vector3.right * i * 0.05f, Vector3.zero, out var particleId );
            }
            //ObiCustomEmitter.EmitParticle(Random.insideUnitSphere * 0.01f, Vector3.zero, out var particleId );
        }

        if (Input.GetKey(KeyCode.W))
        {
            ObiCustomEmitter.KillAll();
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
    
    private void ForeachPointOnUnitCircle(Vector3 origin, Vector3 circleAxis, int pointsCount, Action<Vector3> action)
    {
        var angleStep = 360f / pointsCount;
        for (var i = 0; i < pointsCount; i++)
        {
            var angle = angleStep * i;
            var point = Quaternion.AngleAxis(angle, circleAxis) * Vector3.right;
            action(point);
        }
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