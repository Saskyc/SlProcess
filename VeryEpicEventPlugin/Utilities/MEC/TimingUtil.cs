using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using VeryEpicEventPlugin.Enums;

namespace VeryEpicEventPlugin.Utilities.MEC;

/// <summary>
/// TimingUtil class used for MEC simplification.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract partial class TimingUtil<T> where T : class
{
    /// <summary>
    /// The time that will be used.
    /// </summary>
    public virtual Cloak Times { get; set; } = Cloak.Second;

    /// <summary>
    /// The actions executed when TimingUtil{T} is stopped.
    /// </summary>
    public List<Action> AfterActions { get; set; } = [];

    /// <summary>
    /// Fills list AfterActions with actions that should happen after.
    /// </summary>
    /// <param name="afterActions">The actions that will happen when handler is stopped.</param>
    /// <returns></returns>
    public T After(params Action[] afterActions)
    {
        AfterActions.AddRange(afterActions);
        return this as T;
    }
    
    /// <summary>
    /// MEC Handle filled and worked with later.
    /// </summary>
    public List<CoroutineHandle> Handle = [];
    
    /// <summary>
    /// Is any handle paused?
    /// </summary>
    public bool Paused
    {
        get
        {
            foreach (var handle in Handle)
            {
                if (handle.IsAliveAndPaused)
                {
                    return true;
                }
            }
            return false;
        }
    }
    
    /// <summary>
    /// Is any handle running?
    /// </summary>
    public bool Running
    {
        get
        {
            foreach (var handle in Handle)
            {
                if (handle.IsRunning)
                {
                    return true;
                }
            }
            return false;
        }
    }
    
    /// <summary>
    /// Checks if any handle is active.
    /// </summary>
    public bool Valid
    {
        get
        {
            foreach (var handle in Handle)
            {
                if (handle.IsValid)
                {
                    return true;
                }
            }
            return false;
        }
    }
    
    /// <summary>
    /// Class used to implement Run.
    /// </summary>
    /// <returns>T object</returns>
    public abstract T Run();
    
    /// <summary>
    /// Used to kill all handles and clear them.
    /// </summary>
    /// <returns>T object</returns>
    public T Stop()
    {
        foreach (var handle in Handle)
        {
            Timing.KillCoroutines(handle);
        }
        
        Handle.Clear();
        AfterActions.ForEach(x =>
        {
            try
            {
                x();
            }
            catch (Exception e)
            {
                Log.Error($"After action exception: {e}");
            }
        });
        
        return this as T;
    }
    
    /// <summary>
    /// Used to pause all handles and <not> clear them.
    /// </summary>
    /// <returns>T object</returns>
    public T Pause()
    {
        foreach (var handle in Handle)
        {
            Timing.PauseCoroutines(handle);
        }

        return this as T;
    }

    /// <summary>
    /// Class used to Resume all handles (after pause).
    /// </summary>
    /// <returns>T object</returns>
    public T Resume()
    {
        foreach (var handle in Handle)
        {
            Timing.ResumeCoroutines(handle);
        }
        
        return this as T;
    }
}