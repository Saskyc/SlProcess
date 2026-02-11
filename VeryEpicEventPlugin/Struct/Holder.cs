using System;
using System.Collections.Generic;
using Exiled.API.Features;
using VeryEpicEventPlugin.Utilities.MEC;
using VeryEpicEventPlugin.Utilities.MEC.Loop;
using VeryEpicEventPlugin.Utilities.MEC.Number;
using VeryEpicEventPlugin.Utilities.Process;

namespace VeryEpicEventPlugin.Struct;

public record struct Holder<T> : IHolder where T : TimingUtil<T>
{
    #nullable enable
    
    public SlProcess? Process { get; set; }
    public T HeldItem { get; set; }
    public Func<bool>  StopWhen { get; set; }

    public List<Action> EndActions { get; set; } = [];
    
    public float Rate { get; set; }
    private Loop Ender { get; set; }

    public bool ShouldStart { get; set; } = true;
    
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

    public List<Action> ImmediateActions { get; set;} = [];
    
    public Holder<T> Immediately(params Action[] actionsAfterStart)
    {
        ImmediateActions.AddRange(actionsAfterStart);
        return this;
    }
    
    public Holder<T> Fill(params Action[] endActions)
    {
        EndActions.AddRange(endActions);
        return this;
    }
    
    public Holder<T> ChangeRate(Number number)
    {
        Rate = number;
        return this;
    }

    public float EndWhen()
    {
        Log.Info("BOOL CHECK");
        var status = StopWhen();

        if (status)
        {
            Log.Info("Stopping");
            Stop(true);
        }
        
        return Rate;
    }

    
    public void BaseStart(SlProcess? process = null)
    {
        if(!ShouldStart) return;

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
        
        HeldItem.Run();
        Ender.Run();
        
        Process = process;
        if(process != null && !process.Holders.Contains(this))
            process.Holders.Add(this);
    }

    public Holder<T> Start(SlProcess? process = null)
    {
        BaseStart(process);
        return this;
    }
    
    public void BaseStop(bool remove)
    {
        if(remove && Process != null && Process.Holders.Contains(this))
            Process.Holders.Remove(this);
        
        HeldItem.Stop();
        Ender.Stop();
        
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
    
    public Holder<T> Stop(bool remove)
    {
        BaseStop(remove);
        return this;
    }
    
    public Holder(T heldItem, Func<bool> stopWhen, Number rate)
    {
        HeldItem = heldItem;
        StopWhen = stopWhen;
        Rate = rate;
        
        Ender = (Loop)EndWhen;
    }
}