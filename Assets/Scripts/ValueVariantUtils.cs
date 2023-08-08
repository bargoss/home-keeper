using System;
using ValueVariant;
using ValueVariant.Details;

public static class ValueVariantUtils{
    public static void Switch<TVV, T0, T1>(this TVV valueVariant, Action<T0> action0, Action<T1> action1)
        where TVV : unmanaged, ValueVariant.IValueVariant<TVV, T0, T1> where T0 : unmanaged where T1 : unmanaged
    {
        switch (valueVariant.TypeIndex)
        {
            case 1:
                action0(valueVariant.Item1);
                break;
            case 2:
                action1(valueVariant.Item2);
                break;
        }
    }
        
    // with 3
    public static void Switch<TVV, T0, T1, T2>(this TVV valueVariant, Action<T0> action0, Action<T1> action1, Action<T2> action2)
        where TVV : unmanaged, ValueVariant.IValueVariant<TVV, T0, T1, T2> where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged
    {
        switch (valueVariant.TypeIndex)
        {
            case 1:
                action0(valueVariant.Item1);
                break;
            case 2:
                action1(valueVariant.Item2);
                break;
            case 3:
                action2(valueVariant.Item3);
                break;
        }
    }
        
    // with 4
    public static void Switch<TVV, T0, T1, T2, T3>(this TVV valueVariant, Action<T0> action0, Action<T1> action1, Action<T2> action2, Action<T3> action3)
        where TVV : unmanaged, ValueVariant.IValueVariant<TVV, T0, T1, T2, T3> where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged
    {
        switch (valueVariant.TypeIndex)
        {
            case 1:
                action0(valueVariant.Item1);
                break;
            case 2:
                action1(valueVariant.Item2);
                break;
            case 3:
                action2(valueVariant.Item3);
                break;
            case 4:
                action3(valueVariant.Item4);
                break;
        }
    }
        
    // with 5
    public static void Switch<TVV, T0, T1, T2, T3, T4>(this TVV valueVariant, Action<T0> action0, Action<T1> action1, Action<T2> action2, Action<T3> action3, Action<T4> action4)
        where TVV : unmanaged, ValueVariant.IValueVariant<TVV, T0, T1, T2, T3, T4> where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged
    {
        switch (valueVariant.TypeIndex)
        {
            case 1:
                action0(valueVariant.Item1);
                break;
            case 2:
                action1(valueVariant.Item2);
                break;
            case 3:
                action2(valueVariant.Item3);
                break;
            case 4:
                action3(valueVariant.Item4);
                break;
            case 5:
                action4(valueVariant.Item5);
                break;
        }
    }
        
    // with 6
    public static void Switch<TVV, T0, T1, T2, T3, T4, T5>(this TVV valueVariant, Action<T0> action0, Action<T1> action1, Action<T2> action2, Action<T3> action3, Action<T4> action4, Action<T5> action5)
        where TVV : unmanaged, ValueVariant.IValueVariant<TVV, T0, T1, T2, T3, T4, T5> where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged
    {
        switch (valueVariant.TypeIndex)
        {
            case 1:
                action0(valueVariant.Item1);
                break;
            case 2:
                action1(valueVariant.Item2);
                break;
            case 3:
                action2(valueVariant.Item3);
                break;
            case 4:
                action3(valueVariant.Item4);
                break;
            case 5:
                action4(valueVariant.Item5);
                break;
            case 6:
                action5(valueVariant.Item6);
                break;
        }
    }
    
    // with func with generic return
    public static void Match<TVV, T0, T1, TR>(this TVV valueVariant, Func<T0, TR> func0, Func<T1, TR> func1)
        where TVV : unmanaged, ValueVariant.IValueVariant<TVV, T0, T1> where T0 : unmanaged where T1 : unmanaged
    {
        switch (valueVariant.TypeIndex)
        {
            case 1:
                func0(valueVariant.Item1);
                break;
            case 2:
                func1(valueVariant.Item2);
                break;
        }
    }
    
    // with 3
    public static void Match<TVV, T0, T1, T2, TR>(this TVV valueVariant, Func<T0, TR> func0, Func<T1, TR> func1, Func<T2, TR> func2)
        where TVV : unmanaged, ValueVariant.IValueVariant<TVV, T0, T1, T2> where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged
    {
        switch (valueVariant.TypeIndex)
        {
            case 1:
                func0(valueVariant.Item1);
                break;
            case 2:
                func1(valueVariant.Item2);
                break;
            case 3:
                func2(valueVariant.Item3);
                break;
        }
    }
    
    // with 4
    public static void Match<TVV, T0, T1, T2, T3, TR>(this TVV valueVariant, Func<T0, TR> func0, Func<T1, TR> func1, Func<T2, TR> func2, Func<T3, TR> func3)
        where TVV : unmanaged, ValueVariant.IValueVariant<TVV, T0, T1, T2, T3> where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged
    {
        switch (valueVariant.TypeIndex)
        {
            case 1:
                func0(valueVariant.Item1);
                break;
            case 2:
                func1(valueVariant.Item2);
                break;
            case 3:
                func2(valueVariant.Item3);
                break;
            case 4:
                func3(valueVariant.Item4);
                break;
        }
    }
    
    // with 5
    public static void Match<TVV, T0, T1, T2, T3, T4, TR>(this TVV valueVariant, Func<T0, TR> func0, Func<T1, TR> func1, Func<T2, TR> func2, Func<T3, TR> func3, Func<T4, TR> func4)
        where TVV : unmanaged, ValueVariant.IValueVariant<TVV, T0, T1, T2, T3, T4> where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged
    {
        switch (valueVariant.TypeIndex)
        {
            case 1:
                func0(valueVariant.Item1);
                break;
            case 2:
                func1(valueVariant.Item2);
                break;
            case 3:
                func2(valueVariant.Item3);
                break;
            case 4:
                func3(valueVariant.Item4);
                break;
            case 5:
                func4(valueVariant.Item5);
                break;
        }
    }
    
    // with 6
    public static void Match<TVV, T0, T1, T2, T3, T4, T5, TR>(this TVV valueVariant, Func<T0, TR> func0,
        Func<T1, TR> func1, Func<T2, TR> func2, Func<T3, TR> func3, Func<T4, TR> func4, Func<T5, TR> func5)
        where TVV : unmanaged, ValueVariant.IValueVariant<TVV, T0, T1, T2, T3, T4, T5>
        where T0 : unmanaged
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
        where T5 : unmanaged
    {
        switch (valueVariant.TypeIndex)
        {
            case 1:
                func0(valueVariant.Item1);
                break;
            case 2:
                func1(valueVariant.Item2);
                break;
            case 3:
                func2(valueVariant.Item3);
                break;
            case 4:
                func3(valueVariant.Item4);
                break;
            case 5:
                func4(valueVariant.Item5);
                break;
            case 6:
                func5(valueVariant.Item6);
                break;
        }
    }


}