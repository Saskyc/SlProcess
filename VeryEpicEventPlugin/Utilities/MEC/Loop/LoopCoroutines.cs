using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;

namespace VeryEpicEventPlugin.Utilities.MEC.Loop;

public partial class Loop : TimingUtil<Loop>
{
    public override Loop Run()
    {
        try
        {
            Handle.Add(Timing.RunCoroutine(SuperiorCoroutine()));
        }
        catch (Exception e)
        {
            Log.Error($"Loop Exception: {e.Message}");
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
                if (NumberFunction != null)
                {
                    result = NumberFunction().Base;
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