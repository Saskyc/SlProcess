using System;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using LabApi.Features.Wrappers;
using Mirror;
using UnityEngine;

namespace VeryEpicEventPlugin.Toying;

public static class GameObjectManager
{
    #nullable enable
    public static class Getter
    {
        /*================================================================
                                       Base
        ================================================================*/
        
        public static AdminToyBase? GetToyBase(GameObject gameObject) => 
            gameObject.GetComponent<AdminToyBase>();

        /*================================================================
                                     LabApi
        ================================================================*/
    
        public static AdminToy? GetLabToy(GameObject gameObject) => 
            AdminToy.Get(GetToyBase(gameObject));
    
        public static AdminToy GetLabToy(AdminToyBase toyBase) => 
            AdminToy.Get(toyBase);
    
        /*================================================================
                                     Exiled
        ================================================================*/
    
        public static Exiled.API.Features.Toys.AdminToy GetExiledToy(GameObject gameObject) => 
            Exiled.API.Features.Toys.AdminToy.Get(GetToyBase(gameObject));
    
        public static Exiled.API.Features.Toys.AdminToy GetExiledToy(AdminToyBase toyBase) => 
            Exiled.API.Features.Toys.AdminToy.Get(toyBase);
    }

    public static class Creator
    {
        /*================================================================
                                       Base
        ================================================================*/
    
        public static T Create<T>( )
            where T : AdminToyBase
        {
            if (AdminToy.PrefabCache<T>.Prefab! == null!)
            {
                T? component = null;
                foreach (var gameObject in NetworkClient.prefabs.Values)
                {
                    if (gameObject.TryGetComponent(out component))
                        break;
                }
                AdminToy.PrefabCache<T>.Prefab = !(component == null) ? component : throw new InvalidOperationException($"No prefab in NetworkClient.prefabs has component type {typeof (T)}");
            }
            var obj = UnityEngine.Object.Instantiate(AdminToy.PrefabCache<T>.Prefab);
            return obj;
        }
        
        /*================================================================
                                     Exiled
        ================================================================*/
        
        public static GameObject? Create(PrefabType prefabType)
        {
            if (!PrefabHelper.TryGetPrefab(prefabType, out var gameObject))
                return null;
            return UnityEngine.Object.Instantiate(gameObject);
        }
    }

    public static class Convertor
    {
        public static bool TryConvert<T>(AdminToy labToy, out T? instance) where T : AdminToy
            => (instance = labToy as T) != null;

        public static bool TryConvert<T>(Exiled.API.Features.Toys.AdminToy labToy, out T? instance)
            where T : Exiled.API.Features.Toys.AdminToy
            => (instance = labToy as T) != null;
    }
}