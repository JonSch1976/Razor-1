# Razor Logging System

## Overview

A centralized, thread-safe logging system for Razor that replaces scattered `MessageBox.Show`, `StreamWriter`, and `Console.WriteLine` calls with a unified logging infrastructure.

## Features

? **Thread-Safe**: Uses concurrent queue and background thread for async logging  
? **Multiple Log Levels**: Trace, Debug, Info, Warning, Error, Fatal  
? **Dual Output**: Logs to both file and debug output  
? **Auto-Rotation**: Automatic log file creation with timestamps  
? **Performance**: Async writing doesn't block main thread  
? **Cleanup**: Built-in old log cleanup  
? **Categorized**: Category-specific loggers for better organization  

## Basic Usage

### Simple Logging

```csharp
using Assistant.Core.Logging;

// Basic logging
Logger.Info("Application started");
Logger.Warning("Config file not found, using defaults");
Logger.Error("Failed to connect to server");

// With formatting
Logger.Info("Player {0} logged in at {1}", playerName, DateTime.Now);
Logger.Error("Invalid item ID: {0}", itemId);
```

### Exception Logging

```csharp
try
{
    // Some code
}
catch (Exception ex)
{
    Logger.Error(ex, "Failed to load profile");
}
```

### Log Levels

```csharp
Logger.Trace("Detailed trace information");     // Very verbose
Logger.Debug("Debug information");          // Development info
Logger.Info("Informational message");           // General info
Logger.Warning("Warning message");  // Warnings
Logger.Error("Error message");        // Errors
Logger.Fatal("Fatal error");           // Critical failures
```

## Advanced Usage

### Category-Specific Logging

```csharp
using Assistant.Core.Logging;

// Network logging
LogCategories.Network.PacketSent(0x73, 128);
LogCategories.Network.PacketReceived(0x1B, 37);
LogCategories.Network.ConnectionError(ex);

// Macro logging
LogCategories.Macros.MacroStarted("Healing Loop");
LogCategories.Macros.MacroFinished("Healing Loop", TimeSpan.FromSeconds(45));
LogCategories.Macros.MacroError("Healing Loop", ex);

// Script logging
LogCategories.Scripts.ScriptStarted("farming.txt");
LogCategories.Scripts.ScriptFinished("farming.txt", TimeSpan.FromMinutes(2));
LogCategories.Scripts.ScriptError("farming.txt", 42, ex);

// Configuration logging
LogCategories.Configuration.ProfileLoaded("MyCharacter");
LogCategories.Configuration.SettingChanged("AlwaysOnTop", false, true);

// UI logging
LogCategories.UI.FormOpened("MacroEditor");
LogCategories.UI.UIError("MacroEditor", ex);
```

### Logging with Message Boxes

Replace old `MessageBox.Show` + manual logging:

```csharp
// OLD WAY
MessageBox.Show("Profile could not be loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
// logging was missing or inconsistent

// NEW WAY
LoggerExtensions.ErrorWithMessageBox("Error", "Profile could not be loaded");
// Automatically logs AND shows message box

// With exception
LoggerExtensions.ErrorWithMessageBox("Profile Error", ex, "Failed to load profile");

// Warning
LoggerExtensions.WarningWithMessageBox("Warning", "Setting not found, using default");

// Info
LoggerExtensions.InfoWithMessageBox("Success", "Profile saved successfully");
```

### Safe Execution with Logging

```csharp
// Execute with automatic error logging
LoggerExtensions.SafeExecute(() => 
{
    SaveProfile();
}, "SaveProfile");

// Execute with return value
var result = LoggerExtensions.SafeExecute(() => 
{
    return LoadConfig();
}, "LoadConfig", defaultValue: null);
```

## Configuration

### Set Minimum Log Level

```csharp
// Only log Info and above (default)
Logger.MinimumLevel = LogLevel.Info;

// Log everything (very verbose)
Logger.MinimumLevel = LogLevel.Trace;

// Only errors and fatal
Logger.MinimumLevel = LogLevel.Error;
```

### Enable/Disable Outputs

```csharp
// Disable file logging
Logger.LogToFile = false;

// Disable debug output
Logger.LogToDebug = false;
```

### Log File Management

```csharp
// Get current log file path
string logPath = Logger.CurrentLogFile;

// Rotate to new log file
Logger.RotateLogFile();

// Cleanup old logs (keep last 7 days)
Logger.CleanupOldLogs(7);
```

## Migration Guide

### Replace MessageBox Errors

**Before:**
```csharp
try
{
    LoadProfile(name);
}
catch (Exception ex)
{
    MessageBox.Show($"Error loading profile: {ex.Message}", "Error", 
        MessageBoxButtons.OK, MessageBoxIcon.Error);
}
```

**After:**
```csharp
try
{
    LoadProfile(name);
}
catch (Exception ex)
{
    LoggerExtensions.ErrorWithMessageBox("Error", ex, "Error loading profile");
}
```

### Replace StreamWriter Logging

**Before:**
```csharp
using (StreamWriter txt = new StreamWriter("Crash.log", true))
{
    txt.AutoFlush = true;
    txt.WriteLine("Exception @ {0}", DateTime.Now);
    txt.WriteLine(exception.ToString());
}
```

