using System.Collections.Generic;
using MEC;
using VeryEpicEventPlugin.Enums;

namespace VeryEpicEventPlugin.Utilities;

/// <summary>
/// TimingUtil class used for MEC simplification.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class TimingUtil<T> where T : class
{
    /// <summary>
    /// The time that will be used.
    /// </summary>
    public virtual Cloak Times { get; set; } = Cloak.Second;

    /// <summary>
    /// MEC Handles.
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
    /// Is any handle valid?
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
    /// <returns></returns>
    public abstract T Run();
    
    /// <summary>
    /// Used to kill all handles and clear them.
    /// </summary>
    /// <returns></returns>
    public T Stop()
    {
        foreach (var handle in Handle)
        {
            Timing.KillCoroutines(handle);
        }
        
        Handle.Clear();
        
        return this as T;
    }
    
    /// <summary>
    /// Used to pause all handles and <not> clear them.
    /// </summary>
    /// <returns></returns>
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
    /// <returns></returns>
    public T Resume()
    {
        foreach (var handle in Handle)
        {
            Timing.ResumeCoroutines(handle);
        }
        
        return this as T;
    }
}