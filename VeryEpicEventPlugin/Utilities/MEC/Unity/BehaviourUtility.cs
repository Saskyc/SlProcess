using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VeryEpicEventPlugin.Utilities.MEC.Unity;

public partial class BehaviourUtility : IDisposable
{
    public GameObject GameObject { get; set; }
    private NumeroUno UnoNumero { get; set; }
    
    public BehaviourUtility()
    {
        GameObject = new GameObject();
        UnoNumero = GameObject.AddComponent<NumeroUno>();
    }

    public BehaviourUtility(GameObject gameObject)
    {
        GameObject = gameObject;
        UnoNumero = GameObject.AddComponent<NumeroUno>();
    }
    
    public BehaviourUtility Run()
    {
        return this;
    }

    public void Dispose()
    { 
        Object.Destroy(UnoNumero);
    }
}

public partial class BehaviourUtility
{
    public BehaviourUtility Fill<T>(List<T> list, params T[] objects)
    {
        list.AddRange(objects);
        return this;
    }

    public BehaviourUtility FillOnAwake(params Action[] actions) => Fill(OnAwake, actions);
    public BehaviourUtility FillOnUpdate(params Action[] actions) => Fill(OnUpdate, actions);

    public List<Action> OnAwake { get; set; } = [];
    public List<Action> OnUpdate { get; set; } = [];
}

internal class NumeroUno : MonoBehaviour
{
    private void Awake()
    {
        
    }
}