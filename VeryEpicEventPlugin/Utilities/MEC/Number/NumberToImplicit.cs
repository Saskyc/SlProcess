namespace VeryEpicEventPlugin.Utilities.MEC.Number;

public partial class Number
{
    public static implicit operator Number(int num)
    {
        var number = new Number();
        number.Base = num;
        return number;
    }
    
    public static implicit operator Number(float num)
    {
        var number = new Number();
        number.Base = num;
        return number;
    }
    
    public static implicit operator Number(double num)
    {
        var number = new Number();
        number.Base = (float)num;
        return number;
    }
    
    public static implicit operator Number(short num)
    {
        var number = new Number();
        number.Base = num;
        return number;
    }
    
    public static implicit operator Number(long num)
    {
        var number = new Number();
        number.Base = num;
        return number;
    }
    
    public static implicit operator Number(string num)
    {
        var number = new Number();
        number.Base = float.Parse(num);
        return number;
    }
}