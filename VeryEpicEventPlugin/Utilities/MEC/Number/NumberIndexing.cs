using System.Globalization;
using System.Linq;
using TMPro;

namespace VeryEpicEventPlugin.Utilities.MEC.Number;

public partial class Number
{
    public float this[Number index]
    {
        get => float.Parse(Base.ToString(CultureInfo.CurrentCulture).ToCharArray()[index.Int].ToString());
        set
        {
            var str = Base.ToString();
            var arr = str.ToArray();
            arr[index.Int] = char.Parse(value.ToString(CultureInfo.CurrentCulture));
            str = arr.ArrayToString();
            Base = float.Parse(str);
        }
    }
}