using Exiled.API.Features;
using Mirror;
using UnityEngine;

namespace VeryEpicEventPlugin.Utilities.Primitives.Prefabs;

public abstract class VePrefabBase<TWrapper> where TWrapper : VePrefabBase<TWrapper>, new()
{
    public GameObject Base { get; set; }
    
    public abstract string PrefabName { get; }

    public Vector3 Position
    {
        get
        {
            if (Base != null)
            {
                return field = Base.transform.position;
            }

            return field;
        }
        set
        {
            field = value;

            if (Base != null)
            {
                Base.transform.position = value;
            }
        }
    } = new Vector3(0, 0, 0);

    public Quaternion Rotation
    {
        get
        {
            if (Base != null)
            {
                return field = Base.transform.rotation;
            }

            return field;
        }
        set
        {
            field = value;

            if (Base != null)
            {
                Base.transform.rotation = value;
            }
        }
    } = new Quaternion(0, 0, 0, 1);

    public Vector3 Scale
    {
        get
        {
            if (Base != null)
            {
                return field = Base.transform.localScale;
            }

            return field;
        }
        set
        {
            field = value;

            if (Base != null)
            {
                Base.transform.localScale = value;
            }
        }
    } = new Vector3(1, 1, 1);

    public Transform Transform => Base.transform;

    public static TWrapper Create(Vector3 position, Quaternion rotation, Vector3 scale, bool network = true)
    {
        var twrapper = new TWrapper();
        twrapper.Position = position;
        twrapper.Rotation = rotation;
        twrapper.Scale = scale;
        twrapper.Base = twrapper.Instantiate();

        if (network)
        {
            twrapper.Spawn(twrapper.Base);
        }
        
        return twrapper;
    }
    
    public GameObject Instantiate()
    {
        var gObj = Object.Instantiate(PrefabManager.GetGameObject(PrefabName));
        gObj.transform.position = Position;
        gObj.transform.rotation = Rotation;
        gObj.transform.localScale = Scale;
        return gObj;
    }

    public TWrapper Spawn(GameObject obj = null)
    {
        if (obj == null)
        {
            NetworkServer.Spawn(Base);
            return (TWrapper)this;
        }
        
        NetworkServer.Spawn(obj);
        return (TWrapper)this;
    }
    
    public TWrapper UnSpawn(GameObject obj = null)
    {
        if (obj == null)
        {
            NetworkServer.UnSpawn(Base);
            return (TWrapper)this;
        }
        
        NetworkServer.UnSpawn(obj);
        return (TWrapper)this;
    }
}