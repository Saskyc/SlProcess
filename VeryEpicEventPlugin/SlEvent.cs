using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using LabApi.Features.Console;
using MEC;
using VeryEpicEventPlugin.Interfaces;
using VeryEpicEventPlugin.Utilities;

namespace VeryEpicEventPlugin;

public abstract partial class SlEvent
{
    public static Dictionary<int, SlEvent> Instances = [];
        
    public abstract string Name { get; set; }
    public abstract int Id { get; set; }
    
    public bool IsEnabled { get; set; } = false;

    public virtual List<IEventRegistry> EventRegistry { get; set; } = [];
    public virtual List<Loop> Coroutines { get; set; } = [];
    
    public virtual List<Delayed> Delays { get; set; } = [];
    
    public virtual List<CoroutineHandle> Handles { get; set; } = [];
    
    public virtual void Start()
    {
        
    }

    public virtual void End()
    {
        
    }
    
    public static void RegisterAll(Assembly assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsSubclassOf(typeof(SlEvent)))
            {
                continue;
            }

            SlEvent instance = (SlEvent)Activator.CreateInstance(type);
            Get(instance.Id)?.EndEvent();
            Instances[instance.Id] = instance;
        }
    }

    public static void UnregisterAll()
    {
        foreach (var i in Instances)
        {
            i.Value.EndEvent();
        }
        
        Instances.Clear();
    }

    #nullable enable
    public static SlEvent? Get(int id)
    {
        return Instances.GetValueOrDefault(id);
    }
    #nullable disable
    
    public static bool Start(int id)
    {
        var slEvent = Get(id);

        if (slEvent == null)
        {
            return false;
        }
        
        slEvent.StartEvent();
        return true;
    }

    public static bool End(int id)
    {
        var slEvent = Get(id);

        if (slEvent == null)
        {
            return false;
        }
        
        slEvent.EndEvent();
        return true;
    }
    
    public void StartEvent()
    {
        if (IsEnabled)
        {
            EndEvent();
        }
        
        IsEnabled = true;
        Start();
    }

    public void EndEvent()
    {
        IsEnabled = false;
        
        foreach (var i in EventRegistry)
        {
            i.Unsub();
        }
            
        foreach (var i in Coroutines)
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
        
        Handles.Clear();
        
        End();
    }
}