using System;
using AdminToys;
using Exiled.API.Features;
using Exiled.API.Features.Toys;
using Mirror;
using UnityEngine;

namespace VeryEpicEventPlugin.Utilities.Primitives;

public abstract partial class VeAdminToyBase<TWrapper, TObj> where TObj : AdminToy where TWrapper : VeAdminToyBase<TWrapper, TObj>, new()
{
    public virtual TObj FieldObject { get; set; } = null;
    public virtual TObj RealObject 
    {
        get
        {
            if (FieldObject == null)
            {
                Create();
            }
            return FieldObject;
        }
    }

    public void BeforeCreateCore(bool ifFoundRemoveOldOne = false)
    {
        if (ifFoundRemoveOldOne)
        {
            Destroy();
        }  
    }

    public TWrapper Create(bool ifFoundRemoveOldOne = false)
    {
        FieldObject = CreateObject(ifFoundRemoveOldOne);
        return (TWrapper)this;
    }

    public abstract TObj CreateObject(bool ifFoundRemoveOldOne = false);
    
    public TWrapper CreateFor(Player player)
    {
        var state = ShouldSpawn;
        ShouldSpawn = false;
        player.Connection.Send(GetSpawnMessage(CreateObject()));

        ShouldSpawn = state;
        return (TWrapper)this;
    }
    
    public TWrapper DestroyFor(Player player)
    {
        var state = ShouldSpawn;
        ShouldSpawn = false;
        player.Connection.Send(GetDestroyMessage(CreateObject()));

        ShouldSpawn = state;
        return (TWrapper)this;
    }

    public TWrapper SpawnFor(Player player)
    {
        var state = ShouldSpawn;
        ShouldSpawn = false;
        player.Connection.Send(SpawnMessage);

        ShouldSpawn = state;
        return (TWrapper)this;
    }
    
    public uint NetworkId => FieldObject.AdminToyBase.netId;

    public SpawnMessage SpawnMessage
    {
        get
        {
            var mess = new SpawnMessage();
            mess.position = Position;
            mess.rotation = Rotation;
            mess.netId = FieldObject.AdminToyBase.netId;
            return mess;
        }
    }
    
    public SpawnMessage GetSpawnMessage(TObj obj)
    {
        var mess = new SpawnMessage();
        mess.position = obj.Position;
        mess.rotation = obj.Rotation;
        mess.netId = obj.AdminToyBase.netId;
        return mess;
    }
    
    public ObjectHideMessage HideMessage
    {
        get
        {
            var mess = new ObjectHideMessage(); 
            mess.netId = FieldObject.AdminToyBase.netId;
            return mess;
        }
    }
    
    public ObjectHideMessage GetHideMessage(TObj obj)
    {
        var mess = new ObjectHideMessage();
        mess.netId = obj.AdminToyBase.netId;
        return mess;
    }
    
    public ObjectDestroyMessage DestroyMessage
    {
        get
        {
            var mess = new ObjectDestroyMessage(); 
            mess.netId = FieldObject.AdminToyBase.netId;
            return mess;
        }
    }
    
    public ObjectDestroyMessage GetDestroyMessage(TObj obj)
    {
        var mess = new ObjectDestroyMessage();
        mess.netId = obj.AdminToyBase.netId;
        return mess;
    }

    public virtual VeAdminToyBase<TWrapper, TObj> Destroy(bool safe = true)
    {
        if (!safe)
        {
            FieldObject?.Destroy();
            return this;
        }
        
        try
        {
            FieldObject?.Destroy();
        }
        catch (Exception e)
        {
            //ignored
        }

        return this;
    }

    

    public Vector3 Position
    {
        get
        {
            if (Exists)
            {
                return field = FieldObject.Position;
            }

            return field;
        }
        set
        {
            field = value;
            if (Exists)
            {
                FieldObject.Position = value;
            }
        }
    } = new(0f, 0f, 0f);

    public Quaternion Rotation
    {
        get
        {
            if (!Exists)
            {
                return field;
            }

            return field = FieldObject.Rotation;
        }
        set
        {
            field = value;
            if (Exists)
            {
                FieldObject.Rotation = value;
            }
        }
    } = new(0f, 0f, 0f, 0f);

