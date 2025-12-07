using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.Features;
using VeryEpicEventPlugin.Interfaces;

namespace VeryEpicEventPlugin
{
    public class EntryPoint : Plugin<Config>
    {
        public override string Name => "VeryEpicEventPlugin";
        public override Version Version => new Version(1, 0, 0);

        public override string Author => "Saskyc";

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RestartingRound += OnRoundRestart;
            SlEvent.RegisterAll();
        }

        public void OnRoundRestart()
        {
            foreach (var i in SlEvent.Instances)
            {
                i.EndEvent();
            }
        }
        
        public override void OnDisabled()
        {
            SlEvent.UnregisterAll();
        }
    }
}