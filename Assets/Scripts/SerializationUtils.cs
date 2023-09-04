using System;
using System.Runtime.InteropServices;
using DefaultNamespace;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// create an unsafe context


public static class SerializationUtils
{
    public static unsafe void Serialize<T>(T data, ref FixedBytes510 serializedData) where T : struct
    {
        var ptr = UnsafeUtility.AddressOf(ref serializedData.offset0000);
        UnsafeUtility.CopyStructureToPtr(ref data, ptr);
    }

    public static unsafe T Deserialize<T>(FixedBytes510 serializedData) where T : struct
    {
        var ptr = UnsafeUtility.AddressOf(ref serializedData.offset0000);
        UnsafeUtility.CopyPtrToStructure(ptr, out T result);
        return result;
    }
    
    // for FixedBytes30
    public static unsafe void Serialize<T>(T data, ref FixedBytes30 serializedData) where T : struct
    {
        var ptr = UnsafeUtility.AddressOf(ref serializedData.offset0000);
        UnsafeUtility.CopyStructureToPtr(ref data, ptr);
    }
    
    public static unsafe T Deserialize<T>(FixedBytes30 serializedData) where T : struct
    {
        var ptr = UnsafeUtility.AddressOf(ref serializedData.offset0000);
        UnsafeUtility.CopyPtrToStructure(ptr, out T result);
        return result;
    }
    
    // for FixedString64Bytes
    public static unsafe void Serialize<T>(T data, ref FixedString64Bytes serializedData) where T : struct
    {
        var ptr = UnsafeUtility.AddressOf(ref serializedData);
        UnsafeUtility.CopyStructureToPtr(ref data, ptr);
    }
    
    public static unsafe T Deserialize<T>(FixedString64Bytes serializedData) where T : struct
    {
        var ptr = UnsafeUtility.AddressOf(ref serializedData);
        UnsafeUtility.CopyPtrToStructure(ptr, out T result);
        return result;
    }
    
    // for FixedString512Bytes
    public static unsafe void Serialize<T>(T data, ref FixedString512Bytes serializedData) where T : struct
    {
        var ptr = UnsafeUtility.AddressOf(ref serializedData);
        UnsafeUtility.CopyStructureToPtr(ref data, ptr);
    }
    
    public static unsafe T Deserialize<T>(FixedString512Bytes serializedData) where T : struct
    {
        var ptr = UnsafeUtility.AddressOf(ref serializedData);
        UnsafeUtility.CopyPtrToStructure(ptr, out T result);
        return result;
    }
    
    // for FixedString32Bytes
    public static unsafe void Serialize<T>(T data, ref FixedString32Bytes serializedData) where T : struct
    {
        var ptr = UnsafeUtility.AddressOf(ref serializedData);
        UnsafeUtility.CopyStructureToPtr(ref data, ptr);
    }
    
    public static unsafe T Deserialize<T>(FixedString32Bytes serializedData) where T : struct
    {
        var ptr = UnsafeUtility.AddressOf(ref serializedData);
        UnsafeUtility.CopyPtrToStructure(ptr, out T result);
        return result;
    }
    
    
    public static unsafe void Serialize<T, TDataBytes>(T data, ref TDataBytes serializedData) 
        where T : unmanaged where TDataBytes : unmanaged, IDataBytes
    {
        // Debug.LogError if T size is bigger than TDataBytes size
        if (sizeof(T) > sizeof(TDataBytes))
        {
            // dont use typeof
            throw new Exception("size of T is bigger than size of TDataBytes by " + (sizeof(T) - sizeof(TDataBytes)) + " bytes");
        }
        var ptr = UnsafeUtility.AddressOf(ref serializedData);
        UnsafeUtility.CopyStructureToPtr(ref data, ptr);
    }
    
    public static unsafe T Deserialize<T, TDataBytes>(ref TDataBytes serializedData) 
        where T : unmanaged where TDataBytes : unmanaged, IDataBytes
    {
        // Debug.LogError if T size is bigger than TDataBytes size
        
        if (sizeof(T) > sizeof(TDataBytes))
        {
            throw new Exception("size of T is bigger than size of TDataBytes by " + (sizeof(T) - sizeof(TDataBytes)) + " bytes");
        }
        
        var ptr = UnsafeUtility.AddressOf(ref serializedData);
        UnsafeUtility.CopyPtrToStructure(ptr, out T result);
        return result;
    }
}