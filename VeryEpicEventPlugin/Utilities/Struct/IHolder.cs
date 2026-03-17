using VeryEpicEventPlugin.Utilities.Process;

namespace VeryEpicEventPlugin.Utilities.Struct;

/// <summary>
/// The holder interface
/// </summary>
public interface IHolder
{
    /// <summary>
    /// The base for starting everything in holder.
    /// </summary>
    /// <param name="process">SlProcess object</param>
    public void BaseStart(SlProcess process);

    /// <summary>
    /// The base of stopping everything in holder
    /// </summary>
    /// <param name="remove">If it should remove itself from SlProcess object</param>
    public void BaseStop(bool remove);
}