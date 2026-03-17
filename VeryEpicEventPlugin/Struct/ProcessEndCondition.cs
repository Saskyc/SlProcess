using System;
using Exiled.API.Features;
using VeryEpicEventPlugin.Utilities.MEC.Loop;
using VeryEpicEventPlugin.Utilities.Process;

namespace VeryEpicEventPlugin.Struct;

public partial record struct ProcessEndCondition
{
    /// <summary>
    /// The rate at StopWhen will be checked.
    /// </summary>
    public float CheckRate { get; init; }

    /// <summary>
    /// SlProcess object that is held in this struct.
    /// </summary>
    public SlProcess Process { get; set; }

    /// <summary>
    /// Function defining condition when the SlProcess will stop. If true will stop.
    /// </summary>
    public Func<bool> StopWhen { get; set; }

    /// <summary>
    /// The loop object that takes from StopWhen and CheckRate.
    /// </summary>
    internal Loop MyLoop { get; set; }
    
    /// <summary>
    /// Anchors the ProcessEndCondition to SlProcess object
    /// </summary>
    /// <param name="process">SlProcess object</param>
    /// <returns>ProcessEndCondition object</returns>
    public ProcessEndCondition Anchor(SlProcess process)
    {
        Process = process;
        process.EndCondition.Add(this);
        MyLoop = new Loop(TheLoop);
        process.Loops.Add(MyLoop.Run());

        return this;
    }

    /// <summary>
    /// Stops the ProcessEndCondition, not the SlProcess.
    /// </summary>
    /// <param name="removeItself">True if it should remove itself from SlProcess.</param>
    /// <returns></returns>
    public ProcessEndCondition Stop(bool removeItself = false)
    {
        MyLoop.Stop();

        if (removeItself)
        {
            Process.EndCondition.Remove(this);
        }
        
        return this;
    }
    
    /// <summary>
    /// The main loop function responsible for everything.
    /// </summary>
    /// <returns></returns>
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
    
    /// <summary>
    /// Constructor for object ProcessEndCondition
    /// </summary>
    /// <param name="stopWhen">Function defining condition when SlProcess will be stopped. True if stop.</param>
    /// <param name="checkRate">The rate at the function will be checked. Can't be changed later.</param>
    public ProcessEndCondition(Func<bool> stopWhen, float checkRate)
    {
        StopWhen = stopWhen;
        CheckRate = checkRate;
    }
}