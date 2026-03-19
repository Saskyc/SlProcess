using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Interfaces;
using Exiled.Events.Features;

namespace VeryEpicEventPlugin.Utilities.Events;

/// <summary>
/// Provides a reflection-based utility for subscribing and unsubscribing to Exiled events dynamically.
/// Handles both generic and non-generic event handlers, including Action-based and IExiledEvent-based handlers.
/// </summary>
public class ExiledHandler
{
    /// <summary>
    /// The type of the Exiled event handler class this instance wraps.
    /// Ensures that only types within the Exiled.Events.Handlers namespace are allowed.
    /// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ExiledHandler"/> class for the specified event handler type.
    /// Retrieves all properties of the type for use when subscribing or unsubscribing to events.
    /// </summary>
    /// <param name="type">The type of the Exiled event handler class to wrap.</param>
    public ExiledHandler(Type type)
    {
        //Exiled.Events.Handlers.Server
        Type = type;
        Properties = type.GetProperties().ToList();
    }

    /// <summary>
    /// A list of all properties in the wrapped event handler type.
    /// Used for reflection when finding event subscription methods.
    /// </summary>
    public List<PropertyInfo> Properties { get; set; }

    /// <summary>
    /// Gets a <see cref="MethodInfo"/> for a method with the specified name and signature.
    /// Can optionally handle generic methods and ignore parameter types.
    /// </summary>
    /// <param name="name">The name of the method to retrieve.</param>
    /// <param name="isGeneric">Whether the method is expected to be generic.</param>
    /// <param name="ignoreTypes">Whether to ignore parameter types when searching.</param>
    /// <param name="withTypes">The expected parameter types of the method.</param>
    /// <returns>The <see cref="MethodInfo"/> if found; otherwise, null.</returns>
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

    /// <summary>
    /// Determines whether a method matches the specified parameter types.
    /// </summary>
    /// <param name="methodInfo">The method to check.</param>
    /// <param name="withTypes">The expected parameter types.</param>
    /// <returns>True if the method matches the types; otherwise, false.</returns>
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

    /// <summary>
    /// Subscribes an instance-based event handler to the corresponding Exiled event.
    /// </summary>
    /// <param name="instance">The instance containing the event handler method.</param>
    /// <returns>True if subscription succeeded; otherwise, false.</returns>
    public bool Subscribe(object instance)
    {
        return Between(instance, "Subscribe");
    }

    /// <summary>
    /// Unsubscribes an instance-based event handler from the corresponding Exiled event.
    /// </summary>
    /// <param name="instance">The instance containing the event handler method.</param>
    /// <returns>True if unsubscription succeeded; otherwise, false.</returns>
    public bool Unsubscribe(object instance)
    {
        return Between(instance, "Unsubscribe");
    }

    /// <summary>
    /// Handles the common logic for subscribing or unsubscribing instance-based or generic handlers.
    /// </summary>
    /// <param name="instance">The instance or action to operate on.</param>
    /// <param name="name">The method name, either "Subscribe" or "Unsubscribe".</param>
    /// <returns>True if the operation succeeded; otherwise, false.</returns>
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

    /// <summary>
    /// Subscribes a simple <see cref="Action"/> delegate to the corresponding Exiled event.
    /// </summary>
    /// <param name="action">The action to subscribe.</param>
    /// <returns>True if subscription succeeded; otherwise, false.</returns>
    public bool Subscribe(Action action)
    {
        return Between(action, "Subscribe");
    }

    /// <summary>
    /// Unsubscribes a simple <see cref="Action"/> delegate from the corresponding Exiled event.
    /// </summary>
    /// <param name="action">The action to unsubscribe.</param>
    /// <returns>True if unsubscription succeeded; otherwise, false.</returns>
    public bool Unsubscribe(Action action)
    {
        return Between(action, "Subscribe");
    }

    /// <summary>
    /// Handles subscription or unsubscription of simple <see cref="Action"/> delegates.
    /// </summary>
    /// <param name="action">The action to operate on.</param>
    /// <param name="name">The method name, either "Subscribe" or "Unsubscribe".</param>
    /// <returns>True if the operation succeeded; otherwise, false.</returns>
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

    /// <summary>
    /// Subscribes a generic <see cref="Action{T}"/> delegate to an event implementing <see cref="IExiledEvent"/>.
    /// </summary>
    /// <typeparam name="T">The type of the Exiled event.</typeparam>
    /// <param name="action">The action to subscribe.</param>
    /// <returns>True if subscription succeeded; otherwise, false.</returns>
    public bool Subscribe<T>(Action<T> action) where T : IExiledEvent
    {
        return Between(action, "Subscribe");
    }

    /// <summary>
    /// Unsubscribes a generic <see cref="Action{T}"/> delegate from an event implementing <see cref="IExiledEvent"/>.
    /// </summary>
    /// <typeparam name="T">The type of the Exiled event.</typeparam>
    /// <param name="action">The action to unsubscribe.</param>
    /// <returns>True if unsubscription succeeded; otherwise, false.</returns>
    public bool Unsubscribe<T>(Action<T> action) where T : IExiledEvent
    {
        return Between(action, "Unsubscribe");
    }

    /// <summary>
    /// Handles subscription or unsubscription of generic <see cref="Action{T}"/> delegates.
    /// </summary>
    /// <typeparam name="T">The type of the Exiled event.</typeparam>
    /// <param name="action">The action to operate on.</param>
    /// <param name="name">The method name, either "Subscribe" or "Unsubscribe".</param>
    /// <returns>True if the operation succeeded; otherwise, false.</returns>
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