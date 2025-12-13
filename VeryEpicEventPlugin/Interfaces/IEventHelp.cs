using Exiled.API.Features;

namespace VeryEpicEventPlugin.Interfaces;

public interface IEventHelp
{
    /// <summary>
    /// Message to explain some commands behind your event.
    /// </summary>
    /// <param name="player">Player needing help</param>
    /// <returns>Message for the player.</returns>
    public string HelpMessage(Player player);
}