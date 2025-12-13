using System;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Toys;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using LabApi.Features.Wrappers;
using PlayerRoles;
using UnityEngine;
using VeryEpicEventPlugin.Interfaces;
using VeryEpicEventPlugin.Utilities;
using AdminToy = LabApi.Features.Wrappers.AdminToy;
using Component = UnityEngine.Component;
using LightSourceToy = LabApi.Features.Wrappers.LightSourceToy;
using Map = Exiled.API.Features.Map;
using Pickup = Exiled.API.Features.Pickups.Pickup;
using Player = Exiled.API.Features.Player;
using Room = Exiled.API.Features.Room;

namespace VeryEpicEventPlugin.Events;

public class Bases : SlEvent, IEventCommand, IEventHelp
{
    public override string Name { get; set; } = "Bases";
    public override int Id { get; set; } = 2;
    
    public Dictionary<Player, Side> PlayerSide { get; set; } = [];
    public Dictionary<Player, Primitive> PlayerPrimitive { get; set; } = [];
    
    public (int MTF, int CI) Count { get; set; }
    public (Vector3 MTFSpawn, Vector3 CISpawn) Spawn { get; set; }
    
    public List<Room> Rooms { get; } = [];
    public List<ItemType> ItemPool { get; } = [];

    public List<AdminToy> Toys = [];
    public Dictionary<InteractableToy, Action<LabApi.Features.Wrappers.Player>> Unregistering { get; set; } = [];
    public List<Component> Components = [];
    
    private int Status { get; set; }
    
    public override void End()
    {
        foreach (var component in Components)
        {
            if (component == null)
            {
                continue;
            }
            UnityEngine.Object.Destroy(component);
        }
        
        Exiled.API.Features.Doors.Door.Get(DoorType.CheckpointLczA).Unlock();
        Exiled.API.Features.Doors.Door.Get(DoorType.CheckpointLczB).Unlock();
        
        foreach (var door in Door.List)
        {
            if (door is BreakableDoor breakableDoor)
            {
                breakableDoor.TryRepair();
            }
        }
        
        foreach (var room in Room.List)
        {
            room.ResetColor();
        }
        
        foreach (var adminToy in Toys)
        {
            if (adminToy is InteractableToy interactableToy)
            {
                if (Unregistering.TryGetValue(interactableToy, out var action))
                {
                    interactableToy.OnSearched -= action;
                }
            }
            
            adminToy?.Destroy();
        }
        
        foreach (var player in Player.List)
        {
            Remove(player);
            RemovePrimitive(player);
        }
        
        Components.Clear();
        Toys.Clear();
        PlayerSide.Clear();
        PlayerPrimitive.Clear();

        Count = (0, 0);
        Rooms.Clear();
        ItemPool.Clear();
        Status = 0;
        
        base.End();
    }

