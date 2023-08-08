using System;

public struct Optf<T> 
{
    private T m_Value;
    private bool m_HasValue;
    public bool TryGet(out T value)
    {
        value = m_Value;
        return m_HasValue;
    }
    
    public TR Match<TR>(Func<T, TR> some, Func<TR> none)
    {
        return m_HasValue ? some(m_Value) : none();
    }
    
    public T GetOrDefault(T defaultValue)
    {
        return m_HasValue ? m_Value : defaultValue;
    }
        
    public void Set(T value)
    {
        m_Value = value;
        m_HasValue = true;
    }
        
    public void Clear()
    {
        m_HasValue = false;
    }
        
    public static Optf<T> Some(T value)
    {
        return new Optf<T>
        {
            m_Value = value,
            m_HasValue = true
        };
    }
        
    public static Optf<T> None()
    {
        return new Optf<T>
        {
            m_HasValue = false
        };
    }
        
    public static implicit operator Optf<T>(T value)
    {
        return Some(value);
    }
        
}