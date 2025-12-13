using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using Exiled.API.Features;

namespace VeryEpicEventPlugin.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class TestCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        var player = Player.Get(sender);
        
        response = $"Your room is: {player.CurrentRoom.Name} & relevant position is: {player.Position - player.CurrentRoom.Position}";
        return true;
    }

    public string Command { get; } = "testcommand";
    public string[] Aliases { get; } = [];
    public string Description { get; } = "";
}