    public override void Start()
    {
        Map.CleanAllItems();
        Map.CleanAllRagdolls();
        
        PlayerSide = [];
        Count = (0, 0);
        Spawn = (Room.Get(RoomType.LczArmory).Position + new Vector3(0, 1, 0), Room.Get(RoomType.Lcz173).Position + new Vector3(7.44f, 13.01f, 7.75f));
        Status = 0;
        
        ItemPool.Clear();
        Rooms.Clear();

        ItemPool.AddRange([ItemType.GunCOM15, ItemType.GunCOM15, ItemType.GunCOM15, ItemType.GunCOM15, ItemType.GunCOM15, ItemType.Painkillers, ItemType.Painkillers, ItemType.Painkillers, ItemType.Ammo9x19, ItemType.Ammo556x45]);
        
        foreach (var door in Door.List)
        {
            if (door is BreakableDoor breakableDoor)
            {
                breakableDoor.TryRepair();
            }
        }
        
        Exiled.API.Features.Doors.Door.Get(DoorType.CheckpointLczA).Lock(10000000000, DoorLockType.AdminCommand);
        Exiled.API.Features.Doors.Door.Get(DoorType.CheckpointLczB).Lock(10000000000, DoorLockType.AdminCommand);
        
        foreach(var i in Room.List)
        {
            i.ResetColor();
            
            if (i.Zone != ZoneType.LightContainment)
            {
                continue;
            }

            if (i.Type == RoomType.Lcz173 || i.Type == RoomType.LczArmory)
            {
                continue;
            }
            
            Rooms.Add(i);
        }
        
        foreach (var player in Player.List.OrderBy(_ => Guid.NewGuid()).ToList())
        {
            SetRole(player);
            HasFlag(player);
        }

        AddItems();
        TeleportTo(SetupInteractable(new Vector3(9.05f, 12.43f, -1.86f)), new Vector3(2.11f, 15.28f, 10.87f));
        TeleportTo(SetupInteractable(new Vector3(2.11f, 15.28f, 10.87f)), new Vector3(9.05f, 12.43f, -1.86f));
        
        Coroutines.Add(new Loop(FastSpawn).Run());
        Coroutines.Add(new Loop(LongSpawn).Run());
        Coroutines.Add(new Loop(SpawnItems).Run());
        Coroutines.Add(new Loop(PrimCheckLoop).Run());
        Coroutines.Add(new Loop(Lights).Run());
        Coroutines.Add(new Loop(MakeSureAssigned).Run());
        Coroutines.Add(new Loop(EndCondition).Run());
        Coroutines.Add(new Loop(RemoveDeadEntries).Run());
        
        EventRegistry.Add(new EventRegistry<VerifiedEventArgs>(OnVerified, Exiled.Events.Handlers.Player.Verified).Register());
        EventRegistry.Add(new EventRegistry<LeftEventArgs>(OnLeft, Exiled.Events.Handlers.Player.Left).Register());
        
        EventRegistry.Add(new EventRegistry<ItemAddedEventArgs>(OnItemAddedEventArgs, Exiled.Events.Handlers.Player.ItemAdded).Register());
        EventRegistry.Add(new EventRegistry<ItemRemovedEventArgs>(OnItemRemovedEventArgs, Exiled.Events.Handlers.Player.ItemRemoved).Register());
        EventRegistry.Add(new EventRegistry<PickingUpItemEventArgs>(OnPickingUpItemEventArgs, Exiled.Events.Handlers.Player.PickingUpItem).Register());
        
        EventRegistry.Add(new EventRegistry<PickupAddedEventArgs>(OnPickupAddedEventArgs, Exiled.Events.Handlers.Map.PickupAdded).Register());
        EventRegistry.Add(new EventRegistry<RespawningTeamEventArgs>(OnRespawningTeamEventArgs, Exiled.Events.Handlers.Server.RespawningTeam).Register());
        
        EventRegistry.Add(new EventRegistry<InteractingDoorEventArgs>(OnInteractingDoorEventArgs, Exiled.Events.Handlers.Player.InteractingDoor).Register());

        for (int i = 0; i < 3; i++)
        {
            Pickup.CreateAndSpawn(ItemType.KeycardO5, Spawn.MTFSpawn + new Vector3(0f, 0.3f, 0f), null, null);
            Pickup.CreateAndSpawn(ItemType.KeycardO5, Spawn.CISpawn + new Vector3(0f, 0.3f, 0f), null, null);
        }
    }

    public InteractableToy SetupInteractable(Vector3 offset, RoomType inRoom = RoomType.Lcz173)
    {
        var interactableToy = InteractableToy.Create(Room.Get(inRoom).Position + offset, Quaternion.identity,
            null, true);

        interactableToy.InteractionDuration = 0.1f;
        interactableToy.Shape = InvisibleInteractableToy.ColliderShape.Box;
        interactableToy.Scale = new Vector3(1, 1, 1);
        Toys.Add(interactableToy);
        return interactableToy;
    }

