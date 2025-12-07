using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;

namespace VeryEpicEventPlugin.Commands;

public class EventCommand : ICommand, IUsageProvider
{
    public string Command { get; } = "Event";
    public string[] Aliases { get; } = ["ev"];
    public string Description { get; } = "Command for events";
    public string[] Usage { get; } = ["play <event>", "stop <event>", "list"];
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (arguments.Count < 1)
        {
            response = "Play/Stop/List";
            return false;
        }
        
        bool result; int id;
        
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

                result = SlEvent.Start(id);

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
                    response += $"\n- {i.Id} | {i.Name}";
                }

                return true;
            default:
                response = "Play/Stop/List";
                return false;
        }
    }
}