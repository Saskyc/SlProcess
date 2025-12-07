using System.Collections.Generic;
using System.Linq;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using PlayerRoles;
using UnityEngine;
using VeryEpicEventPlugin.Interfaces;

namespace VeryEpicEventPlugin.Events;

public class Mtfci : SlEvent
{
    public override string Name { get; set; } = "MTF Vs CI";
    public override int Id { get; set; } = 1;

    public static List<Exiled.API.Features.Player> Spectators => Exiled.API.Features.Player.List.Where(x => x.Role == RoleTypeId.Spectator).ToList();
    
    public static int FoundationForces => Exiled.API.Features.Player.List.Where(x => x.IsFoundationForces).ToList().Count;
    
    public static int Ci => Exiled.API.Features.Player.List.Where(x => x.IsCHI).ToList().Count;
    
    public override void Start()
    {
        Coroutines.Add(new Loop(Spawn).Run());
        base.Start();
    }

    public float Spawn()
    {
        foreach (var player in Spectators)
        {
            SpawnPlayer(player);
        }
        
        return 20;
    }

    public static void SpawnPlayer(Exiled.API.Features.Player player)
    {
        if (FoundationForces == Ci)
        {
            player.Role.Set(RoleTypeId.NtfSergeant);
        }

        if (FoundationForces > Ci)
        {
            player.Role.Set(RoleTypeId.ChaosMarauder);
        }
            
        if (FoundationForces < Ci)
        {
            player.Role.Set(RoleTypeId.NtfSergeant);
        }
    }
}