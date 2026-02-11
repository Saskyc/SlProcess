using System;
using VeryEpicEventPlugin.Enums;

namespace VeryEpicEventPlugin.Utilities.MEC.Loop;

public partial class Loop
{
    public Loop()
    {
        
    }

    public Loop(Action function, Number.Number time, Cloak cloak = Cloak.Second)
    {
        NumberFunction = Func;

        Number.Number Func()
        {
            function();
            return time;
        }

        Times = cloak;
    }
    
    public Loop(Func<int> function, Cloak cloak = Cloak.Second)
    {
        NumberFunction = Number.Number.Convert(function);
        Times = cloak;
    }
    
    public Loop(Func<float> function, Cloak cloak = Cloak.Second)
    {
        NumberFunction = Number.Number.Convert(function);
        Times = cloak;
    }
    
    public Loop(Func<double> function, Cloak cloak = Cloak.Second)
    {
        NumberFunction = Number.Number.Convert(function);
        Times = cloak;
    }
    
    public Loop(Func<short> function, Cloak cloak = Cloak.Second)
    {
        NumberFunction = Number.Number.Convert(function);
        Times = cloak;
    }
    
    public Loop(Func<long> function, Cloak cloak = Cloak.Second)
    {
        NumberFunction = Number.Number.Convert(function);
        Times = cloak;
    }
    
    public Loop(Func<Number.Number> function, Cloak cloak = Cloak.Second)
    {
        NumberFunction = function;
        Times = cloak;
    }
}