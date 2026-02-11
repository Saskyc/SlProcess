using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Interfaces;
using Exiled.Events.Features;

namespace VeryEpicEventPlugin.Utilities.Events;

public class ExiledHandler
{
    public Type Type
    {
        get => field;
        set
        {
            if (!value.Namespace.Contains("Exiled.Events.Handlers"))
            {
                Log.Error("Event you created with ExiledHandler is wrong!");
            }

            field = value;
        }
    }
    
    public ExiledHandler(Type type)
    {
        //Exiled.Events.Handlers.Server
        Type = type;
        Properties = type.GetProperties().ToList();
    }

    public List<PropertyInfo> Properties { get; set; }

    public MethodInfo GetMethod(string name, bool isGeneric = false, bool ignoreTypes = false, params Type[] withTypes)
    {
        var type = GetType();
        foreach (var methodInfo in type.GetMethods())
        {
            if (methodInfo.Name != name)
            {
                continue;
            }
            
            if (isGeneric != methodInfo.IsGenericMethod)
            {
                continue;
            }

            if (ignoreTypes)
            {
                return methodInfo;
            }

            if (!HasMethodTypes(methodInfo, withTypes))
            {
                continue;
            }

            return methodInfo;
        }

        return null;
    }

    public bool HasMethodTypes(MethodInfo methodInfo, params Type[] withTypes)
    {
        var parameters = methodInfo.GetParameters();

        if (parameters.Length != withTypes.Length)
            return false;

        for (var i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].ParameterType != withTypes[i])
                return false;
        }

        return true;
    }
    
    public bool Subscribe(object instance)
    {
        return Between(instance, "Subscribe");
    }
    
    public bool Unsubscribe(object instance)
    {
        return Between(instance, "Unsubscribe");
    }
    
    public bool Between(object instance, string name)
    {
        var instanceType = instance.GetType();
        
        if (instanceType.GenericTypeArguments.Length == 0)
        {
            var method = GetMethod("Subscribe", false, false, [typeof(Action)]);

            if (method == null)
            {
                Log.Error("METHOD IS NULL");
                return false;
            }
            
            try
            {
                return (bool)method.Invoke(this, [instance]);
            }
            catch (Exception e)
            {
                Log.Error($"Empty Action error reflection object: {e}");
            }

            return false;
        }
        
        var generic = instanceType.GenericTypeArguments[0];

        var subscribeMethod = GetMethod(name, true, true, []);
        
        var genericMethod = subscribeMethod.MakeGenericMethod(generic);
        try
        {
            genericMethod.Invoke(this, [instance]);
            return true;
        }
        catch (Exception e)
        {
            Log.Error($"ERROR EXECUTING METHOD WITHOUT GENERIC: {e}");
        }

        return false;
    }

    public bool Subscribe(Action action)
    {
        return Between(action, "Subscribe");
    }
    
    public bool Unsubscribe(Action action)
    {
        return Between(action, "Subscribe");
    }
    
    public bool Between(Action action, string name)
    {
        var handler = new CustomEventHandler(action);

        foreach (var property in Properties)
        {
            if (property.Name != action.Method.Name)
            {
                continue;
            }
            
            var value = property.GetValue(null);
            var propertyType = value.GetType();
            
            if (propertyType.GenericTypeArguments.Length != 0)
            {
                continue;
            }

            var method = propertyType.GetMethod(name, [typeof(CustomEventHandler)]);
            try
            {
                method.Invoke(value, [handler]);
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"I GOT ERROR INVOKING EMPTY METHOD: {e}");
            }
        }

        return false;
    }
    
    public bool Subscribe<T>(Action<T> action) where T : IExiledEvent
    {
        return Between(action, "Subscribe");
    }
    
    public bool Unsubscribe<T>(Action<T> action) where T : IExiledEvent
    {
        return Between(action, "Unsubscribe");
    }

    public bool Between<T>(Action<T> action, string name) where T : IExiledEvent
    {
        var handler = new CustomEventHandler<T>(action);
        
        foreach (var property in Properties)
        {
            var value = property.GetValue(null);
            var propertyType = value.GetType();
            
            var args = propertyType.GenericTypeArguments;
            if (args.Length == 0)
            {
                continue;
            }

            if (args[0].Name == typeof(T).Name)
            {
                var method = propertyType.GetMethod(name, [typeof(CustomEventHandler<T>)]);
                if (method == null)
                {
                    Log.Error("I HAD ERROR FINDING SUBSCRIBE METHOD");
                    return false;
                }

                try
                {
                    method.Invoke(value, [handler]);
                    return true;
                }
                catch (Exception e)
                {
                    Log.Error($"HAD ERROR INVOKING: {e}");
                }

                return false;
            }
        }

        return false;
    }
}