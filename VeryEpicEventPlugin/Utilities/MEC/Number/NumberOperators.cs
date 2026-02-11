namespace VeryEpicEventPlugin.Utilities.MEC.Number;

public partial class Number
{
    public static Number operator +(Number left, Number right)
    {
        var num = new Number();
        num.Base = left.Base + right.Base;
        return num;
    }
    
    public static Number operator -(Number left, Number right)
    {
        var num = new Number();
        num.Base = left.Base - right.Base;
        return num;
    }
    
    public static Number operator /(Number left, Number right)
    {
        var num = new Number();
        num.Base = left.Base / right.Base;
        return num;
    }
    
    public static Number operator *(Number left, Number right)
    {
        var num = new Number();
        num.Base = left.Base * right.Base;
        return num;
    }
    
    public static Number operator ++(Number operand)
    {
        var num = new Number();
        num.Base = operand.Base + 1;
        return num;
    }
    
    public static Number operator --(Number operand)
    {
        var num = new Number();
        num.Base = operand.Base - 1;
        return num;
    }
    
    public static bool operator ==(Number left, Number right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }
        
        return left.Base == right.Base;
    }

    public static bool operator !=(Number left, Number right)
    {
        return !(left == right);
    }
}