    public Vector3 VectorRotation
    {
        get => Rotation.eulerAngles;
        set => Rotation = Quaternion.Euler(value);
    }

    public Vector3 Scale
    {
        get
        {
            if(Exists)
            {
                return field = FieldObject.Scale;
            }
            
            return field;
        }
        set
        {
            field = value;

            if (Exists)
            {
                FieldObject.Scale = value;
            }
        }
    } = new(0f, 0f, 0f);

    public bool ShouldSpawn { get; set; } = true;
    
    public VeAdminToyBase(params ObjectProperty[] properties)
    {
        SetProperties(properties);
    }

    public bool Exists => FieldObject != null;
    
    public TWrapper Follow(Transform target, bool worldPositionStays = true, Vector3? positionOffset = null, Quaternion? rotationOffset = null)
    {
        RealObject.Transform.SetParent(target, worldPositionStays);
        RealObject.Transform.localPosition = positionOffset ?? Vector3.zero;
        RealObject.Transform.localRotation = rotationOffset ?? Quaternion.identity;
        return (TWrapper)this;
    }
    
    public virtual void SetProperties(params ObjectProperty[] properties)
    {
        bool willCreate = false;
        
        for (int i = 0; i < properties.Length; i++)
        {
            bool should = true;
            Processing(properties[i], ref should);
            if (!should)
            {
                continue;
            }
            
            switch (properties[i].Setting)
            {
                case PropertySetting.Position:
                    Position = properties[i].GetValue<Vector3>();
                    break;
                case PropertySetting.Rotation:
                    if (properties[i].IsValue<Vector3>())
                    {
                        Rotation = Quaternion.Euler(properties[i].GetValue<Vector3>());
                    }
                    
                    if (properties[i].IsValue<Quaternion>())
                    {
                        Rotation = properties[i].GetValue<Quaternion>();
                    }
                    break;
                case PropertySetting.Scale:
                    Scale = properties[i].GetValue<Vector3>();
                    break;
                case PropertySetting.AutoCreate:
                    willCreate = true;
                    break;
                case PropertySetting.ShouldSpawn:
                    ShouldSpawn = properties[i].GetValue<bool>();
                    break;
            }
        }

        BeforeCreate(properties);
        
        if (willCreate)
        {
            Create();
        }
    }

    public virtual void Processing(ObjectProperty property, ref bool shouldProcess)
    {
        
    }
    
    public virtual void BeforeCreate(params ObjectProperty[] properties)
    {
        
    }

    public static implicit operator VeAdminToyBase<TWrapper, TObj>(TObj primitive)
    {
        var vePrim = new TWrapper();
        vePrim.FieldObject = primitive;
        return vePrim;
    }

    public static implicit operator TObj(VeAdminToyBase<TWrapper, TObj> veAdminToy)
    {
        return veAdminToy.RealObject;
    }
    
    public static implicit operator LabApi.Features.Wrappers.AdminToy(VeAdminToyBase<TWrapper, TObj> toy)
    {
        return LabApi.Features.Wrappers.AdminToy.Get(toy.RealObject.AdminToyBase);
    }
    
    public static implicit operator AdminToy(VeAdminToyBase<TWrapper, TObj> toy)
    {
        return toy.RealObject;
    }
    
    public static implicit operator AdminToyBase(VeAdminToyBase<TWrapper, TObj> toy)
    {
        return toy.RealObject.AdminToyBase;
    }
    
    public static implicit operator VeAdminToyBase<TWrapper, TObj>(AdminToyBase toy)
    {
        var vBase = new TWrapper();
        vBase.FieldObject = (TObj)AdminToy.Get(toy);
        return vBase;
    }
    
    public static implicit operator VeAdminToyBase<TWrapper, TObj>(AdminToy toy)
    {
        return toy.AdminToyBase;
    }
    
    public static implicit operator VeAdminToyBase<TWrapper, TObj>(LabApi.Features.Wrappers.AdminToy toy)
    {
        return toy.Base;
    }
}