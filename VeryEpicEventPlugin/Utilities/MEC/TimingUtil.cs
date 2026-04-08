using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using VeryEpicEventPlugin.Enums;
using VeryEpicEventPlugin.Utilities.Struct;

namespace VeryEpicEventPlugin.Utilities.MEC;

/// <summary>
/// TimingUtil class used for MEC simplification.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract partial class TimingUtil<T> where T : class
{
    /// <summary>
    /// Type of time that should be used.
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
    public MethodResult<T> After(params Action[] afterActions)
    {
        if (afterActions == null)
        {
            return new MethodResult<T>(this as T, new NullReferenceException());
        }
        AfterActions.AddRange(afterActions);
        return this as T;
    }
    
    /// <summary>
    /// MEC Handle of this whole wrapper.
    /// </summary>
    public CoroutineHandle? Handle = null;

    /// <summary>
    /// Property defining if methods should log exceptions or only return them via <see cref="MethodResult{T}"/>
    /// </summary>
    public bool LogExceptions { get; set; } = false;

    /// <summary>
    /// Checks if <see cref="TimingUtil{T}.Handle"/> is not null.
    /// </summary>
    public bool HasHandle 
    { 
        get
        {
            return Handle != null;
        } 
    }

    /// <summary>
    /// Checks if <see cref="TimingUtil{T}.Handle"/> is paused.
    /// </summary>
    public bool IsPaused
    {
        get
        {
            if (!HasHandle) return true;
            return Handle.Value.IsAliveAndPaused;
        }
    }

    /// <summary>
    /// Checks if <see cref="TimingUtil{T}.Handle"/> is running.
    /// </summary>
    public bool IsRunning
    {
        get
        {
            if (!HasHandle) return false;
            return Handle.Value.IsRunning;
        }
    }
    
    /// <summary>
    /// Checks if <see cref="TimingUtil{T}.Handle"/> is valid
    /// </summary>
    public bool Valid
    {
        get
        {
            if (!HasHandle) return false;
            return Handle.Value.IsValid;
        }
    }
    
    /// <summary>
    /// Starts the Coroutine. Also creates handle <see cref="TimingUtil{T}.CreateHandle"/> while starting, if already contained then Disposes <see cref="TimingUtil{T}.Dispose"/>
    /// If unsuccessfully started (or created) then it will be attempted to dispose via <see cref="TimingUtil{T}.Dispose"/>.
    /// </summary>
    /// <param name="exception">Exception caused if unsucsesfully executed</param>
    /// <returns><see cref="MethodResult{bool}"/></returns>
    public MethodResult<bool> Start(out Exception? exception)
    {
        exception = null;
        try
        {
            if (HasHandle)
            {
                Kill();
            }
            var creationResult = CreateHandle();
            if(creationResult.Exception != null)
            {
                Dispose();
                return new MethodResult<bool>(false, creationResult.Exception);
            }
            return HasHandle;
        }
        catch (Exception e)
        {
            Dispose();
            return new MethodResult<bool>(false, e);
        }
    }

    /// <summary>
    /// Safely disposes the Handle via killing and removing <see cref="TimingUtil{T}.Kill"/> & <see cref="TimingUtil{T}.RemoveHandle"/>
    /// </summary>
    /// <returns>If didn't have handle or wasn't sucsessfully disposed then returns false.</returns>
    public MethodResult<bool> Dispose()
    {
        if (!HasHandle) return false;
        try
        {
            Kill();
            RemoveHandle();
            return true;
        }
        catch (Exception e)
        {
            return new MethodResult<bool>(false, e);
        }
    }

    /// <summary>
    /// Method to create handle.
    /// </summary>
    /// <returns>T object</returns>
    public abstract MethodResult<T> CreateHandle();

    /// <summary>
    /// Kills the coroutine and executes <see cref="AfterActions"/>
    /// Makes sure <see cref="TimingUtil{T}.AfterActions"/> exists. 
    /// Doesn't remove the <see cref="Handle"/> method <see cref="TimingUtil{T}.RemoveHandle"/> does that.
    /// </summary>
    /// <returns><see cref="MethodResult{bool}"/> | <see cref="MethodResult{bool}.Exception"/> if any will be of last Action executed.</returns>
    public MethodResult<bool> Kill()
    {
        if (HasHandle)
        {
            Timing.KillCoroutines(Handle.Value);
        }
        
        if(AfterActions == null)
        {
            AfterActions = [];
            return new MethodResult<bool>(false, new NullReferenceException());
        }

        Exception? finalException = null;
        AfterActions.ForEach(afterAction =>
        {
            try
            {
                afterAction();
            }
            catch (Exception e)
            {
                if(finalException == null)
                {
                    finalException = e;
                }
            }
        });
        
        return new MethodResult<bool>(true, finalException);
    }

    /// <summary>
    /// Removes handle from existing, doesn't kill <see cref="TimingUtil{T}.Kill"/>
    /// </summary>
    /// <returns>True if removed.</returns>
    public bool RemoveHandle()
    {
        if (HasHandle)
        {
            Handle = null;
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Used to pause handle and not kill them.
    /// Will pause only is <see cref="TimingUtil{T}.IsPaused"/> is false.
    /// </summary>
    /// <returns>T object</returns>
    public T Pause()
    {
        if (!HasHandle) return this as T;
        if (IsPaused) return this as T;
        Timing.PauseCoroutines(Handle.Value);

        return this as T;
    }

    /// <summary>
    /// Class used to Resumes handle.
    /// Will only happen if <see cref="TimingUtil{T}.IsPaused"/> is true aka <see cref="TimingUtil{T}.Pause"/> was called.
    /// </summary>
    /// <returns>T object</returns>
    public T Resume()
    {
        if (!HasHandle) return this as T;
        if (!IsPaused) return this as T;
        Timing.ResumeCoroutines(Handle.Value);
        
        return this as T;
    }
}