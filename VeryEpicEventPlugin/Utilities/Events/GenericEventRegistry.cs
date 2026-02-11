using System;
using System.Collections.Generic;
using System.Reflection;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using LabApi.Events.Arguments.PlayerEvents;

namespace VeryEpicEventPlugin.Utilities.Events;

public class GenericEventRegistry
{
    public Assembly ExiledAssembly { get; set; } = typeof(VerifiedEventArgs).Assembly;
    public Assembly LabApiAssembly { get; set; } = typeof(PlayerJoinedEventArgs).Assembly;
    
    public List<ExiledHandler> ExiledHandlers { get; set; } = [];
    
    public GenericEventRegistry()
    {
        Register(ExiledAssembly.GetTypes());
        Register(LabApiAssembly.GetTypes());
    }
    
    public void Register(params Type[] types)
    {
        foreach (var type in types)
        {
            Register(type);
        }
    }
    
    public void Register(Type type)
    {
        if (type.Namespace == null)
        {
            return;
        }
        
        if (type.Namespace.Contains("Exiled.Events.Handlers"))
        {
            Log.Info($"Exiled: {type.Name}");
            ExiledHandlers.Add(new ExiledHandler(type));
        }
        
        if (type.Namespace.Contains("LabApi.Events.Handlers"))
        {
            Log.Info($"LabApi: {type.Name}");
        }
    }

    /// <summary>
    /// Subscribes event
    /// </summary>
    /// <param name="obj">Either Action<T> or Action</param>
    /// <returns>sucsess</returns>
    public bool Subscribe(object obj)
    {
        foreach (var handler in ExiledHandlers)
        {
            var result = handler.Subscribe(obj);
            if (result)
            {
                return true;
            }
        }

        return false;
    }
    
    public bool Unsubscribe(object obj)
    {
        foreach (var handler in ExiledHandlers)
        {
            var result = handler.Unsubscribe(obj);
            if (result)
            {
                return true;
            }
        }

        return false;
    }
}