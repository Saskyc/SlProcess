using CustomPlayerEffects;

namespace VeryEpicEventPlugin.Utilities.MEC.EasyInheritance;

public class Test : SharedInheritance<StatusEffectBase>
{
    public override bool IsDebug { get; } = true;
}