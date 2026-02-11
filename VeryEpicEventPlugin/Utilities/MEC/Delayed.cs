using System;
using Exiled.API.Features;
using MEC;
using VeryEpicEventPlugin.Enums;

namespace VeryEpicEventPlugin.Utilities.MEC;

public partial class Delayed : TimingUtil<Delayed>
{
    public Action Action { get; set; }
    public Number.Number Delay { get; set; }
    
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

    public Delayed(Action action, Number.Number delay, Cloak cloak = Cloak.Second)
    {
        Action = action;
        Delay = delay;
        Times = cloak;
    }
}