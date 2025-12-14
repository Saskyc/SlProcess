using LabApi.Features.Wrappers;
using UnityEngine;
using Pickup = Exiled.API.Features.Pickups.Pickup;

namespace VeryEpicEventPlugin.Extensions;

public static class PickupExtension
{
    extension(Pickup pickup)
    {
        public LightSourceToy Light(Color color, float range, float intensity, LightType type = LightType.Point, LightShadows shadowType = LightShadows.Hard)
        {
            var lightSourceToy = LightSourceToy.Create(pickup.Transform);
        
            lightSourceToy.Type = type;
            lightSourceToy.ShadowType = shadowType;
            lightSourceToy.Color = color;
            lightSourceToy.Range = range;
            lightSourceToy.Intensity = intensity;

            return lightSourceToy;
        }
    }
}