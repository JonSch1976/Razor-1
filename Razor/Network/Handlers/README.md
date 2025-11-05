# Network Packet Handlers

## Overview

The packet handling system has been refactored into specialized handler classes for better organization and maintainability.

## Structure

```
Razor\Network\Handlers\
??? BaseHandlers.cs          - Initialization and common functionality
??? MobileHandlers.cs        - Mobile updates, movement, stats
??? ItemHandlers.cs        - Items, containers, equipment
??? CombatHandlers.cs        - Damage, attacks, war mode
??? CommunicationHandlers.cs - Speech, messages, gumps
??? TargetHandlers.cs        - Targeting system
??? ServerHandlers.cs        - Login, server changes, features
```

## Handler Categories

### MobileHandlers.cs
**Packet IDs:** 0x11, 0x17, 0x1D, 0x20, 0x2D, 0x77, 0x78, 0xA1, 0xA2, 0xA3

Handles all mobile-related packets:
- Mobile status updates (stats, hits, mana, stam)
- Mobile movement and incoming
- Mobile removal
- Stat changes with overhead notifications
- Party stats display

**Key Features:**
- Last target highlighting
- Stealth step counting
- Health bar percentage coloring
- Party stat display

### ItemHandlers.cs
**Packet IDs:** 0x07, 0x08, 0x13, 0x1A, 0x24, 0x25, 0x27, 0x2E, 0x3C, 0xF3

Handles all item-related packets:
- World items (ground items)
- Container management
- Equipment updates
- Lift/drop requests
- SA (Stygian Abyss) world items

**Key Features:**
- Auto-open corpses
- Scavenger agent integration
- Search exemption handling
- Counter integration

### CombatHandlers.cs
**Packet IDs:** 0x05, 0x0B, 0x2C, 0x72, 0xAA, 0xAF

Handles combat-related packets:
- Attack requests
- Damage tracking
- War mode changes
- Death animations
- Combatant tracking

**Key Features:**
- Damage tracker integration
- Death screenshot capture
- Last combatant tracking
- Waypoint creation on death

### CommunicationHandlers.cs
**Packet IDs:** 0x1C, 0xAD, 0xAE, 0xB0, 0xB1, 0xC1, 0xCC, 0xDD

Handles communication packets:
- ASCII and Unicode speech
- Localized messages
- Gump display and response
- System messages

**Key Features:**
- Speech filtering
- Message overhead display
- Gump handling
- Spell powerword formatting

### TargetHandlers.cs
**Packet IDs:** 0x6C

Handles targeting system:
- Target requests
- Target responses
- Target cancellation

**Key Features:**
- Last target management
- Smart targeting (harmful/beneficial)
- Target queue system
- Heal poison blocking

### ServerHandlers.cs
**Packet IDs:** 0x1B, 0x55, 0x5B, 0x76, 0x8C, 0xA8, 0xB9, 0xBC, 0xBE, 0xBF, 0xC8, 0xDF

Handles server and session management:
- Login confirmation
- Server list
- Server changes
- Features negotiation
- Season changes
- Map patches

**Key Features:**
- Profile loading on login
- UO Assist integration
- Title bar updates
- Session management

## Usage

### Initialization

All handlers are automatically registered during startup:

```csharp
PacketHandlers.Initialize();
```

### Adding New Handlers

1. Choose the appropriate handler class based on functionality
2. Add your handler method:

```csharp
private static void MyNewHandler(PacketReader p, PacketHandlerEventArgs args)
{
    Logger.Debug("MyNewHandler called for packet 0xXX");
    
    // Your packet handling logic
    
    if (shouldBlock)
    args.Block = true;
}
```

3. Register in the `Register()` method:

```csharp
PacketHandler.RegisterServerToClientViewer(0xXX, MyNewHandler);
```

### Logging Integration

All handlers integrate with the new logging system:

```csharp
// Simple logging
Logger.Debug("Packet received");
Logger.Info("Mobile updated: {0}", mobile.Name);

// Category logging
LogCategories.Network.PacketReceived(packetId, length);
LogCategories.Network.PacketSent(packetId, length);

// Exception logging
try
{
    // Handler logic
}
catch (Exception ex)
{
    Logger.Error(ex, "Error handling packet 0x{0:X2}", packetId);
}
```

## Packet Flow

```
Client/Server
     ?
PacketHandler (Razor\Network\PacketHandler.cs)
     ?
Appropriate Handler (MobileHandlers, ItemHandlers, etc.)
     ?
World Update / UI Update / Agent Action
```

## Best Practices

1. **Logging**: Use appropriate log levels
- `Trace` for packet details
   - `Debug` for handler flow
   - `Info` for important state changes
   - `Error` for exceptions

2. **Null Checks**: Always check World.Player before accessing

3. **Blocking**: Set `args.Block = true` to prevent packet from reaching client/server

4. **Performance**: Keep handlers lightweight, defer heavy processing

5. **Documentation**: Add XML comments to new handlers

## Common Patterns

### Viewing vs Filtering

```csharp
// Viewer - Read-only, cannot block or modify
PacketHandler.RegisterServerToClientViewer(0xXX, MyViewer);

// Filter - Can block or modify
PacketHandler.RegisterServerToClientFilter(0xXX, MyFilter);
```

### Blocking Packets

```csharp
private static void BlockExample(Packet p, PacketHandlerEventArgs args)
{
    if (ShouldBlock())
    {
  args.Block = true;
      Logger.Debug("Packet 0x{0:X2} blocked", p.PacketID);
    }
}
```

### Modifying Packets

```csharp
private static void ModifyExample(Packet p, PacketHandlerEventArgs args)
{
    ushort hue = p.ReadUInt16();
    
    if (ShouldChangeHue())
    {
    p.Seek(-2, System.IO.SeekOrigin.Current);
        p.Write((ushort)newHue);
 }
}
```

## Migration from Old System

The old `PacketHandlers` class has been split up. To find a handler:

1. Check the packet ID in the handler documentation
2. Look in the appropriate handler class
3. Search across all handler files if needed

## Debugging

Enable packet logging in Settings:

```csharp
Packet.Logging = true;
Logger.MinimumLevel = LogLevel.Trace;
```

Log file location: `Razor\Logs\Razor_YYYYMMDD_HHMMSS.log`

## Performance Considerations

- Handlers are called for EVERY packet
- Keep processing minimal
- Use caching where appropriate
- Defer expensive operations to Timer callbacks
- Profile handler performance if issues arise

## Future Improvements

- [ ] Add packet statistics tracking
- [ ] Implement packet replay for debugging
- [ ] Add unit tests for critical handlers
- [ ] Performance profiling integration
- [ ] Handler metrics dashboard
