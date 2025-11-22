# Script Call Fix - Final Implementation

## Problem Identified
Called scripts (`call 'checkServerSave'`) were not executing their lines. The calling script would continue immediately instead of pausing.

## Root Causes Found

### Issue #1: Command Return Value (FIXED ?)
**Location:** `Razor\Scripts\Commands.cs` - `CallScript()` method

**Problem:** Was returning `true`, which tells the interpreter "continue to next line"

**Fix:** Changed to return `false` to block the calling script

```csharp
// Line ~1820
ScriptManager.CallScript(best.Lines, best.Name);
return false;  // ? Block caller until callee finishes
```

### Issue #2: CallScript Logic (FIXED ?)
**Location:** `Razor\Scripts\ScriptManager.cs` - `CallScript()` method

**Problem:** The condition for pushing the caller onto the stack was unreliable:
```csharp
// OLD (BROKEN):
if (_activeScriptName != null && !_isScriptCall)
{
    var caller = Interpreter.GetActiveScript();
    Interpreter.PushCall(caller, _activeScriptName);
    Interpreter.SuspendScript();
}
```

**Why it failed:**
- `_activeScriptName` might not be set when the call executes
- `!_isScriptCall` prevented nested calls
- The logic didn't check `ScriptRunning` status

**Fix:** Simplified and improved the logic:
```csharp
// NEW (FIXED):
var activeScript = Interpreter.GetActiveScript();

if (activeScript != null && ScriptRunning)
{
    // Push the currently executing script onto the call stack
    Interpreter.PushCall(activeScript, _activeScriptName ?? "Unknown");
    
    // Suspend the current script
    Interpreter.SuspendScript();
}

// Queue the new script to run next
_isScriptCall = true;
_queuedScript = newScript;
_queuedScriptName = name;
```

### Issue #3: Timer Handling (ALREADY WORKING ?)
**Location:** `Razor\Scripts\ScriptManager.cs` - `ScriptTimer.OnTick()`

The timer already had the correct logic to handle call returns (lines 177-188):

```csharp
if (Interpreter.HasCalls)
{
    Script callingScript = Interpreter.PopCall();
    if (callingScript != null)
    {
        _queuedScript = callingScript;
        _queuedScriptName = "Returning from call";
        // Advance the caller past the call statement
        callingScript.Advance();
        return;
    }
}
```

## How It Works Now

### Execution Flow:

1. **checkTools.razor starts**
   - Timer sets `_activeScriptName = "checkTools"`
   - Script begins executing
   - `_isScriptCall = false`

2. **Line: `call 'checkServerSave'`**
   - `Commands.CallScript()` is called
   - Finds the script
   - Calls `ScriptManager.CallScript(lines, name)`
   - Returns `false` ? **BLOCKS** checkTools

3. **ScriptManager.CallScript() executes**
   - Gets active script from Interpreter
   - `activeScript != null` and `ScriptRunning == true` ?
   - **Pushes checkTools onto call stack**
   - **Suspends checkTools** (sets Interpreter._activeScript = null)
   - Queues checkServerSave
   - Sets `_isScriptCall = true`

4. **Next timer tick**
   - Sees `_queuedScript != null`
   - Resets `_isScriptCall = false`
   - Starts checkServerSave
   - **checkServerSave begins executing**

5. **checkServerSave finishes**
   - All lines execute
   - `ExecuteScript()` returns `false`
   - Timer detects `running == false`
   - Checks `Interpreter.HasCalls` ? **TRUE** ?

6. **Timer pops the call**
   - `PopCall()` returns checkTools script
   - Advances checkTools past the `call` line
   - Queues checkTools to resume
   - **checkTools resumes execution**

## What You Should See

### Before (Broken):
```
Enter checkTools
Exit checkTools
```
*checkServerSave never ran*

### After (Fixed):
```
Enter checkTools
[CALL] Request to call script: 'checkServerSave'
[CallScript] Starting call to 'checkServerSave'
[CallScript] Pushing active script 'checkTools' onto call stack
[CallScript] Queued 'checkServerSave' for execution
ENTER Check_Server_Save
EXIT Check_Server_Save
Exit checkTools
```

## Debug Messages Added

To help diagnose issues, the following debug messages were added:

### In Commands.CallScript():
- `[CALL] Request to call script: '{scriptName}' depth={depth}`
- `[CALL] Found script: '{name}' (len={lines}) path='{path}'`
- `[CALL] Queued '{name}' for execution (blocking caller)`

### In ScriptManager.CallScript():
- `[CallScript] Starting call to '{name}', activeScript={active}, Running={running}`
- `[CallScript] Pushing active script '{name}' onto call stack`
- `[CallScript] Queued '{name}' for execution`

### In ReturnFromCall():
- `[RETURN] Early return requested (depth={depth})`

## Files Modified

| File | Lines | Description |
|------|-------|-------------|
| Razor\Scripts\Commands.cs | ~1820 | Changed return value from `true` to `false` |
| Razor\Scripts\ScriptManager.cs | 613-640 | Rewrote CallScript() logic with proper checks |

## Build Status
? **BUILD SUCCESSFUL**
? **Ready to test**

## Testing Checklist

- [ ] Run checkTools.razor
- [ ] Verify "ENTER Check_Server_Save" appears
- [ ] Verify "EXIT Check_Server_Save" appears  
- [ ] Verify "Exit checkTools" appears after
- [ ] Test nested calls (call within a call)
- [ ] Test early `return` from called script

## Next Steps

1. **Test your mining scripts**
2. **Check for debug messages in Razor output**
3. **If it still doesn't work:**
   - Share the debug output
   - Check if scripts are being loaded (UORenaissance path)
   - Verify script files exist and are readable

---

**The call system should now work correctly!** ??
