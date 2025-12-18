namespace VeryEpicEventPlugin.Utilities.Primitives;

public partial class ObjectProperty
{
    public PropertySetting Setting { get; set; }
    
    #nullable enable
    public object? Value { get; set; }
    #nullable disable

    public bool IsValue<T>()
    {
        return Value is T;
    }
    
    public T GetValue<T>()
    {
        return Value is T value ? value : default(T);
    }
    
    public ObjectProperty(PropertySetting setting)
    {
        Setting = setting;
        Value = null;
    }

    public ObjectProperty(PropertySetting setting, object value)
    {
        Setting = setting;
        Value = value;
    }

    public static implicit operator ObjectProperty(PropertySetting setting)
    {
        return new ObjectProperty(setting);
    }
    
    public static implicit operator ObjectProperty((PropertySetting setting, object value) tuple)
    {
        return new ObjectProperty(tuple.setting, tuple.value);
    }
}