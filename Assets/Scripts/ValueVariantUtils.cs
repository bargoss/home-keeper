using System;

using ValueVariant;

public static class ValueVariantUtils{
    public static void Match<TVV, T0, T1>(this TVV valueVariant, Action<T0> action0, Action<T1> action1)
        where TVV : unmanaged, IValueVariant<TVV, T0, T1> where T0 : unmanaged where T1 : unmanaged
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
    public static void Match<TVV, T0, T1, T2>(this TVV valueVariant, Action<T0> action0, Action<T1> action1, Action<T2> action2)
        where TVV : unmanaged, IValueVariant<TVV, T0, T1, T2> where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged
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
    public static void Match<TVV, T0, T1, T2, T3>(this TVV valueVariant, Action<T0> action0, Action<T1> action1, Action<T2> action2, Action<T3> action3)
        where TVV : unmanaged, IValueVariant<TVV, T0, T1, T2, T3> where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged
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
    public static void Match<TVV, T0, T1, T2, T3, T4>(this TVV valueVariant, Action<T0> action0, Action<T1> action1, Action<T2> action2, Action<T3> action3, Action<T4> action4)
        where TVV : unmanaged, IValueVariant<TVV, T0, T1, T2, T3, T4> where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged
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
    public static void Match<TVV, T0, T1, T2, T3, T4, T5>(this TVV valueVariant, Action<T0> action0, Action<T1> action1, Action<T2> action2, Action<T3> action3, Action<T4> action4, Action<T5> action5)
        where TVV : unmanaged, IValueVariant<TVV, T0, T1, T2, T3, T4, T5> where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged
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


}