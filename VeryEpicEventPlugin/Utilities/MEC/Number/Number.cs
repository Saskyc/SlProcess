using System.Diagnostics.Contracts;

namespace VeryEpicEventPlugin.Utilities.MEC.Number;

public partial class Number
{
    [Pure]
    public float Base { get; set; } = 0;
    
    private Number()
    {
        
    }
}