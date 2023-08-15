using System.Diagnostics;
using DefaultNamespace;

[DebuggerTypeProxy(typeof(BytesAs<,>.DebuggerProxy))]
public struct BytesAs<TField, TDataBytes> where TField : unmanaged where TDataBytes : unmanaged, IDataBytes
{
    public TDataBytes RawData; // it needs to be public so it can be serialized by Unity NetCode
    
    public TField Get() => SerializationUtils.Deserialize<TField, TDataBytes>(ref RawData);
    public void Set(TField value) => SerializationUtils.Serialize(value, ref RawData);
    
    public delegate void EditAction(ref TField value);
    public void Edit(EditAction editAction)
    {
        var value = Get();
        editAction(ref value);
        Set(value);
    }
        
    //ctor and implicit conversion
    public BytesAs(TField value)
    {
        RawData = default;
        Set(value);
    }
    public static implicit operator TField(BytesAs<TField, TDataBytes> bytesAs) => bytesAs.Get();
    public static implicit operator BytesAs<TField, TDataBytes>(TField value) => new(value);
    
    public override string ToString() => Get().ToString();
        
    // debugger proxy
    internal class DebuggerProxy
    {
        private readonly BytesAs<TField, TDataBytes> m_BytesAs;
        public DebuggerProxy(BytesAs<TField, TDataBytes> bytesAs) => m_BytesAs = bytesAs;
        public TField Value => m_BytesAs.Get();
    }
}