using System;
using System.Collections.Generic;
using LabApi.Features.Console;
using MEC;

namespace VeryEpicEventPlugin.Utilities.MEC;

#nullable enable

/// <summary>
/// Simplest form of coroutine.
/// </summary>
public class SimpleCoroutine
{
    /// <summary>
    /// The handle of the coroutine.
    /// </summary>
    public CoroutineHandle? Handle { get; private set; }

    /// <summary>
    /// The original IEnumerator (coroutine) behind the SimpleCoroutine object.
    /// </summary>
    public IEnumerator<float> Original { get; set; }
    
    /// <summary>
    /// Method starting the coroutine.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Method ending the coroutine.
    /// </summary>
    public void End()
    {
        if(Handle.HasValue)
            Timing.KillCoroutines(Handle.Value);
    }
    
    /// <summary>
    /// Constructor creating SimpleCoroutine object out of coroutine
    /// </summary>
    /// <param name="coroutine">IEnumerator{float} object</param>
    public SimpleCoroutine(IEnumerator<float> coroutine)
    {
        Original = coroutine;
    }
}