using System;

namespace VeryEpicEventPlugin.Utilities.MEC.Loop;

public partial class Loop
{
    public static implicit operator Func<Number.Number>(Loop loop)
    {
        return loop.NumberFunction;
    }
    
    public static implicit operator Func<int>(Loop loop)
    {
        return Number.Number.Convert<int>(loop.NumberFunction);
    }
    
    public static implicit operator Func<float>(Loop loop)
    {
        return Number.Number.Convert<float>(loop.NumberFunction);
    }
    
    public static implicit operator Func<double>(Loop loop)
    {
        return Number.Number.Convert<double>(loop.NumberFunction);
    }
    
    public static implicit operator Func<short>(Loop loop)
    {
        return Number.Number.Convert<short>(loop.NumberFunction);
    }
    
    public static implicit operator Func<long>(Loop loop)
    {
        return Number.Number.Convert<long>(loop.NumberFunction);
    }
}