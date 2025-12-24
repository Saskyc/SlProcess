using AdminToys;
using Exiled.API.Features;
using Exiled.API.Features.Toys;
using UnityEngine;

namespace VeryEpicEventPlugin.Utilities.Primitives;

public class VeAdminToyPrimitive : VeAdminToyBase<VeAdminToyPrimitive, Primitive>
{
    public PrimitiveType PrimitiveType
    {
        get
        {
            if (!Exists)
            {
                return field;
            }

            
            return field = FieldObject.Type;
        }
        set
        {
            if (!Exists)
            {
                field = value;
            }
            
            Log.Error("You may not change the PrimitiveType of object after creation.");
        }
    } = PrimitiveType.Cube;
    
    public Color Color
    {
        get
        {
            if (Exists)
            {
                return field = FieldObject.Color;
            }

            return field;
        }
        set
        {
            field = value;

            if (Exists)
            {
                FieldObject.Color = value;
            }
        }
    } = Color.white;
    
    public PrimitiveFlags Flags
    {
        get
        {
            if (Exists)
            {
                return field = FieldObject.Flags;
            }

            return field;
        }
        set
        {
            field = value;

            if (Exists)
            {
                FieldObject.Flags = value;
            }
        }
    }

    public VeAdminToyPrimitive()
    {
        
    }
    
    public VeAdminToyPrimitive(params ObjectProperty[] properties)
    {
        SetProperties(properties);
    }

    public override Primitive CreateObject(bool ifFoundRemoveOldOne = false)
    {
        BeforeCreateCore(ifFoundRemoveOldOne);
        var obj = Primitive.Create(PrimitiveType, Flags, Position, VectorRotation, Scale, ShouldSpawn, Color);
        return obj;
    }

    public override void Processing(ObjectProperty property, ref bool shouldProcess)
    {
        switch (property.Setting)
        {
            case PropertySetting.Type:
                PrimitiveType = property.GetValue<PrimitiveType>();
                break;
            case PropertySetting.Color:
                Color = property.GetValue<Color>();
                break;
            case PropertySetting.PrimitiveFlags:
                Flags = property.GetValue<PrimitiveFlags>();
                break;
        }
    }
}