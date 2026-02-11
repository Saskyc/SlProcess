using System;
using System.Collections.Generic;
using MEC;
using VeryEpicEventPlugin.Struct;
using VeryEpicEventPlugin.Utilities.Events;
using VeryEpicEventPlugin.Utilities.MEC;
using VeryEpicEventPlugin.Utilities.MEC.Loop;

namespace VeryEpicEventPlugin.Utilities.Process;

public partial class SlProcess
{
    public GenericEventRegistry EventRegistry = new GenericEventRegistry();
    
    public bool IsEnabled { get; set; } = false;

    public List<ProcessEndCondition> EndCondition { get; set; } = [];
    
    public List<Loop> Loops { get; set; } = [];
    
    public List<Delayed> Delays { get; set; } = [];
    
    public List<CoroutineHandle> Handles { get; set; } = [];

    public List<IHolder> Holders { get; set; } = [];
    
    public List<Action> ActionsOnEnd { get; set; } = [];

    public List<object> Events { get; set; } = [];
}