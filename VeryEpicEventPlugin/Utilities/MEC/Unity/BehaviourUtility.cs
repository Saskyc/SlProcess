using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VeryEpicEventPlugin.Utilities.MEC.Unity;

/// <summary>
/// 
/// </summary>
public partial class BehaviourUtility : IDisposable
{
    /// <summary>
    /// 
    /// </summary>
    public GameObject GameObject { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public BehaviourUtility()
    {
        GameObject = new GameObject();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    public BehaviourUtility(GameObject gameObject)
    {
        GameObject = gameObject;
        UnoNumero = GameObject.AddComponent<NumeroUno>();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public BehaviourUtility Run()
    {
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    { 
        Object.Destroy(UnoNumero);
    }
}

/// <summary>
/// 
/// </summary>
public partial class BehaviourUtility
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="objects"></param>
    /// <returns></returns>
    public BehaviourUtility Fill<T>(List<T> list, params T[] objects)
    {
        list.AddRange(objects);
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actions"></param>
    /// <returns></returns>
    public BehaviourUtility FillOnAwake(params Action[] actions) => Fill(OnAwake, actions);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="actions"></param>
    /// <returns></returns>
    public BehaviourUtility FillOnUpdate(params Action[] actions) => Fill(OnUpdate, actions);

    /// <summary>
    /// 
    /// </summary>
    public List<Action> OnAwake { get; set; } = [];

    /// <summary>
    /// 
    /// </summary>
    public List<Action> OnUpdate { get; set; } = [];
}