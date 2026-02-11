using System;

namespace VeryEpicEventPlugin.Utilities.MEC.Loop;

public partial class Loop
{
    public static implicit operator Loop(Func<int> function)
    {
        return Create(function);
    }
    
    public static implicit operator Loop(Func<float> function)
    {
        return Create(function);
    }
    
    public static implicit operator Loop(Func<double> function)
    {
        return Create(function);
    }
    
    public static implicit operator Loop(Func<short> function)
    {
        return Create(function);
    }
    
    public static implicit operator Loop(Func<long> function)
    {
        return Create(function);
    }
    
    public static implicit operator Loop(Func<Number.Number> function)
    {
        return Create(function);
    }
    
    public static implicit operator Loop((Action function, Number.Number time) tuple)
    {
        return new Loop(tuple.function, tuple.time);
    }
}