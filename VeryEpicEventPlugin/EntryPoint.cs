using CustomPlayerEffects;
using Exiled.API.Features;
using HarmonyLib;
using VeryEpicEventPlugin.Utilities.MEC.EasyInheritance;

namespace VeryEpicEventPlugin;

public class EntryPoint : Plugin<Config>
{
    public override string Author { get; } = "Saskyc";
    public override string Name { get; } = "SlProcess";
    public float Number { get; set; } = 0;
    public static EntryPoint? Instance { get; set; }
    public Harmony? Harmony { get; set; }
    
    public override void OnEnabled()
    {
        Instance = this;
        Harmony = new Harmony("VeryEpicEventPlugin.smth");
        
        Test.RegisterAll(typeof(StatusEffectBase).Assembly);
        
        Harmony?.PatchAll();
        
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Harmony?.UnpatchAll();
        Instance = null;
        base.OnDisabled();
    }
}