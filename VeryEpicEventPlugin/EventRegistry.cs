using System;
using Exiled.Events.EventArgs.Interfaces;
using Exiled.Events.Features;
using VeryEpicEventPlugin.Interfaces;

namespace VeryEpicEventPlugin;

public class EventRegistry<T> : IEventRegistry
    where T : IExiledEvent
{
    public CustomEventHandler<T> Handler;
    public Event<T> Event;

    public EventRegistry(CustomEventHandler<T> handler, Event<T> @event)
    {
        Handler = handler;
        Event = @event;
    }

    public void Sub()
    {
        Event += Handler;
        var y = this;
    }
    
    public void Unsub()
    {
        Event -= Handler;
    }

    public EventRegistry<T> Register()
    {
        Sub();
        return this;
    }

    public EventRegistry<T> UnRegister()
    {
        Unsub();
        return this;
    }
}
