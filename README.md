# Very Epic Event Plugin
This is event plugin I mainly develop for server called Project Echo [SCP:SL] (Czech server).
From developer page this plugin might look very stupid and questionable, everything is mainly develop to prevent ANY memory leaks **KNOWN BUG: When event executed two (2) times bad things will happen :(**. This plugin will not be using any dependencies in the future and will be kept simplified as much as it can be.

### Events
Bases
This event takes place in Light Containment Zone. Two factions exist, the MTF and Chaos. Each have at the start of round three (3) O5 keycards and trying to steal from one another. Alongside stealing there will be items spawned everywhere in Light Containment Zone (not in bases).
The goal of this is to take all O5 keycards to your base MTF (LCZ Armory) CI (Peanut Chamber). Keycards have light on them showing where they are and peanut chamber normally inacsessible doors have intractable toy meaning if you want to pick up the locked door in peanut chamber it will teleport you to the other door and reversed. This is done, so keycards can't be unaccsessible in PT chamber.
When all keycards are in base then all zone lights will be turned to the winning side CI (green) or MTF (dark blue). This means the other team will not be able to spawn until they collected keycards back to their base. This gives the other side time to wipe the other team and win the game (if they are able to defend their base).

Overwatch is from this game removed, so you can spawn. **KNOWN BUG: Overwatch people will be spawned at the start of event.**.

### Event command
this command can be executed by anyone with Remote Admin permissions. **TODO: Permissions**.
Arguments:
- Event Play <Id> - Will start event with Id to this one.
- Event Stop <Id> - Will stop event with Id.
- Event List - will give information about all registered events.
- Event <id> <arguments> - this will play any commands registered from the events itself. If you won't have any arguments written and event has help message implemented it will write it.

# API
I hate making documentation so much :pray:, so it will take me some time, but the API is not complicated. Youa are able to register your own events with this API via using RegisterAll() method in SlEvent class.
Just know that if you register all and will have duplicate id to other event it will rewrite it, because this plugin uses dictionaries instead of list. If this happens when event is playing it will be unable to stop.
