using System;
using System.Collections.Generic;
using MEC;
using VeryEpicEventPlugin.Utilities.Events;
using VeryEpicEventPlugin.Utilities.MEC;
using VeryEpicEventPlugin.Utilities.MEC.Loop;
using VeryEpicEventPlugin.Utilities.Struct;

namespace VeryEpicEventPlugin.Utilities.Process;

public partial class SlProcess
{
    /// <summary>
    /// Registry used to subscribe and unsubscribe events dynamically.
    /// </summary>
    public GenericEventRegistry EventRegistry = new GenericEventRegistry();

    /// <summary>
    /// Indicates whether the process is currently enabled and running.
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// A list of end conditions that automatically stop the process when triggered.
    /// </summary>
    public List<ProcessEndCondition> EndCondition { get; set; } = [];

    /// <summary>
    /// A list of loop tasks that run repeatedly while the process is active.
    /// </summary>
    public List<Loop> Loops { get; set; } = [];

    /// <summary>
    /// A list of delayed actions to execute during the process.
    /// </summary>
    public List<Delayed> Delays { get; set; } = [];

    /// <summary>
    /// A list of coroutine handles to manage asynchronous tasks within the process.
    /// </summary>
    public List<CoroutineHandle> Handles { get; set; } = [];

    /// <summary>
    /// A list of holders that manage the lifecycle of specific process tasks.
    /// </summary>
    public List<IHolder> Holders { get; set; } = [];

    /// <summary>
    /// A list of actions to execute when the process ends.
    /// </summary>
    public List<Action> ActionsOnEnd { get; set; } = [];

    /// <summary>
    /// A list of events that the process subscribes to dynamically while running.
    /// </summary>
    public List<object> Events { get; set; } = [];
}