using System;
using System.Collections;
using System.Collections.Generic;
using Exiled.API.Features;
using LabApi.Features.Console;
using MEC;

namespace VeryEpicEventPlugin;

public class Loop
{
    public Func<float> Function { get; set; }

    public CoroutineHandle? Handle { get; set; }
    
    public Loop(Func<float> function)
    {
        Function = function;
    }

    public Loop Run()
    {
        Handle = Timing.RunCoroutine(PrRun());
        return this;
    }
    
    private IEnumerator<float> PrRun()
    {
        for (;;)
        {
            float result;
            
            try
            {
                result = Function();
            }
            catch (Exception e)
            {
                Log.Error("Loop error " + e.Message);
                yield break;
            }
            

            if (result < 0)
            {
                yield break;
            }

            yield return result;
        }
    }
    
    public Loop Stop()
    {
        if (Handle == null)
        {
            return this;
        }
        
        Timing.KillCoroutines(Handle.Value);
        return this;
    }
}