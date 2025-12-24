using System;
using Exiled.API.Features;
using Mirror;
using VeryEpicEventPlugin.Utilities.Primitives.Prefabs;
using Generator = LabApi.Features.Wrappers.Generator;

namespace VeryEpicEventPlugin
{
    public partial class EntryPoint : Plugin<Config>
    {
        public override string Name => "SlProcess";
        public override Version Version => new Version(1, 1, 0);

        public override string Author => "Saskyc";

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RestartingRound += OnRoundRestart;
            SlEvent.RegisterAll();
            PrefabManager.WasFilled = false;
        }
        
        public void OnRoundRestart()
        {
            PrefabManager.PrintAll();
            PrefabManager.WasFilled = false;
            
            foreach (var i in SlEvent.Instances)
            {
                i.Value.EndEvent();
            }
        }
        
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RestartingRound -= OnRoundRestart;
            SlEvent.UnregisterAll();
        }
    }
}