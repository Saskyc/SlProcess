using VeryEpicEventPlugin.Utilities.Process;

namespace VeryEpicEventPlugin.Struct;

public interface IHolder
{
    public void BaseStart(SlProcess process);
    public void BaseStop(bool remove);
}