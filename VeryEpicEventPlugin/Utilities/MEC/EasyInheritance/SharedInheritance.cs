using System;
using System.Reflection;
using Exiled.API.Features;

namespace VeryEpicEventPlugin.Utilities.MEC.EasyInheritance;

public class SharedInheritance<TOther> where TOther : class
{
    public static WatchableList<InheritanceHolder<TOther>> Registered { get; } = new(OnAdd, OnRemove, () => {});

    public virtual bool IsDebug => false;
    public virtual bool ShouldRegister => true;

    public static SharedInheritance<TOther>? Singleton
    {
        get
        {
            field ??= new SharedInheritance<TOther>();
            return field;
        }
    }

    public SharedInheritance()
    {
        
    }
    
    private static void OnAdd(InheritanceHolder<TOther> item)
    {
        item.Assembly ??= typeof(TOther).Assembly;
        Singleton?.OnRegistered(item, item.Base);
    }

    private static void OnRemove(InheritanceHolder<TOther> item)
    {
        Singleton?.OnUnregistered(item, item.Base);
    }

    
    protected virtual bool ShouldAddInstance(InheritanceHolder<TOther> item)
    {
        return true;
    }
        
    protected virtual void OnRegistered(InheritanceHolder<TOther> item, TOther instance) { }
    protected virtual void OnUnregistered(InheritanceHolder<TOther> item, TOther instance) { }
    
    public static void RegisterAll(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract) continue;
            if (typeof(InheritanceHolder<TOther>) == type || typeof(TOther) == type)
            {
                Log.Info($"Skipping {type.Name}, because it's the original instance.");
                continue;
            }
            if (!type.IsSubclassOf(typeof(InheritanceHolder<TOther>)) && 
                !type.IsSubclassOf(typeof(TOther))) 
                continue;
            
            if (type.GetConstructor([]) == null)
            {
                Log.Error($"Type {type.Name} could not be registered, please add default constructor");
                continue;
            }
            TOther instance;
            try
            {
                instance = (TOther)Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
                Log.Error($"Instance registration error. Type: {type}\nMessage: {e}");
                continue;
            }

            var holder = new InheritanceHolder<TOther>(instance, assembly);

            if (Singleton?.ShouldAddInstance(holder) ?? false)
            {
                if (Singleton?.IsDebug ?? false)
                {
                    Log.Info($"Adding {type.Name} as {typeof(TOther).Name}");
                }
                Registered.Add(holder);
            }
        }

        if (Singleton?.IsDebug ?? false)
        {
            Log.Info($"Registering of {typeof(TOther).Name} is done with instances: {Registered.Count}");
        }
    }

    public virtual InheritanceHolder<TOther>? CloneMyself()
    {
        if (this.GetType().GetConstructor([]) == null)
        {
            Log.Error($"{this.GetType().Name} does not have default constructor");
            return null;
        }
        return (InheritanceHolder<TOther>)Activator.CreateInstance(this.GetType());
    }

    public static void Register(InheritanceHolder<TOther> instance)
    {
        Registered.Add(instance);
    }
    
    public static void Unregister(InheritanceHolder<TOther> instance)
    {
        Registered.Remove(instance);
    }
    
    public static void UnregisterAll(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        foreach (var i in Registered)
        {
            Singleton?.OnUnregistered(i, i.Base);
        }
        Registered.RemoveAll(r => r.Assembly == assembly);
    }
}