using System;

public struct Option<T> 
{
    private T m_Value;
    private bool m_HasValue;
    public readonly bool TryGet(out T value)
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
        
    public static Option<T> Some(T value)
    {
        return new Option<T>
        {
            m_Value = value,
            m_HasValue = true
        };
    }
        
    public static Option<T> None()
    {
        return new Option<T>
        {
            m_HasValue = false
        };
    }
        
    public static implicit operator Option<T>(T value)
    {
        return Some(value);
    }
    
    // implicit conversion from T
    public static implicit operator T(Option<T> option)
    {
        return option.m_Value;
    }
    
    public override string ToString()
    {
        return Match(
            some: value => $"Some({value})",
            none: () => "None"
        );
    }
}