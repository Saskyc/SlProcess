using System;
using System.Globalization;

namespace VeryEpicEventPlugin.Utilities.MEC.Number;

public partial class Number
{
    public static Number Parse(object obj)
    {
        return obj switch
        {
            Number n => n,
            int i => new Number { Base = i },
            float f => new Number { Base = f },
            double d => new Number { Base = (float)d },
            long l => new Number { Base = l },
            short s => new Number { Base = s },
            string s => new Number { Base = float.Parse(s, CultureInfo.InvariantCulture) },
            _ => throw new InvalidCastException($"Cannot parse {obj.GetType()} to Number")
        };
    }


    public TypeCode TypeCode => Base.GetTypeCode();
    public int HashCode => Base.GetHashCode();

    public static Func<Number> Convert<T>(Func<T> func)
    {
        Number Function()
        {
            return Number.Parse(func());
        }

        return Function;
    }
    
    public static Func<T> Convert<T>(Func<Number> func)
    {
        return () =>
        {
            var n = func();

            if (typeof(T) == typeof(int))
                return (T)(object)n.Int;

            if (typeof(T) == typeof(float))
                return (T)(object)n.Float;

            if (typeof(T) == typeof(double))
                return (T)(object)n.Double;

            if (typeof(T) == typeof(long))
                return (T)(object)n.Long;

            if (typeof(T) == typeof(short))
                return (T)(object)n.Short;

            if (typeof(T) == typeof(string))
                return (T)(object)n.String;

            if (typeof(T) == typeof(Number))
                return (T)(object)n;

            throw new InvalidCastException($"Cannot convert Number to {typeof(T)}");
        };
    }
    
    public string String
    {
        get => this.ToString();
        set => Base = float.Parse(value);
    }
    
    public int Int
    {
        get => (int)Base;
        set => Base = value;
    }
    
    public short Short
    {
        get => (short)Base;
        set => Base = value;
    }
    
    public long Long
    {
        get => (long)Base;
        set => Base = value;
    }
    
    public float Float
    {
        get => Base;
        set => Base = value;
    }
    
    public double Double
    {
        get => Base;
        set => Base = (float)value;
    }
}