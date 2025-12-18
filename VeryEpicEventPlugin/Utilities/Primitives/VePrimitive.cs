using AdminToys;
using Exiled.API.Features;
using Exiled.API.Features.Toys;
using UnityEngine;

namespace VeryEpicEventPlugin.Utilities.Primitives;

public class VePrimitive : VeBase<VePrimitive, Primitive>
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

    public VePrimitive()
    {
        
    }
    
    public VePrimitive(params ObjectProperty[] properties)
    {
        SetProperties(properties);
    }
    
    public override void Create(bool ifFoundRemoveOldOne = false)
    {
        BeforeCreate();
        FieldObject = Primitive.Create(PrimitiveType, Flags, Position, VectorRotation, Scale, ShouldSpawn, Color);
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