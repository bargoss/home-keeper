using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Pool;

public abstract class PoolerBase<T> where T : Component 
{
    private T m_Prefab;
    private Transform m_DefaultParent;
    private ObjectPool<T> m_Pool;

    private ObjectPool<T> Pool {
        get {
            if (m_Pool == null) throw new InvalidOperationException("You need to call InitPool before using it.");
            return m_Pool;
        }
        set => m_Pool = value;
    }

    public PoolerBase(T prefab, int defaultCapacity = 10, int max = 20, bool collectionChecks = true, Transform defaultParent = null) {
        m_Prefab = prefab;
        m_DefaultParent = defaultParent;
        Pool = new ObjectPool<T>(
            CreateSetup,
            GetSetup,
            ReleaseSetup,
            DestroySetup,
            collectionChecks,
            defaultCapacity,
            max);
    }
    private PoolerBase() { }


    #region Overrides
    protected virtual T CreateSetup() => GameObject.Instantiate(m_Prefab, m_DefaultParent);
    protected virtual void GetSetup(T cube)
    {
        cube.gameObject.SetActive(true);
        cube.transform.SetParent(null);
        cube.transform.ResetLocal();
    }

    protected virtual void ReleaseSetup(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(m_DefaultParent);
    }

    protected virtual void DestroySetup(T obj) => GameObject.Destroy(obj.gameObject);
    #endregion

    #region Getters
    public T Get() => Pool.Get();
    public void Release(T obj)
    {
        try
        {
            Pool.Release(obj);
        }
        catch (Exception e)
        {
            // todo: there was something wrong with the merger after level is restarted
            Debug.LogError("problem in the pool: " + e);
        }
            
    }

    #endregion
}