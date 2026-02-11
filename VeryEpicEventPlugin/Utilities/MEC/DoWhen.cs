using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;

namespace VeryEpicEventPlugin.Utilities.MEC;

public partial class DoWhen : TimingUtil<DoWhen>
{
    public bool ShouldExecute { get; set; } = false;
    public Func<bool> Condition { get; set; }

    public List<Action> Actions { get; set; } = [];

    public DoWhen(Func<bool> condition, params Action[] actions)
    {
        Condition = condition;
        Actions.AddRange(actions);
    }

    public DoWhen Fill(params Action[] actions)
    {
        Actions.AddRange(actions);
        return this;
    }
    
    public override DoWhen Run()
    {
        ShouldExecute = false;
        Handle.Add(Timing.RunCoroutine(Slave()));
        return this;
    }

    private IEnumerator<float> Slave()
    {
        for (;;)
        {
            if (ShouldExecute)
            {
                foreach (var action in Actions)
                {
                    try
                    {
                        action();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            
                yield break;
            }
            
            ShouldExecute = true;
            yield return Timing.WaitUntilTrue(Condition);
        }
    }

    public static implicit operator DoWhen(Func<bool> condition)
    {
        return new DoWhen(condition);
    }
}