    public InteractableToy TeleportTo(InteractableToy toy, Vector3 offset, RoomType inRoom = RoomType.Lcz173)
    {
        var act = (LabApi.Features.Wrappers.Player player) =>
        {
            player.Position = Room.Get(inRoom).Position + offset;
        };
        
        toy.OnSearched += act;
        Unregistering[toy] = act;
        return toy;
    }
    
    public void AddItems()
    {
        Delays.Add(new Delayed(() =>
        {
            ItemPool.Add(ItemType.Medkit);
            ItemPool.Add(ItemType.Medkit);
        }, 30).Run());
        
        Delays.Add(new Delayed(() =>
        {
            ItemPool.Add(ItemType.GunFSP9);
            ItemPool.Add(ItemType.GunFSP9);
            ItemPool.Add(ItemType.ArmorLight);
        }, 60).Run());
        
        Delays.Add(new Delayed(() =>
        {
            ItemPool.Add(ItemType.GunFSP9);
            ItemPool.Add(ItemType.ArmorLight);
            ItemPool.Add(ItemType.ArmorLight);
            ItemPool.Add(ItemType.GunCrossvec);
            ItemPool.Add(ItemType.GunCrossvec);
        }, 90).Run());
        
        Delays.Add(new Delayed(() =>
        {
            ItemPool.Add(ItemType.Ammo12gauge);
        }, 120).Run());
        
        Delays.Add(new Delayed(() =>
        {
            ItemPool.Add(ItemType.Ammo762x39);
            ItemPool.Add(ItemType.ArmorCombat);
            ItemPool.Add(ItemType.ArmorCombat);
        }, 150).Run());
        
        Delays.Add(new Delayed(() =>
        {
            ItemPool.Add(ItemType.GunShotgun);
            ItemPool.Add(ItemType.GunAK);
        }, 180).Run());
        
        Delays.Add(new Delayed(() =>
        {
            ItemPool.Add(ItemType.ArmorHeavy);
        }, 210).Run());
        
        Delays.Add(new Delayed(() =>
        {
            ItemPool.Add(ItemType.Ammo44cal);
            ItemPool.Add(ItemType.GunRevolver);
            ItemPool.Add(ItemType.GrenadeFlash);
        }, 240).Run());
        
        Delays.Add(new Delayed(() =>
        {
            ItemPool.Add(ItemType.ParticleDisruptor);
        }, 270).Run());
        
        Delays.Add(new Delayed(() =>
        {
            ItemPool.Add(ItemType.GunCom45);
            ItemPool.Add(ItemType.GrenadeHE);
        }, 300).Run());
        
        Delays.Add(new Delayed(() =>
        {
            ItemPool.Add(ItemType.Jailbird);
        }, 330).Run());
        
        Delays.Add(new Delayed(() =>
        {
            Coroutines.Add(new Loop(() => ItemPool.Add(ItemType.ParticleDisruptor), 15));
        }, 360));
    }

    public int RemoveDeadEntries()
    {
        foreach (var player in PlayerSide.ToList())
        {
            if (player.Key.Role.Type == RoleTypeId.Overwatch)
            {
                Remove(player.Key);
            }
            
            if (!player.Key.IsConnected)
            {
                Remove(player.Key);
            }
        }
        return 5;
    }
    
    public float SpawnItems()
    {
        foreach (var i in Player.List)
        {
            SpawnItem();
            SpawnItem();
            SpawnItem();
        }
        return 50;
    }

    public int MakeSureAssigned()
    {
        Log.Info("Making sure assigned");
        foreach (var player in Player.List)
        {
            AssignRole(player);
            HasFlag(player);
        }
        
        foreach (var player in Npc.List)
        {
            AssignRole(player);
            HasFlag(player);
        }
        
        return 10;
    }
    public int Lights()
    {
        Side win = Winning();
        Color? color = null;
        if (win == Side.Mtf)
        {
            color = Color.blue;
        }

        if (win == Side.ChaosInsurgency)
        {
            color = Color.green;
        }

        foreach (var room in Room.List)
        {
            if (color == null)
            {
                room.ResetColor();
                continue;
            }
            
            room.Color = color.Value;
        }
        
        return 5;
    }
    
