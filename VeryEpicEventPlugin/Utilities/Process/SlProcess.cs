using System;
using MEC;
using VeryEpicEventPlugin.Struct;

namespace VeryEpicEventPlugin.Utilities.Process;

public partial class SlProcess
{
    public virtual void CustomStartCode(bool run)
    {
        
    }

    public virtual void CustomStopCode()
    {
        
    }
    
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
            i.Run();
        }
        
        foreach (var i in Delays)
        {
            i.Run();
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

    public void End()
    {
        IsEnabled = false;
            
        foreach (var i in Loops)
        {
            i.Stop();
        }

        foreach (var i in Delays)
        {
            i.Stop();
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
    
    public SlProcess()
    {
        
    }
    
    public SlProcess(Func<bool> condition, float checkRateInSeconds, bool start = false)
    {
        EndCondition.Add(new ProcessEndCondition(condition, checkRateInSeconds).Anchor(this));
        if (start)
        {
            Start();
        }
    }
}