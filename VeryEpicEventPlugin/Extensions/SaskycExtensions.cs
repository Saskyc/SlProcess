using AdminToys;
using LabApi.Features.Wrappers;
using UnityEngine;
using LightSourceToy = LabApi.Features.Wrappers.LightSourceToy;
using Pickup = Exiled.API.Features.Pickups.Pickup;

namespace VeryEpicEventPlugin.Extensions;

public static class SaskycExtensions
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

    extension(AdminToy toy)
    {
        public void Test()
        {
            
        }
    }

    
    extension(InteractableToy toy)
    {
        #nullable enable
        public Collider? Collision {
            get
            {
                return toy.Base._collider;
            }
            set
            {
                if (value == null)
                {
                    Object.Destroy(toy.Base._collider);
                }
                
                toy.Base._collider = value;
            } 
        }
        
        #nullable disable
    }
}