    public Side Winning()
    {
        Side winningSide = Side.None;
        
        foreach (var pickup in Pickup.List)
        {
            if (pickup.Type != ItemType.KeycardO5)
            {
                continue;
            }

            Side theSide = Side.None;
            
            if (pickup.Room.Type == RoomType.LczArmory)
            {
                theSide = Side.Mtf;
            }
            else if (pickup.Room.Type == RoomType.Lcz173)
            {
                theSide = Side.ChaosInsurgency;
            }

            if (theSide == Side.None)
            {
                return Side.None;
            }

            if (winningSide == Side.None)
            {
                winningSide = theSide;
                continue;
            }

            if (winningSide != theSide)
            {
                return Side.None;
            }
        }
        
        return winningSide;
    }
    
    public void SpawnItem()
    {
        Pickup.CreateAndSpawn(ItemPool.RandomItem(), Rooms.RandomItem().Position + new Vector3(0, 1, 0), null, null);
    }
    
    #nullable enable
    public Primitive? GetPrimitive(Player player)
    {
        return PlayerPrimitive.GetValueOrDefault(player);
    }
    #nullable disable
    
    public Primitive AddPrimitive(Player player)
    {
        if (GetPrimitive(player) != null)
        {
            return GetPrimitive(player);
        }
        
        Primitive prim = Primitive.Create(primitiveType: PrimitiveType.Cube, flags: PrimitiveFlags.Visible, position: player.Position + new Vector3(0, 1, 0), rotation: Quaternion.identity.eulerAngles, scale: Vector3.one * 0.2f, spawn: true, color: Color.white);
        prim.Transform.parent = player.Transform;
        
        //primitive.Transform.parent = player.Transform;
        PlayerPrimitive[player] = prim;
        return prim;
    }

    public int PrimCheckLoop()
    {
        foreach (var player in Player.List)
        {
            HasFlag(player);
        }
        return 2;
    }
    
    public bool HasFlag(Player player)
    {
        if (!player.HasItem(ItemType.KeycardO5))
        {
            AddPrimitive(player).Color = Color.gray;
            return false;
        }

        var role = GetRole(player);
        var room = player.CurrentRoom.Type;
        
        if (role == Side.ChaosInsurgency && room == RoomType.Lcz173)
        {
            AddPrimitive(player).Color = Color.green;
        }
        
        else if (role == Side.Mtf && room == RoomType.LczArmory)
        {
            AddPrimitive(player).Color = Color.blue;
        }
        else
        {
            AddPrimitive(player).Color = Color.red;
        }
        return true;
    }
    
    public void RemovePrimitive(Player player)
    {
        var prim = GetPrimitive(player);
        if (prim == null)
        {
            return;
        }
        
        prim.Destroy();
        PlayerPrimitive.Remove(player);
    }

    public void OnRespawningTeamEventArgs(RespawningTeamEventArgs ev)
    {
        ev.IsAllowed = false;
    }

    public void OnInteractingDoorEventArgs(InteractingDoorEventArgs ev)
    {
        if (ev.Door.Type != DoorType.LczArmory && ev.Door.Type != DoorType.Scp173Gate)
        {
            return;
        }
        
        if(ev.Player.CurrentItem is not Keycard keycardItem)
        {
            ev.IsAllowed = false;
        }

        ev.IsAllowed = true;
    }
    
