namespace VeryEpicEventPlugin.Utilities.MEC.Number;

public partial class Number
{
    protected bool Equals(Number other)
    {
        return Base.Equals(other.Base);
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Number)obj);
    }

    public override int GetHashCode()
    {
        return Base.GetHashCode();
    }

    public override string ToString()
    {
        return Base.ToString();
    }
}