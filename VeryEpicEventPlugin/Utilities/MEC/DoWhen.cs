using System;
using System.Collections.Generic;
using Exiled.API.Features;
using MEC;

namespace VeryEpicEventPlugin.Utilities.MEC;

/// <summary>
/// Does action when Condition becomes true.
/// </summary>
public partial class DoWhen : TimingUtil<DoWhen>
{
    /// <summary>
    /// If should execute, false means it won't execute. Good when wanting to setup variables.
    /// </summary>
    public bool ShouldExecute { get; set; } = false;

    /// <summary>
    /// Condition defining when will the <see cref="DoWhen.Actions"/> be executed.
    /// </summary>
    public Func<bool> Condition { get; set; }

    /// <summary>
    /// Actions that will be executed after <see cref="DoWhen.Condition"/> becomes true.
    /// </summary>
    public List<Action> Actions { get; set; } = [];

    /// <summary>
    /// Constructor for <see cref="DoWhen"/> object.
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="actions"></param>
    public DoWhen(Func<bool> condition, params Action[] actions)
    {
        Condition = condition;
        Actions.AddRange(actions);
    }

    /// <summary>
    /// Method filling <see cref="DoWhen.Actions"/>
    /// </summary>
    /// <param name="actions">Actions that will be added into <see cref="DoWhen.Actions"/></param>
    /// <returns><see cref="DoWhen"/></returns>
    public DoWhen Fill(params Action[] actions)
    {
        Actions.AddRange(actions);
        return this;
    }

    /// <summary>
    /// Override for Run. Adds slave coroutine into Handle.
    /// </summary>
    /// <returns><see cref="DoWhen"/></returns>
    public override DoWhen Run()
    {
        ShouldExecute = false;
        Handle.Add(Timing.RunCoroutine(Slave()));
        return this;
    }

    /// <summary>
    /// Main logic of coroutine.
    /// </summary>
    /// <returns>IEnumarator{float} object</returns>
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

    /// <summary>
    /// Implicit conversion between Func{bool} object & <see cref="DoWhen"/>
    /// </summary>
    /// <param name="condition">Func{bool} object</param>
    public static implicit operator DoWhen(Func<bool> condition)
    {
        return new DoWhen(condition);
    }
}
