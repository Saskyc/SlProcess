using AdminToys;
using Exiled.API.Features.Toys;
using TextToy = LabApi.Features.Wrappers.TextToy;

namespace VeryEpicEventPlugin.Utilities.Primitives;

public class VeAdminToyInteractable : VeAdminToyBase<VeAdminToyInteractable, Text>
{
    public VeAdminToyInteractable(params ObjectProperty[] properties)
    {
        SetProperties(properties);
    }

    public VeAdminToyInteractable()
    {
        
    }

    public override void Processing(ObjectProperty property, ref bool shouldProcess)
    {
        switch (property.Setting)
        {
            
        }
    }
    
    public override Text CreateObject(bool ifFoundRemoveOldOne = false)
    {
        return null;
    }
    
}