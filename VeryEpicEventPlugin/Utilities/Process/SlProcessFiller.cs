using System;
using System.Collections.Generic;
using MEC;
using VeryEpicEventPlugin.Utilities.MEC;
using VeryEpicEventPlugin.Utilities.MEC.Loop;
using VeryEpicEventPlugin.Utilities.Struct;

namespace VeryEpicEventPlugin.Utilities.Process;

public partial class SlProcess
{
    /// <summary>
    /// Adds multiple objects to a specified list and returns the process for chaining.
    /// </summary>
    /// <typeparam name="T">The type of the list items.</typeparam>
    /// <param name="list">The list to fill.</param>
    /// <param name="objects">Objects to add to the list.</param>
    /// <returns>The current <see cref="SlProcess"/> instance.</returns>
    public SlProcess Fill<T>(List<T> list, params T[] objects)
    {
        list.AddRange(objects);
        return this;
    }

    /// <summary>
    /// Adds one or more <see cref="ProcessEndCondition"/> to the process.
    /// </summary>
    /// <param name="processEndConditions">The end conditions to add.</param>
    /// <returns>The current <see cref="SlProcess"/> instance.</returns>
    public SlProcess Fill(params ProcessEndCondition[] processEndConditions) => Fill(EndCondition, processEndConditions);

    /// <summary>
    /// Adds one or more <see cref="Loop"/> objects to the process.
    /// </summary>
    /// <param name="loops">The loops to add.</param>
    /// <returns>The current <see cref="SlProcess"/> instance.</returns>
    public SlProcess Fill(params Loop[] loops) => Fill(Loops, loops);

    /// <summary>
    /// Adds one or more <see cref="Delayed"/> objects to the process.
    /// </summary>
    /// <param name="delays">The delayed actions to add.</param>
    /// <returns>The current <see cref="SlProcess"/> instance.</returns>
    public SlProcess Fill(params Delayed[] delays) => Fill(Delays, delays);

    /// <summary>
    /// Adds one or more <see cref="CoroutineHandle"/> objects to the process.
    /// </summary>
    /// <param name="handles">The coroutine handles to add.</param>
    /// <returns>The current <see cref="SlProcess"/> instance.</returns>
    public SlProcess Fill(params CoroutineHandle[] handles) => Fill(Handles, handles);

    /// <summary>
    /// Adds one or more <see cref="Action"/> delegates to be executed when the process ends.
    /// </summary>
    /// <param name="actions">The actions to execute on end.</param>
    /// <returns>The current <see cref="SlProcess"/> instance.</returns>
    public SlProcess Fill(params Action[] actions) => Fill(ActionsOnEnd, actions);

    /// <summary>
    /// Adds one or more <see cref="IHolder"/> objects to the process.
    /// </summary>
    /// <param name="holders">The holders to add.</param>
    /// <returns>The current <see cref="SlProcess"/> instance.</returns>
    public SlProcess Fill(params IHolder[] holders) => Fill(Holders, holders);

    /// <summary>
    /// Adds one or more event objects to the process for dynamic subscription.
    /// </summary>
    /// <param name="events">The events to subscribe to.</param>
    /// <returns>The current <see cref="SlProcess"/> instance.</returns>
    public SlProcess FillEvent(params object[] events) => Fill(Events, events);
}