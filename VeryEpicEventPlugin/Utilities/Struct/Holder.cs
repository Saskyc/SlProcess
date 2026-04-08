using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Exiled.API.Features;
using VeryEpicEventPlugin.Utilities.MEC;
using VeryEpicEventPlugin.Utilities.MEC.Loop;
using VeryEpicEventPlugin.Utilities.MEC.Number;
using VeryEpicEventPlugin.Utilities.Process;

namespace VeryEpicEventPlugin.Utilities.Struct;

/// <summary>
/// Holder of object inheriting from TimingUtil{T} that will be executed and stopped when Func<bool> return true.
/// </summary>
/// <typeparam name="T">Object that inherits from TimingUtil{T}</typeparam>
public record struct Holder<T> : IHolder where T : TimingUtil<T>
{
#nullable enable

    /// <summary>
    /// The main process
    /// </summary>
    public SlProcess? Process { get; set; }

    /// <summary>
    /// The item being held.
    /// </summary>
    public T HeldItem { get; set; }

    /// <summary>
    /// Function defining when it'll stop.
    /// </summary>
    public Func<bool> StopWhen { get; set; }

    /// <summary>
    /// The actions executed after end.
    /// </summary>
    public List<Action> EndActions { get; set; } = [];

    /// <summary>
    /// Rate of checking the StopWhen Function.
    /// </summary>
    public float Rate { get; set; }

    /// <summary>
    /// The loop responsible for checking the ending. Will use Rate and StopWhen in constructor.
    /// </summary>
    private Loop Ender { get; set; }

    /// <summary>
    /// If the holder should start.
    /// </summary>
    public bool ShouldStart { get; set; } = true;

    /// <summary>
    /// Fills conditions
    /// </summary>
    /// <param name="conditions">Conditions that define should start</param>
    /// <returns>Holder{T} object</returns>
    public Holder<T> If(params bool[] conditions)
    {
        if (!ShouldStart) return this;
        foreach (var i in conditions)
        {
            if (!i)
            {
                ShouldStart = false;
                return this;
            }
        }

        return this;
    }

    /// <summary>
    /// ImmediateActions executed right as the Holder is started
    /// </summary>
    public List<Action> ImmediateActions { get; set; } = [];

    /// <summary>
    /// Fills the ImmediateActions
    /// </summary>
    /// <param name="actionsAfterStart"></param>
    /// <returns></returns>
    public Holder<T> Immediately(params Action[] actionsAfterStart)
    {
        ImmediateActions.AddRange(actionsAfterStart);
        return this;
    }

    /// <summary>
    /// Fills the end actions
    /// </summary>
    /// <param name="endActions">Actions executed after end</param>
    /// <returns>Holder{T} object</returns>
    public Holder<T> Fill(params Action[] endActions)
    {
        EndActions.AddRange(endActions);
        return this;
    }

    /// <summary>
    /// Change rate of the End loop checking.
    /// </summary>
    /// <param name="number">Number object</param>
    /// <returns>Holder{T} object</returns>
    public Holder<T> ChangeRate(Number number)
    {
        Rate = number;
        return this;
    }

    /// <summary>
    /// Method that wraps StopWhen method.
    /// </summary>
    /// <returns></returns>
    public float EndWhen()
    {
        Log.Info("BOOL CHECK");
        bool status = false;
        try
        {
            status = StopWhen();
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        if (status)
        {
            Log.Info("Stopping");
            Stop(true);
        }

        return Rate;
    }

    /// <summary>
    /// The start of Holder based on SlProcess.
    /// </summary>
    /// <param name="process">SlProcess object</param>
    public void BaseStart(SlProcess? process = null)
    {
        if (!ShouldStart) return;

        foreach (var i in ImmediateActions)
        {
            if (i == null)
            {
                continue;
            }

            try
            {
                i();
            }
            catch (Exception e)
            {
                Log.Error($"Immediate Actions error: {e}");
            }
        }

        HeldItem.Start(out _);
        Ender.CreateHandle();

        Process = process;
        if (process != null && !process.Holders.Contains(this))
            process.Holders.Add(this);
    }

    /// <summary>
    /// Starts with SlProcess
    /// </summary>
    /// <param name="process">SlProcess object</param>
    /// <returns>Holder{T} object</returns>
    public Holder<T> Start(SlProcess? process = null)
    {
        BaseStart(process);
        return this;
    }

    /// <summary>
    /// The method responsible for stopping the whole process.
    /// </summary>
    /// <param name="remove">If it should remove from SlProcess object.</param>
    public void BaseStop(bool remove)
    {
        if (remove && Process != null && Process.Holders.Contains(this))
            Process.Holders.Remove(this);
        
        HeldItem.Start(out _);
        
        Ender.Kill();

        foreach (var i in EndActions)
        {
            try
            {
                i();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }

    /// <summary>
    /// Stop method.
    /// </summary>
    /// <param name="remove">Should remove itself from SlProcess object</param>
    /// <returns>Holder{T} object</returns>
    public Holder<T> Stop(bool remove)
    {
        BaseStop(remove);
        return this;
    }

    /// <summary>
    /// Constructor of Holder{T} object
    /// </summary>
    /// <param name="heldItem">The item held that inherits from TimingUtil{T}</param>
    /// <param name="stopWhen">Function defining when holder should stop</param>
    /// <param name="rate">Rate at what will the function defining stop will be checked.</param>
    public Holder(T heldItem, Func<bool> stopWhen, Number rate)
    {
        HeldItem = heldItem;
        StopWhen = stopWhen;
        Rate = rate;

        Ender = (Loop)EndWhen;
    }
}