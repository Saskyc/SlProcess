using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using Exiled.API.Features;
using VeryEpicEventPlugin.Interfaces;

namespace VeryEpicEventPlugin.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class EventCommand : ICommand, IUsageProvider
{
    public string Command { get; } = "event";
    public string[] Aliases { get; } = ["ev"];
    public string Description { get; } = "Command for events";
    public string[] Usage { get; } = ["play <event>", "stop <event>", "list"];
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        var player = Player.Get(sender);
        
        if (arguments.Count < 1)
        {
            response = "Play/Stop/List";
            return false;
        }
        
        bool result; int id;

        if (int.TryParse(arguments.At(0), out id))
        {
            var slEvent = SlEvent.Get(id);
            
            if (slEvent == null)
            {
                response = "I didn't find event you were looking for.";
                return false;
            }

            if (arguments.Count < 2)
            {
                if (slEvent is IEventHelp eventHelp)
                {
                    response = eventHelp.HelpMessage(player);
                    return true;
                }

                response = $"Event {slEvent.Name} of id {id}, does not implement help message";
                return false;
            }

            if (slEvent is not IEventCommand eventCommand)
            {
                response = "Sory this event does not have any additional commands";
                return false;
            }
            
            List<string> args = [];
            for (int i = 0; i < arguments.Count -1; i++)
            {
                if (i == 0)
                {
                    continue;
                }
                
                args.Add(arguments.At(i));
            }

            response = eventCommand.Execute(player, args);

            if (response.Length < 1)
            {
                response = "Command returned empty message unknown if state was success or not.";
                return false;
            }
            
            if (response[0].ToString() == "!")
            {
                response = response.Substring(1);
                return false;
            }

            return true;
        }
        
        
        switch (arguments.At(0).ToLower())
        {
            case "p":
            case "play":
                if (arguments.Count < 2)
                {
                    response = "Id of event you want to play";
                    return false;
                }

                if (!int.TryParse(arguments.At(1), out id))
                {
                    response = "Id is number";
                    return false;
                }

                result = SlEvent.Start(id);

                if (!result)
                {
                    response = $"Couldn't find event {id}";
                    return false;
                }
                
                response = $"Playing event {id}";
                return true;
            case "s":
            case "stop":
                if (arguments.Count < 2)
                {
                    response = "Id of event you want to stop";
                    return false;
                }

                if (!int.TryParse(arguments.At(1), out id))
                {
                    response = "Id is number";
                    return false;
                }

                result = SlEvent.End(id);

                if (!result)
                {
                    response = $"Couldn't find event {id}";
                    return false;
                }
                
                response = $"Stopped event {id}";
                return true;
            case "l":
            case "list":
                response = "Event list";
                foreach (var i in SlEvent.Instances)
                {
                    response += $"\n- {i.Key} | {i.Value.Name}";
                }

                return true;
            default:
                response = "Play/Stop/List";
                return false;
        }
    }
}