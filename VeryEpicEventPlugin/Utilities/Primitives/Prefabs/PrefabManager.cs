using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Exiled.API.Features;
using Interactables.Interobjects;
using LabApi.Features.Wrappers;
using Mirror;
using UnityEngine;
using Light = Exiled.API.Features.Toys.Light;

namespace VeryEpicEventPlugin.Utilities.Primitives.Prefabs;

public class PrefabManager
{
    public static bool WasFilled = false;
    
    public static Dictionary<string, GameObject> Prefabs = [];
    public static Dictionary<string, Component> Components = [];
    
    public static void Fill()
    {
        foreach (var i in NetworkClient.prefabs.Values)
        {
            Prefabs[i.name] = i;
            foreach (var j in i.GetComponents(typeof(Component)))
            {
                Components[j.name] = j;
            }
        }

        WasFilled = true;
    }

    public static void SafeFill()
    {
        foreach (var i in NetworkClient.prefabs.Values)
        {
            if (Prefabs.ContainsKey(i.name))
                continue;
            Prefabs[i.name] = i;
            
            foreach (var j in i.GetComponents(typeof(Component)))
            {
                if (Components.ContainsKey(j.name))
                    continue;
                Components[j.name] = j;
            }
        }
    }

    public static GameObject GetGameObject(string name)
    {
        if (!WasFilled)
        {
            Fill();
        }
        
        SafeFill();
        SafeFill();
        SafeFill();
        
        return Prefabs[name];
    }
    
    public static Component GetComponent(string name)
    {
        if (!WasFilled)
        {
            Fill();
            WasFilled = true;
        }
        
        return Components[name];
    }
    
    public static void PrintAll()
    {
        HashSet<string> hashSet = [];
        
        foreach (var i in NetworkClient.prefabs.Values)
        {
            hashSet.Add(i.name);
        }

        foreach (var i in hashSet)
        {
            Log.Info($"Found prefab: {i}");
        }
    }
}