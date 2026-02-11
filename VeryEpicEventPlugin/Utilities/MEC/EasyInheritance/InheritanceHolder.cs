using System;
using System.Reflection;

namespace VeryEpicEventPlugin.Utilities.MEC.EasyInheritance;

public class InheritanceHolder<T> where T : class
{
    public T Base { get; set; }
    public Type GenericType => Base.GetType();
    public Assembly Assembly { get; internal set; }

    public InheritanceHolder(T tBase, Assembly assembly)
    {
        Base = tBase;
        Assembly = assembly;
    }
}