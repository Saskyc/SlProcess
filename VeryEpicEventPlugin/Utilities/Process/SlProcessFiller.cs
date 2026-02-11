using System;
using System.Collections.Generic;
using MEC;
using VeryEpicEventPlugin.Struct;
using VeryEpicEventPlugin.Utilities.MEC;
using VeryEpicEventPlugin.Utilities.MEC.Loop;

namespace VeryEpicEventPlugin.Utilities.Process;

public partial class SlProcess
{
    public SlProcess Fill<T>(List<T> list, params T[] objects)
    {
        list.AddRange(objects);
        return this;
    }
    public SlProcess Fill(params ProcessEndCondition[] processEndConditions) => Fill(EndCondition, processEndConditions);
    public SlProcess Fill(params Loop[] loops) => Fill(Loops, loops);
    public SlProcess Fill(params Delayed[] delays) => Fill(Delays, delays);
    public SlProcess Fill(params CoroutineHandle[] handles) => Fill(Handles, handles);
    public SlProcess Fill(params Action[] actions) => Fill(ActionsOnEnd, actions);
    public SlProcess Fill(params IHolder[] holders) => Fill(Holders, holders);
    public SlProcess FillEvent(params object[] events) => Fill(Events, events);
}