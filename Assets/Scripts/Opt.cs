using System;
using JetBrains.Annotations;

public struct Option<T> where T : unmanaged
{
    public T Value;
    public bool HasValue;
    
    // implicit conversion from T
    public static implicit operator Option<T>(T value)
    {
        return new Option<T>
        {
            Value = value,
            HasValue = true
        };
    }
    
    // implicit conversion from None
    public static Option<T> None => new() { HasValue = false };


    [Pure]
    public bool TryGet(out T value)
    {
        value = Value;
        return HasValue;
    }
    
    // get or default
    public T GetOrDefault(T defaultValue)
    {
        return HasValue ? Value : defaultValue;
    }
}


public struct Opt<T> 
{
    public T m_Value;
    public bool m_HasValue;
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
        
    public static Opt<T> Some(T value)
    {
        return new Opt<T>
        {
            m_Value = value,
            m_HasValue = true
        };
    }
        
    public static Opt<T> None()
    {
        return new Opt<T>
        {
            m_HasValue = false
        };
    }
        
    public static implicit operator Opt<T>(T value)
    {
        return Some(value);
    }
    
    // implicit conversion from T
    public static implicit operator T(Opt<T> opt)
    {
        return opt.m_Value;
    }
    
    public override string ToString()
    {
        return Match(
            some: value => $"Some({value})",
            none: () => "None"
        );
    }
}