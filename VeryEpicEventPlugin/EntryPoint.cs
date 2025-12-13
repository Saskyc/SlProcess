using System;
using Exiled.API.Features;

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
            
            Log.Info("Starting 2 seconds delay and 2 seconds loop");
        }
        
        public void OnRoundRestart()
        {
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