**After:**
```csharp
Logger.Fatal(exception as Exception, "Application crash");
```

### Replace Debug Statements

**Before:**
```csharp
#if DEBUG
Console.WriteLine("Processing packet 0x{0:X2}", packetId);
#endif
```

**After:**
```csharp
Logger.Debug("Processing packet 0x{0:X2}", packetId);
```

## Log File Format

Logs are written to `Razor\Logs\Razor_YYYYMMDD_HHMMSS.log`:

```
================================================================================
Razor Log Started: 2024-01-15 10:30:45
Version: 1.7.0.0
================================================================================

2024-01-15 10:30:45.123 [Info   ] [  1] Engine.Initialize: Application initialized
2024-01-15 10:30:45.234 [Info   ] [  1] Config.LoadProfile: Profile loaded: Default
2024-01-15 10:30:46.345 [Warning] [  1] Network.Connect: Connection attempt timed out
2024-01-15 10:30:47.456 [Error  ] [  1] Macro.Execute: Exception in macro execution
Exception: NullReferenceException
Message: Object reference not set to an instance of an object
Stack Trace: ...
2024-01-15 10:30:48.567 [Info   ] [  5] PacketHandler.Process: Packet 0x73 processed
```

## Log Levels in Detail

| Level   | Purpose | Example Use Case |
|---------|---------|------------------|
| **Trace** | Very detailed diagnostic | Packet contents, variable values |
| **Debug** | Development information | Method entries, conditional branches |
| **Info**  | General information | Application start, profile loaded |
| **Warning** | Unexpected but handled | Config missing, using defaults |
| **Error** | Failures that can continue | Save failed, network timeout |
| **Fatal** | Critical failures | Unhandled exceptions, crashes |

## Best Practices

1. **Use Appropriate Levels**
   - Don't log everything at Info level
   - Use Trace/Debug for development, Info+ for production

2. **Include Context**
   ```csharp
   // Bad
   Logger.Error("Failed");
   
   // Good
   Logger.Error("Failed to load macro '{0}' from '{1}'", macroName, filePath);
   ```

3. **Log Exceptions Properly**
   ```csharp
   // Bad
 Logger.Error(ex.Message);
   
// Good
   Logger.Error(ex, "Context about what was being done");
   ```

4. **Don't Log in Loops**
   ```csharp
   // Bad
   foreach (var item in items)
   {
       Logger.Debug("Processing item {0}", item.Name); // Too much!
 }
   
   // Good
   Logger.Debug("Processing {0} items", items.Count);
   foreach (var item in items)
   {
       // Process without logging each
   }
   Logger.Debug("Finished processing items");
   ```

5. **Use Categories**
   ```csharp
// Instead of generic logging
   Logger.Info("Macro started");
   
   // Use categorized logging
   LogCategories.Macros.MacroStarted(macroName);
   ```

## Performance

- **Async Writing**: Logs are queued and written by background thread
- **No Blocking**: Main thread continues immediately after queuing
- **Buffered**: StreamWriter uses buffering, flushes periodically
- **Minimal Overhead**: When below minimum level, logging is skipped immediately

## Integration with Existing Code

The logger is designed to coexist with existing logging:

1. **Gradually migrate** - No need to change everything at once
2. **Backwards compatible** - Old logging still works
3. **Low risk** - Logger failures don't crash the app

## Initialization & Cleanup

The logger initializes automatically on first use. To properly shut down:

```csharp
// In Engine.Close() or application shutdown
Logger.Shutdown();
```

## Example: Converting a Class

**Before:**
```csharp
public class ProfileManager
{
    public void LoadProfile(string name)
    {
        try
      {
      // Load logic
            MessageBox.Show("Profile loaded", "Success");
   }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error");
    using (StreamWriter w = new StreamWriter("errors.log", true))
            {
    w.WriteLine($"{DateTime.Now}: {ex}");
  }
        }
    }
}
```

**After:**
```csharp
using Assistant.Core.Logging;

public class ProfileManager
{
    public void LoadProfile(string name)
    {
     try
 {
       Logger.Info("Loading profile: {0}", name);
      // Load logic
            LogCategories.Configuration.ProfileLoaded(name);
         LoggerExtensions.InfoWithMessageBox("Success", "Profile loaded");
        }
        catch (Exception ex)
        {
       Logger.Error(ex, "Failed to load profile: {0}", name);
   LoggerExtensions.ErrorWithMessageBox("Error", ex, "Failed to load profile");
     }
    }
}
```

## Troubleshooting

**Logger not writing to file:**
- Check `Logger.LogToFile` is true
- Verify write permissions to Logs directory
- Check `Logger.CurrentLogFile` path

**Too many log files:**
- Call `Logger.CleanupOldLogs(days)` periodically
- Reduce `daysToKeep` parameter

**Logs too verbose:**
- Increase `Logger.MinimumLevel`
- Use appropriate log levels

**Performance concerns:**
- Logger uses async queue, minimal impact
- If needed, increase minimum level in production
