using Unity.Entities;


public readonly struct ComponentRef<T> where T : unmanaged, IComponentData
{
    private readonly Entity m_Entity;
    
    public RefRW<T> GetRW(ComponentLookup<T> componentLookup)
    {
        return componentLookup.GetRefRW(m_Entity);
    }
    
    public RefRO<T> GetRO(ComponentLookup<T> componentLookup)
    {
        return componentLookup.GetRefRO(m_Entity);
    }
    
    public T Get(ref ComponentLookup<T> componentLookup)
    {
        return componentLookup[m_Entity];
    }
    
    public ComponentRef(Entity entity)
    {
        m_Entity = entity;
    }
}

public readonly struct ComponentsRef<T0, T1>
    where T0 : unmanaged, IComponentData
    where T1 : unmanaged, IComponentData
{
    private readonly Entity m_Entity;
    
    public (RefRW<T0>, RefRW<T1>) GetRW(ComponentLookup<T0> componentLookup0, ComponentLookup<T1> componentLookup1)
    {
        return (componentLookup0.GetRefRW(m_Entity), componentLookup1.GetRefRW(m_Entity));
    }
    
    public (RefRO<T0>, RefRO<T1>) GetRO(ComponentLookup<T0> componentLookup0, ComponentLookup<T1> componentLookup1)
    {
        return (componentLookup0.GetRefRO(m_Entity), componentLookup1.GetRefRO(m_Entity));
    }
    
    public (T0, T1) Get(ref ComponentLookup<T0> componentLookup0, ref ComponentLookup<T1> componentLookup1)
    {
        return (componentLookup0[m_Entity], componentLookup1[m_Entity]);
    }
    
    public ComponentsRef(Entity entity)
    {
        m_Entity = entity;
    }
}


public readonly struct ManagedComponentRef<T> where T : class, IComponentData
{
    private readonly Entity m_Entity;
    
    
    public T Get(ref EntityManager entityManager)
    {
        return entityManager.GetComponentObject<T>(m_Entity);
    }
    
    public ManagedComponentRef(Entity entity)
    {
        m_Entity = entity;
    }
}