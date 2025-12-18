using AdminToys;
using UnityEngine;
using Light = Exiled.API.Features.Toys.Light;

namespace VeryEpicEventPlugin.Utilities.Primitives;

public partial class VeLight : VeBase<VeLight, Light>
{
    public float Range
    {
        get
        {
            if (Exists)
            {
                return field = FieldObject.Range;
            }

            return field;
        }
        set
        {
            field = value;

            if (Exists)
            {
                FieldObject.Range = value;
            }
        }
    } = 1f;

    public float Intensity
    {
        get
        {
            if (Exists)
            {
                return field = FieldObject.Intensity;
            }

            return field;
        }
        set
        {
            field = value;

            if (Exists)
            {
                FieldObject.Intensity = value;
            }
        }
    } = 1f;

    public LightType LightType
    {
        get
        {
            if (Exists)
            {
                return field = FieldObject.LightType;
            }

            return field;
        }
        set
        {
            field = value;

            if (Exists)
            {
                FieldObject.LightType = value;
            }
        }
    } = LightType.Spot;

    public LightShadows ShadowType
    {
        get
        {
            if (Exists)
            {
                return field = FieldObject.ShadowType;
            }

            return field;
        }
        set
        {
            field = value;

            if (Exists)
            {
                FieldObject.ShadowType = value;
            }
        }
    } = LightShadows.Soft;

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

    public VeLight()
    {
        
    }
    
    public VeLight(params ObjectProperty[] properties)
    {
        SetProperties(properties);
    }
    
    public override void Create(bool ifFoundRemoveOldOne = false)
    {
        BeforeCreateCore();
        FieldObject = Light.Create(Position, VectorRotation, Scale, ShouldSpawn);
        FieldObject.Color = Color;
        FieldObject.Range = Range;
        FieldObject.Intensity = Intensity;
        FieldObject.LightType = LightType;
    }

    public override void Processing(ObjectProperty property, ref bool shouldProcess)
    {
        switch (property.Setting)
        {
            case PropertySetting.Range:
                Range = property.GetValue<float>();
                break;
            case PropertySetting.Intensity:
                Intensity = property.GetValue<float>();
                break;
            case PropertySetting.LightType:
                LightType = property.GetValue<LightType>();
                break;
            case PropertySetting.ShadowType:
                ShadowType = property.GetValue<LightShadows>();
                break;
        }
    }
    
    public static implicit operator VeLight(Light light)
    {
        var veLight = new VeLight();
        veLight.FieldObject = light;
        return veLight;
    }

    public static implicit operator Light(VeLight ve)
    {
        return ve.RealObject;
    }
}