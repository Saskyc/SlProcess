using System;
using VeryEpicEventPlugin.Enums;

namespace VeryEpicEventPlugin.Utilities.MEC.Loop;

public partial class Loop
{
    #nullable enable
    private Func<Number.Number>? NumberFunction { get; set; }
    #nullable disable
    
    public float TimeUsed(float delay) => Times switch
    {
        Cloak.Milisecond => delay * 1000f,
        Cloak.Second     => delay,
        Cloak.Minute     => delay / 60f,
        Cloak.Hour       => delay / 3600f,
        Cloak.Day        => delay / 86400f,
        Cloak.Week       => delay / 604800f,
        Cloak.Month      => delay / 2592000f,
        Cloak.Year       => delay / 31536000f,
        _                => 0f
    };

    public static Loop Create<T>(Func<T> function, Cloak cloak = Cloak.Second)
    {
        return new Loop(Number.Number.Convert(function), cloak);
    }
    
    public static Func<T> Create<T>(Loop loop)
    {
        return Number.Number.Convert<T>(loop.NumberFunction);
    }
}