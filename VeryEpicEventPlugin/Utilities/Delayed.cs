using System;
using Exiled.API.Features;
using MEC;
using VeryEpicEventPlugin.Enums;
using VeryEpicEventPlugin.Utilities;

namespace VeryEpicEventPlugin;

public partial class Delayed : TimingUtil<Delayed>
{
    public Action Action { get; set; }
    public float Delay { get; set; }
    
    public float TimeUsed => Times switch
    {
        Cloak.Milisecond => Delay * 1000f,
        Cloak.Second     => Delay,
        Cloak.Minute     => Delay / 60f,
        Cloak.Hour       => Delay / 3600f,
        Cloak.Day        => Delay / 86400f,
        Cloak.Week       => Delay / 604800f,
        Cloak.Month      => Delay / 2592000f,
        Cloak.Year       => Delay / 31536000f,
        _                => 0f
    };
    
    public override Delayed Run()
    {
        Handle.Add(Timing.CallDelayed(TimeUsed, () =>
        {
            try
            {
                Action.Invoke();
            }
            catch (Exception e)
            {
               Log.Error(e.Message);
            }
        }));
        
        return this;
    }

    public Delayed(Action action, float delay, Cloak cloak = Cloak.Second)
    {
        Action = action;
        Delay = delay;
        Times = cloak;
    }    
    
    public Delayed(Action action, int delay, Cloak cloak = Cloak.Second)
    {
        Action = action;
        Delay = delay;
        Times = cloak;
    }
    
    public Delayed(Action action, double delay, Cloak cloak = Cloak.Second)
    {
        Action = action;
        Delay = (float)delay;
        Times = cloak;
    }
    
    public Delayed(Action action, short delay, Cloak cloak = Cloak.Second)
    {
        Action = action;
        Delay = delay;
        Times = cloak;
    }
    
    public Delayed(Action action, long delay, Cloak cloak = Cloak.Second)
    {
        Action = action;
        Delay = delay;
        Times = cloak;
    }
}