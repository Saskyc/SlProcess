using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using LabApi.Features.Console;
using VeryEpicEventPlugin.Interfaces;
using VeryEpicEventPlugin.Utilities;

namespace VeryEpicEventPlugin;

public abstract class SlEvent
{
    public static Dictionary<int, SlEvent> Instances = [];
        
    public abstract string Name { get; set; }
    public abstract int Id { get; set; }
    
    public bool IsEnabled { get; set; } = false;

    public virtual List<IEventRegistry> EventRegistry { get; set; } = [];
    public virtual List<Loop> Coroutines { get; set; } = [];
    
    public virtual List<Delayed> Delays { get; set; } = [];
    
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

            if (instance == null)
            {
                Logger.Error($"Instance could not be created for: {type.FullName}");
            }

            Instances[instance.Id] = instance;
        }
    }

    public static void UnregisterAll()
    {
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
        if (!Instances.TryGetValue(id, out var ev))
        {
            return false;
        }
        
        ev.StartEvent();
        return true;
    }

    public void StartEvent()
    {
        if (IsEnabled)
        {
            EndEvent();
        }
        
        IsEnabled = true;

        foreach (var i in EventRegistry)
        {
            i.Sub();
        }

        foreach (var i in Coroutines)
        {
            i.Run();
        }

        Start();
    }

    
    public static bool End(int id)
    {
        if (!Instances.TryGetValue(id, out var ev))
        {
            return false;
        }
        
        ev.EndEvent();
        return true;
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
        
        End();
    }
}