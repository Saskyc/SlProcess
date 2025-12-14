using System.Collections.Generic;
using Exiled.API.Features;

namespace VeryEpicEventPlugin.Interfaces;

public interface IEventCommand
{
    /// <summary>
    /// If you return !message it will return false as not executed, if you return without ! it will be true.
    /// </summary>
    /// <param name="player">The player executing command</param>
    /// <param name="args">List of arguments you'll get from the person executing the event</param>
    /// <returns>The message given to player.</returns>
    public string Execute(Player player, List<string> args);
}