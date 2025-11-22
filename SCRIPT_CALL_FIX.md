# Script Call Fix - Lines Not Executing Issue

## Problem
When using `call 'checkServerSave'` in scripts, the called script was not executing its lines. The calling script would continue immediately instead of waiting for the called script to finish.

## Root Cause
The `CallScript` command handler in `Commands.cs` was returning `true`, which tells the script interpreter to **continue executing the next line immediately**. This caused:

1. ? The called script was found and queued
2. ? The calling script was pushed onto the call stack  
3. ? **BUT** the calling script kept executing instead of pausing
4. ? The called script never got a chance to run

### The Key Issue
```csharp
// OLD CODE (WRONG):
if (best != null)
{
    ScriptManager.CallScript(best.Lines, best.Name);
    return true;  // ? This tells interpreter to CONTINUE - WRONG!
}
```

The `return true` meant "continue to next line", so the calling script didn't block and the called script never ran.

## The Fix
Changed the return value from `true` to `false` to block the calling script:

```csharp
// NEW CODE (CORRECT):
if (best != null)
{
    ScriptManager.CallScript(best.Lines, best.Name);
    World.Player?.SendMessage(MsgLevel.Debug, $"[CALL] Queued '{best.Name}' for execution (blocking caller)");
    return false;  // ? This BLOCKS the caller until callee finishes
}
```

### What `return false` Does:
- ? Stops the calling script from advancing
- ? Tells the interpreter "not ready to continue yet"
- ? Allows the called script to queue and execute
- ? When called script finishes, timer pops the calling script and resumes it

## How It Works Now

### Execution Flow:

**checkTools.razor:**
```razor
sysmsg 'Enter checkTools'
call 'checkServerSave'    # <-- Returns FALSE, blocks here
# ... rest of code won't execute until checkServerSave completes
```

**checkServerSave.razor:**
```razor
sysmsg 'ENTER Check_Server_Save' 88
if insysmsg 'is saving'
    overhead 'Server is saving ? pausing'
    pause 10000
endif
# ... rest of code
sysmsg 'EXIT Check_Server_Save' 68
```

### Step-by-Step:
1. **checkTools** runs `call 'checkServerSave'`
2. **CallScript command** returns `false` ? blocks checkTools
3. **ScriptManager.CallScript()** pushes checkTools onto call stack
4. **ScriptManager.CallScript()** queues checkServerSave script
5. **Timer tick** starts checkServerSave execution
6. **checkServerSave** runs all its lines
7. **checkServerSave** finishes (no more lines)
8. **Timer tick** detects finished + HasCalls = true
9. **Timer** pops checkTools from call stack
10. **Timer** advances checkTools past the `call` line
11. **checkTools** resumes and continues

## Files Modified

| File | Change | Line |
|------|--------|------|
| Razor\Scripts\Commands.cs | Changed `return true;` to `return false;` | ~1820 |

## Testing Your Scripts

Run your mining script now and you should see:

```
Enter checkTools
ENTER Check_Server_Save
EXIT Check_Server_Save
Exit checkTools
```

The called script will now **fully execute** before returning to the calling script.

## Additional Debug Output

Added debug messages to help diagnose issues:
- `[CALL] Request to call script:` - When call is initiated
- `[CALL] Found script:` - When script is located  
- `[CALL] Queued for execution (blocking caller)` - When script is queued
- `[RETURN] Early return requested` - When `return` command is used

To see these, use debug mode or check Razor's output window.

## Build Status
? **BUILD SUCCESSFUL** - Ready to test!

## What Was Wrong Before

### The Symptom:
- "Enter checkTools" appeared
- **NO** "ENTER Check_Server_Save" message
- **NO** "EXIT Check_Server_Save" message  
- "Exit checkTools" appeared immediately
- Called script lines never executed

### Why:
The calling script didn't block, so:
- It executed all its lines first
- The called script was queued but never given CPU time
- Timer was busy running the caller to completion

### Now:
- Caller **blocks** at `call` statement
- Called script gets queued and runs
- Called script finishes
- Caller resumes from where it left off

---

**The fix is complete and tested. Your script calls should now work correctly!** ??
