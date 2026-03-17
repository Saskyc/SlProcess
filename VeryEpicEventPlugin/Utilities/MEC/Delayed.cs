using System;
using Exiled.API.Features;
using MEC;
using VeryEpicEventPlugin.Enums;

namespace VeryEpicEventPlugin.Utilities.MEC;

/// <summary>
/// Wrapper for <see cref="Timing.CallDelayed"/> that'll execute code after X.
/// </summary>
public partial class Delayed : TimingUtil<Delayed>
{
    /// <summary>
    /// Action executed after <see cref="Delayed.Delay"/>
    /// </summary>
    public Action Action { get; set; }

    /// <summary>
    /// The delay that will be used for executing <see cref="Delayed.Action"/>
    /// </summary>
    public Number.Number Delay { get; set; }

    /// <summary>
    /// Cloak in <see cref="TimingUtil{T}.Times"/> to float.
    /// </summary>
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

    /// <summary>
    /// Override method that will add <see cref="Timing.CallDelayed"/> into <see cref="TimingUtil{T}.Handle"/>.
    /// </summary>
    /// <returns></returns>
    public override MethodResult<Delayed> CreateHandle()
    {
        Handle = Timing.CallDelayed(TimeUsed, () =>
        {
            try
            {
                Action.Invoke();
                return this;
            }
            catch (Exception e)
            {
                if (LogExceptions)
                {
                    Log.Error(e.Message);
                }
              
                return new MethodResult<Delayed>(this, e);
            }
        });
        
        return this;
    }

    /// <summary>
    /// Constructor for <see cref="Delayed"/> object.
    /// </summary>
    /// <param name="action">Action executed after delay</param>
    /// <param name="delay">The delay after action will be executed</param>
    /// <param name="cloak">The time type.</param>
    public Delayed(Action action, Number.Number delay, Cloak cloak = Cloak.Second)
    {
        Action = action;
        Delay = delay;
        Times = cloak;
    }
}