    public void OnPickupAddedEventArgs(PickupAddedEventArgs ev)
    {
        if (ev.Pickup.Type != ItemType.KeycardO5)
        {
            return;
        }
        
        var y = LightSourceToy.Create(ev.Pickup.Transform);
        
        y.Type = LightType.Point;
        y.ShadowType = LightShadows.Hard;
        y.Color = Color.white;
        y.Range = 0.5f;
        y.Intensity = 0.5f;
        
        var compo = ev.Pickup.GameObject.AddComponent<BasesItemLightBehaviour>();
        compo.StartIt(y);
        Components.Add(compo);
        Toys.Add(y);
    }
    public void OnPickingUpItemEventArgs(PickingUpItemEventArgs ev)
    {
        if (ev.Pickup.Type != ItemType.KeycardO5)
        {
            return;
        }
        
        if (ev.Player.HasItem(ItemType.KeycardO5))
        {
            ev.IsAllowed = false;
        }
    }
    
    public void OnItemAddedEventArgs(ItemAddedEventArgs ev)
    {
        HasFlag(ev.Player);
    }
    
    public void OnItemRemovedEventArgs(ItemRemovedEventArgs ev)
    {
        HasFlag(ev.Player);
    }
    
    public void OnVerified(VerifiedEventArgs ev)
    {
        AssignRole(ev.Player);
        AddPrimitive(ev.Player);
    }
    
    public void OnLeft(LeftEventArgs ev)
    {
        Remove(ev.Player);
        RemovePrimitive(ev.Player);
    }

    public int EndCondition()
    {
        var win = Winning();

        if (win == Side.ChaosInsurgency && Player.List.Where(x => x.Role.Side == Side.Mtf).ToList().Count == 0)
        {
            foreach (var player in Player.List)
            {
                player.Broadcast(8, "Vyhráli CI!");
                EndEvent();
            }
        }
        
        if (win == Side.Mtf && Player.List.Where(x => x.Role.Side == Side.ChaosInsurgency).ToList().Count == 0)
        {
            foreach (var player in Player.List)
            {
                player.Broadcast(8, "Vyhrál MTF!");
                EndEvent();
            }
        }
        
        return 1;
    }
    
    public int FastSpawn()
    {
        var win = Winning();
        
        if (win == Side.None || win == Side.ChaosInsurgency && !SomeoneAround(Spawn.CISpawn, 10, Side.ChaosInsurgency))
        {
            SetupSpectator(Side.ChaosInsurgency);
        }
        
        if (win == Side.None || win == Side.Mtf && !SomeoneAround(Spawn.MTFSpawn, 10, Side.Mtf))
        {
            SetupSpectator(Side.Mtf);
        }
        
        return 10;
    }

    public int LongSpawn()
    {
        var win = Winning();
        if (win == Side.None || win == Side.ChaosInsurgency)
        {
            SetupSpectator(Side.ChaosInsurgency, false);
        }
        
        if (win == Side.None || win == Side.Mtf)
        {
            SetupSpectator(Side.Mtf, false);
        }
        
        return 60;
    }

    /// <summary>
    /// Will setup the nearest spectator from list.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="shouldBreak"></param>
    public void SetupSpectator(Side from, bool shouldBreak = true)
    {
        foreach (var player in Player.List)
        {
            if (GetRole(player) != from)
            {
                continue;
            }
            
            if (player.Role.IsAlive)
            {
                continue;
            }

            if (player.Role.Type == RoleTypeId.Overwatch)
            {
                continue;
            }
            
            Setup(player);
            if (shouldBreak)
            {
                break;
            }
        }
    }

    public void Setup(Player player)
    {
        SetRole(player);
        if (GetRole(player) == Side.ChaosInsurgency)
        {
            player.Position = Spawn.CISpawn;
            return;
        }
        
        player.Position = Spawn.MTFSpawn;
    }
    
    public bool SomeoneAround(Vector3 position, float radius, Side ignored = Side.None)
    {
        foreach (var player in Player.List)
        {
            if (player.Role.Side == ignored)
            {
                continue;
            }
            
            var dist = Vector3.Distance(player.Position, position);
            if (dist < radius)
            {
                return true;
            }
        }
        
        return false;
    }
    
    public Side GetRole(Player player)
    {
        AssignRole(player);
        return PlayerSide[player];
    }
    
