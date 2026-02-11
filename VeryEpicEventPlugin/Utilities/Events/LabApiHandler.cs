/*using System;
using System.Reflection;
using LabApi.Events;

namespace VeryEpicEventPlugin.Events;

public class LabApiHandler
{
    public Type Type { get; set; }

    public LabApiHandler(Type type)
    {
        Type = type;
    }

    public void Test<T>(Action<T> action)
    {
        if (action is not Delegate del)
        {
            return;
        }

        var y = new LabEventHandler(Laaaa);
    }

    public void Laaaa(LabEventHandler eventHandler)
    {
        
    }
    
    public object GetLabEventHandler(string fromProperty)
    {
        var property = Type.GetProperty(fromProperty);
        return property.GetValue(null);
    }

    public bool IsGeneric(PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType.IsGenericType;
    }
    
    public Type GetGeneric(PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType.GenericTypeArguments[0];
    }

    public Delegate GetDelegate(object obj)
    {
        if (obj is not Delegate del)
            throw new InvalidOperationException("Object is not a delegate.");
        
        return del;
    }
    
    public static Delegate Subscribe(Delegate source, Delegate handler)
    {
        if (source == null)
            return handler;

        return Delegate.Combine(source, handler);
    }

    public static Delegate Unsubscribe(Delegate source, Delegate handler)
    {
        if (source == null)
            return null;

        return Delegate.Remove(source, handler);
    }

}*/