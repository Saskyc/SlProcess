using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using VeryEpicEventPlugin.Enums;

namespace VeryEpicEventPlugin.Utilities;

public class Loop : TimingUtil<Loop>
{
    #nullable enable
    private Func<int>? IntFunction { get; set; }
    private Func<float>? FloatFunction { get; set; }
    private Func<double>? DoubleFunction { get; set; }
    
    private Func<short>? ShortFunction { get; set; }
    
    private Func<long>? LongFunction { get; set; }
    
    #nullable disable
    
    public float TimeUsed(float delay) => Times switch
    {
        Cloak.Milisecond => delay * 1000f,
        Cloak.Second     => delay,
        Cloak.Minute     => delay / 60f,
        Cloak.Hour       => delay / 3600f,
        Cloak.Day        => delay / 86400f,
        Cloak.Week       => delay / 604800f,
        Cloak.Month      => delay / 2592000f,
        Cloak.Year       => delay / 31536000f,
        _                => 0f
    };
    
    public Loop()
    {
        
    }

    public Loop(Action function, int time, Cloak cloak = Cloak.Second)
    {
        IntFunction = Func;

        int Func()
        {
            function();
            return time;
        }

        Times = cloak;
    }
    
    public Loop(Action function, double time, Cloak cloak = Cloak.Second)
    {
        DoubleFunction = Func;

        double Func()
        {
            function();
            return time;
        }

        Times = cloak;
    }
    
    public Loop(Action function, short time, Cloak cloak = Cloak.Second)
    {
        ShortFunction = Func;

        short Func()
        {
            function();
            return time;
        }

        Times = cloak;
    }
    
    public Loop(Action function, long time, Cloak cloak = Cloak.Second)
    {
        LongFunction = Func;

        long Func()
        {
            function();
            return time;
        }

        Times = cloak;
    }
    
    public Loop(Action function, float time, Cloak cloak = Cloak.Second)
    {
        FloatFunction = Func;

        float Func()
        {
            function();
            return time;
        }

        Times = cloak;
    }
    
    public Loop(Func<int> function, Cloak cloak = Cloak.Second)
    {
        IntFunction = function;
        Times = cloak;
    }
    
    public Loop(Func<float> floatFunction, Cloak cloak = Cloak.Second)
    {
        FloatFunction = floatFunction;
        Times = cloak;
    }

    public Loop(Func<double> doubleFunction, Cloak cloak = Cloak.Second)
    {
        DoubleFunction = doubleFunction;
        Times = cloak;
    }
    
    public Loop(Func<short> shortFunction, Cloak cloak = Cloak.Second)
    {
        ShortFunction = shortFunction;
        Times = cloak;
    }
    
    public Loop(Func<long> longFunction, Cloak cloak = Cloak.Second)
    {
        LongFunction = longFunction;
        Times = cloak;
    }

    public override Loop Run()
    {
        try
        {
            Handle.Add(Timing.RunCoroutine(SuperiorCoroutine()));
        }
        catch (Exception e)
        {
            Log.Error($"Fuckass error message: {e.Message}");
        }
        
        return this;
    }

    private IEnumerator<float> SuperiorCoroutine()
    {
        for (;;)
        {
            float result = 0;
            
            try
            {
                if (FloatFunction != null)
                {
                    result = FloatFunction();
                }

                if (IntFunction != null)
                {
                    result = IntFunction();
                }
                
                if (DoubleFunction != null)
                {
                    result = (float)DoubleFunction();
                }
                
                if (ShortFunction != null)
                {
                    result = ShortFunction();
                }
                
                
                if (LongFunction != null)
                {
                    result = LongFunction();
                }
            }
            catch (Exception e)
            {
                Log.Error("Loop error " + e.Message);
                yield break;
            }
            
            if (result == 0)
            {
                yield break;
            }
            yield return Timing.WaitForSeconds(TimeUsed(result));
        }
    }
}