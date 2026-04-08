using System;
using MEC;
using VeryEpicEventPlugin.Utilities.Struct;

namespace VeryEpicEventPlugin.Utilities.Process;

/// <summary>
/// Represents a process manager that can run loops, delays, events, and custom logic with start/stop control.
/// Supports end conditions, coroutines, and dynamic event subscription.
/// </summary>
public partial class SlProcess
{
    /// <summary>
    /// Executes custom logic when the process starts. Override to add process-specific start behavior.
    /// </summary>
    /// <param name="run">Whether the process should immediately run loops and delays after starting.</param>
    public virtual void CustomStartCode(bool run)
    {
        
    }

    /// <summary>
    /// Executes custom logic when the process ends. Override to add process-specific cleanup behavior.
    /// </summary>
    public virtual void CustomStopCode()
    {
        
    }

    /// <summary>
    /// Starts the process, optionally running loops, delays, and subscribing events.
    /// If the process is already enabled, it first ends the current execution.
    /// </summary>
    /// <param name="run">Whether loops, delays, and holders should execute immediately after start.</param>
    public void Start(bool run = true)
    {
        if (IsEnabled)
        {
            End();
        }
        
        IsEnabled = true;

        CustomStartCode(run);
        
        if (!run)
        {
            return;
        }

        foreach (var i in Loops)
        {
            i.Start(out _);
        }
        
        foreach (var i in Delays)
        {
            i.Start(out _);
        }

        foreach (var i in Holders)
        {
            i.BaseStart(this);
        }

        foreach (var i in Events)
        {
            EventRegistry.Subscribe(i);
        }
    }

    /// <summary>
    /// Stops the process, terminating loops, delays, coroutines, holders, end conditions, and unsubscribing events.
    /// Also executes <see cref="CustomStopCode"/> for custom cleanup.
    /// </summary>
    public void End()
    {
        IsEnabled = false;
            
        foreach (var i in Loops)
        {
            i.Start(out _);
        }

        foreach (var i in Delays)
        {
            i.Start(out _);
        }

        foreach (var i in Handles)
        {
            Timing.KillCoroutines(i);
        }
        
        foreach (var i in Holders)
        {
            i.BaseStop(false);
        }

        foreach (var i in EndCondition)
        {
            i.Stop();
        }
        
        foreach (var i in Events)
        {
            EventRegistry.Unsubscribe(i);
        }
        
        CustomStopCode();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SlProcess"/> with no initial conditions or start behavior.
    /// </summary>
    public SlProcess()
    {
        
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SlProcess"/> with a condition to automatically end the process.
    /// Optionally starts the process immediately.
    /// </summary>
    /// <param name="condition">A function that determines when the process should end.</param>
    /// <param name="checkRateInSeconds">The rate, in seconds, at which the end condition is evaluated.</param>
    /// <param name="start">Whether to immediately start the process after creation.</param>
    public SlProcess(Func<bool> condition, float checkRateInSeconds, bool start = false)
    {
        EndCondition.Add(new ProcessEndCondition(condition, checkRateInSeconds).Anchor(this));
        if (start)
        {
            Start();
        }
    }
}