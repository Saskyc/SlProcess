using System.Collections.Generic;
using System.Reflection;

namespace VeryEpicEventPlugin.Utilities;

public class ObjectSaver
{
#nullable enable

    public static BindingFlags Binding = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
    
    public Dictionary<string, object?> Properties { get; set; } = [];
    public Dictionary<string, object?> Fields { get; set; } = [];
    
    public ObjectSaver(object? obj)
    {
        if (obj == null)
        {
            return;
        }

        var type = obj.GetType();

        foreach (var property in type.GetProperties(Binding))
        {
            if (property.SetMethod == null)
            {
                continue;
            }
            Properties[property.Name] = property.GetValue(obj);
        }

        foreach (var field in type.GetFields(Binding))
        {
            Fields[field.Name] = field.GetValue(obj);
        }
    }

    public void Paste(object? obj)
    {
        if (obj == null)
        {
            return;
        }

        var type = obj.GetType();

        foreach (var property in Properties)
        {
            var prop = type.GetProperty(property.Key);

            if (prop == null)
            {
                continue;
            }
            
            prop.SetValue(obj, property.Value);
        }

        foreach (var field in Fields)
        {
            var fiel = type.GetField(field.Key);
            if (fiel == null)
            {
                continue;
            }
            
            fiel.SetValue(obj, field.Value);
        }
    }

    public T? Get<T>(string key)
    {
        var value = Get(key);
        if (value == null)
        {
            return default(T);
        }

        return (T)value;
    }

    public object? Get(string key)
    {
        if (Properties.TryGetValue(key, out var value) && value != null || 
            Fields.TryGetValue(key, out value) && value != null)
        {
            return value;
        }

        return null;
    }
}