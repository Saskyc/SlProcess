namespace VeryEpicEventPlugin.Interfaces;

/// <summary>
/// Interface used for saving class EventRegistry<T> that has fuckass generic.
/// </summary>
public interface IEventRegistry
{
    /// <summary>
    /// The sub method to event.
    /// </summary>
    void Sub();
    
    /// <summary>
    /// The unsub method to event.
    /// </summary>
    void Unsub();
}