    public void SetRole(Player player)
    {
        AssignRole(player);

        if (PlayerSide[player] == Side.Mtf)
        {
            player.Role.Set(RoleTypeId.NtfSergeant, SpawnReason.Respawn, RoleSpawnFlags.None);
            player.ClearInventory();
            player.ClearAmmo();
            player.Position = Spawn.MTFSpawn;
            return;
        }
        
        player.Role.Set(RoleTypeId.ChaosRifleman, SpawnReason.Respawn, RoleSpawnFlags.None);
        player.ClearInventory();
        player.ClearAmmo();
        player.Position = Spawn.CISpawn;
    }
    
    public bool HasRole(Player player)
    {
        return PlayerSide.ContainsKey(player);
    }
    
    public void AssignRole(Player player)
    {
        if (HasRole(player))
        {
            return;
        }
        
        if (Count.MTF == Count.CI)
        {
            AddTo(player, Side.Mtf);
            return;
        }

        if (Count.MTF > Count.CI)
        {
            AddTo(player, Side.ChaosInsurgency);
            return;
        }
        
        AddTo(player, Side.Mtf);
    }
    
    public void AddTo(Player player, Side side)
    {
        if (side == Side.Mtf)
        {
            Count = (Count.MTF + 1, Count.CI);
            PlayerSide[player] = Side.Mtf;
        }
        
        if (side == Side.ChaosInsurgency)
        {
            Count = (Count.MTF, Count.CI + 1);
            PlayerSide[player] = Side.ChaosInsurgency;
        }
    }

    public void Remove(Player player)
    {
        if (!PlayerSide.TryGetValue(player, out var side))
        {
            return;
        }

        if (side == Side.ChaosInsurgency)
        {
            Count = (Count.MTF, Count.CI - 1);
        }
        
        if (side == Side.Mtf)
        {
            Count = (Count.MTF - 1, Count.CI );
        }
        
        PlayerSide.Remove(player);
    }

    public string Execute(Player player, List<string> args)
    {
        switch (args[0].ToLower())
        {
            case "relative":
                return $"Your room is: {player.CurrentRoom.Name} & relevant position is: {player.Position - player.CurrentRoom.Position}";
            case "ci":
                if(args.Count < 2)
                    return "!Input player turned into ci";
                if (!Player.TryGet(args[1], out var plr))
                {
                    Remove(plr);
                    AddTo(plr, Side.Mtf);
                }
                break;
            case "mtf":
                if(args.Count < 2)
                    return "!Input player turned into mtf";
                if (!Player.TryGet(args[1], out plr))
                {
                    Remove(plr);
                    AddTo(plr, Side.ChaosInsurgency);
                }
                break;
        }
        
        return "!just don't write anything for help";
    }

    public string HelpMessage(Player player)
    {
        return "Availiable command is: relative.";
    }
}

public class BasesItemLightBehaviour : MonoBehaviour
{
    public Pickup Pickup { get; set; }
    public LightSourceToy Toy { get; set; }
    
    public bool Started { get; set; } = false;
    private void Awake()
    {
        Pickup = Exiled.API.Features.Pickups.Pickup.Get(gameObject);
        if (Pickup == null)
        {
            Destroy(this);
        }
    }

    public void StartIt(LightSourceToy toy)
    {
        Toy = toy;
        Started = true;
    }
    
    public float T { get; set; }
    
    private void Update()
    {
        T += Time.deltaTime;
        if (T < 0.6f)
        {
            return;
        }
        
        T = 0;
        
        if (!Started)
        {
            return;
        }

        if (Pickup.Room is null)
        {
            Toy.Color = Color.gray;
            return;
        }
        
        if (Pickup.Room.Type == RoomType.LczArmory)
        {
            Toy.Color = Color.blue;
        }
        else if (Pickup.Room.Type == RoomType.Lcz173)
        {
            Toy.Color = Color.green;
        }
        else
        {
            Toy.Color = Color.gray;
        }
    }
}