using System.Collections.Generic;
using MEC;
using VeryEpicEventPlugin.Enums;

namespace VeryEpicEventPlugin.Utilities;

public abstract class TimingUtil<T> where T : class
{
    public virtual Cloak Times { get; set; } = Cloak.Second;

    public List<CoroutineHandle> Handle = [];
    
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
    
    public abstract T Run();
    
    public T Stop()
    {
        foreach (var handle in Handle)
        {
            Timing.KillCoroutines(handle);
        }
        
        Handle.Clear();
        
        return this as T;
    }
    
    public T Pause()
    {
        foreach (var handle in Handle)
        {
            Timing.PauseCoroutines(handle);
        }

        return this as T;
    }

    public T Resume()
    {
        foreach (var handle in Handle)
        {
            Timing.ResumeCoroutines(handle);
        }
        
        return this as T;
    }
}