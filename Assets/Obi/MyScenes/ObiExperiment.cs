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

        for (int i = 0; i < 5; i++)
        {
            ForeachPointOnUnitCircle(Vector3.zero, Vector3.forward, 10, point =>
            {
                ParticleInfos.Add(new ObiCustomEmitter.ParticleInfo(point * 0.25f * (i +1), Vector3.zero));
                //ObiCustomEmitter.EmitParticle(point * 1.0f, Vector3.zero, out var particleId );
            });
        }
        ParticleInfos.Add(new ObiCustomEmitter.ParticleInfo(new Vector3(0,0,0.01f), Vector3.zero));
    }

    // Update is called once per frame

    private List<int> particleIds = new();

    private List<ObiCustomEmitter.ParticleInfo> ParticleInfos = new();
    private float SpawnedCount = 0;

    private void Update()
    {
        return;
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

    private void AddForce(List<ObiCustomEmitter.ParticleInfo> particleInfos, Vector3 force)
    {
        for (var i = 0; i < particleInfos.Count; i++)
        {
            var particleInfo = particleInfos[i];
            particleInfo.Velocity += force;
            particleInfos[i] = particleInfo;
        }
    }
    void FixedUpdate()
    {
        //ObiCustomEmitter.PullParticles(ParticleInfos);

        //if (Input.GetKey(KeyCode.Space))
        //{
        //    ParticleInfos.Add(new ObiCustomEmitter.ParticleInfo()
        //    {
        //        Position = -Vector3.right * SpawnedCount * 10.11f + Vector3.up * Time.time * 0.1f,
        //        Velocity = Vector3.up * 0,
        //    });
        //    SpawnedCount++;
        //}
        
        
        HandleCircularGravity(ParticleInfos);
        if (Input.GetKey(KeyCode.Space))
        {
            AddForce(ParticleInfos, Vector3.up * 0.5f);
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

    private void HandleCircularGravity(List<ObiCustomEmitter.ParticleInfo> particleInfos)
    {
        for (var i = 0; i < particleInfos.Count; i++)
        {
            //var particleInfo = particleInfos[i];
            //var distance = Vector3.Distance(particleInfo.Position, Vector3.zero);
            //var gravity = Vector3.down * 0.1f * distance;
            //particleInfo.Velocity += gravity;
            //particleInfos[i] = particleInfo;
            
            //like that but pull towards the origin
            var particleInfo = particleInfos[i];
            var deltaToCenter = Vector3.zero - particleInfo.Position;
            var distance = deltaToCenter.magnitude;
            var gravity = deltaToCenter.normalized * 0.1f * distance;
            particleInfo.Velocity += gravity * Time.deltaTime * 50;
            particleInfos[i] = particleInfo;
        }
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