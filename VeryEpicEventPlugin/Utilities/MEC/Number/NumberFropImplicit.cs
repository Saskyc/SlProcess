namespace VeryEpicEventPlugin.Utilities.MEC.Number;

public partial class Number
{
    public static implicit operator int(Number num)
    {
        return (int)num.Base;
    }
    
    public static implicit operator float(Number num)
    {
        return num.Base;
    }
    
    public static implicit operator double(Number num)
    {
        return num.Base;
    }
    
    public static implicit operator short(Number num)
    {
        return (short)num.Base;
    }
    
    public static implicit operator long(Number num)
    {
        return (long)num.Base;
    }
    
    public static implicit operator string(Number num)
    {
        return num.ToString();
    }
}