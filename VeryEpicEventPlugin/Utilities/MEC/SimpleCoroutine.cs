using System;
using System.Collections.Generic;
using LabApi.Features.Console;
using MEC;

namespace VeryEpicEventPlugin.Utilities.MEC;

public class SimpleCoroutine
{
    public CoroutineHandle? Handle { get; private set; }

    public IEnumerator<float> Original { get; set; }
    
    public bool Start()
    {
        End();

        try
        {
            Handle = Timing.RunCoroutine(Original);
            return true;
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return false;
        }
    }

    public void End()
    {
        if(Handle.HasValue)
            Timing.KillCoroutines(Handle.Value);
    }
    
    public SimpleCoroutine(IEnumerator<float> coroutine)
    {
        Original = coroutine;
    }
}