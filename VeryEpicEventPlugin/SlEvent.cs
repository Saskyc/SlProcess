using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LabApi.Features.Console;
using VeryEpicEventPlugin.Interfaces;

namespace VeryEpicEventPlugin;

public abstract class SlEvent
{
    public static List<SlEvent> Instances = [];
        
    public abstract string Name { get; set; }
    public abstract int Id { get; set; }
    
    public bool IsEnabled { get; set; } = false;

    public virtual List<IEventRegistry> EventRegistry { get; set; } = [];
    public virtual List<Loop> Coroutines { get; set; } = [];
    
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
            
            Instances.Add(instance);
        }
    }

    public static void UnregisterAll()
    {
        Instances.Clear();
    }

    public static bool Start(int id)
    {
        for (var index = 0; index < Instances.Count; index++)
        {
            if (Instances[index].Id != id)
            {
                continue;
            }

            Instances[index].StartEvent();
            return true;
        }

        return false;
    }

    internal void StartEvent()
    {
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
        for (int index = 0; index > Instances.Count; index++)
        {
            if (id != Instances[index].Id)
            {
                continue;
            }
            
            Instances[index].EndEvent();
            
            return true;
        }

        return false;
    }

    internal void EndEvent()
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
            
        End();
    }
}