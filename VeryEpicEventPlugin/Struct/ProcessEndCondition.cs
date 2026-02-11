using System;
using Exiled.API.Features;
using VeryEpicEventPlugin.Utilities.MEC.Loop;
using VeryEpicEventPlugin.Utilities.Process;

namespace VeryEpicEventPlugin.Struct;

public partial record struct ProcessEndCondition
{
    public float CheckRate { get; init; }
    public SlProcess Process { get; set; }
    public Func<bool> StopWhen { get; set; }

    public Loop MyLoop { get; set; }
    
    public ProcessEndCondition Anchor(SlProcess process)
    {
        Process = process;
        process.EndCondition.Add(this);
        MyLoop = new Loop(TheLoop);
        process.Loops.Add(MyLoop.Run());

        return this;
    }

    public ProcessEndCondition Stop(bool removeItself = false)
    {
        MyLoop.Stop();

        if (removeItself)
        {
            Process.EndCondition.Remove(this);
        }
        
        return this;
    }
    
    public float TheLoop()
    {
        try
        {
            var status = StopWhen.Invoke();
            if (status)
            {
                Process.End();
                Process.EndCondition.Remove(this);
            }
        }
        catch(Exception e)
        {
            Log.Error($"Your end condition in process is fucked: \n{e.Message}");
        }


        return CheckRate;
    }
    
    public ProcessEndCondition(Func<bool> stopWhen, float checkRate)
    {
        StopWhen = stopWhen;
        CheckRate = checkRate;
    }
}