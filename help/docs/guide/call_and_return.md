---
title: Script Call, Return, and Parameters
description: How to use subroutine calls, return values, and parameter passing in Razor scripts.
---

# Script Call / Return / Parameters

Razor scripting now supports calling other scripts like subroutines, passing parameters, and retrieving return values.

## Overview

Features:
- `call 'scriptName'` executes another script and resumes the caller when it finishes
- Supports nested calls (scripts can call other scripts) up to a maximum depth of **32**
- Optional arguments: `call 'scriptName' arg1 arg2 ...`
- Parameters available inside the called script as global variables: `arg0`, `arg1`, ..., `argc`
- Scripts can return early using `return` (optionally with a return value)
- Return value is exposed to the caller in global variable `_ret`
- Safe guard: if depth reaches 32 further calls are ignored with an error message

## Max Depth Guard
The call stack has a hard limit of **32** nested calls. When attempting to exceed it:
```
sysmsg 'Max call depth 32 reached. Call canceled.'
```
Reason: prevent runaway recursion / infinite loops.
Best practice: ensure recursive patterns have termination conditions.

## Calling a Script

Syntax:
```
call 'ScriptName' [arg1] [arg2] [...]
```
Examples:
```
call 'HealSelf'
call 'SmeltOre' 50 'iron'
call 'Check_Server_Save'
```

## Accessing Parameters in Called Script
Inside the called script:
- `argc` = number of arguments passed
- `arg0`, `arg1`, ... contain the argument strings in order

Example (`HealSelf.razor`):
```
// HealSelf.razor
// Usage: call 'HealSelf' <hpThreshold> <mode>
if argc >= 1
    if hp < arg0
        if argc >= 2 and arg1 = 'bandage'
            if findtype 'bandage' backpack
                dclicktype 'bandage'
                waitfortarget
                target 'self'
                return 'bandaged'
            endif
        else
            cast 'heal'
            waitfortarget
            target 'self'
            return 'healed'
        endif
    else
        return 'skip'
    endif
endif
return 'done'
```
Caller:
```
call 'HealSelf' 50 'bandage'
if _ret = 'bandaged'
    overhead 'Used bandage'
endif
```

## Returning Early
Use `return` with or without value:
```
return          // no value
return 'ok'     // sets _ret = 'ok' for caller
```
If no value is provided, `_ret` is cleared (previous value removed).

## Return Value Handling
After a called script finishes (naturally or via `return`), the caller resumes at the line after the `call` statement. The return value (if provided) is stored in `_ret`.

Caller example:
```
overhead 'Before call'
call 'MineNode' 1450 1249
if _ret = 'depleted'
    overhead 'Node depleted'
endif
overhead 'After call'
```

`MineNode.razor` example:
```
// Usage: call 'MineNode' <x> <y>
if argc >= 2
    overhead 'Mining at' arg0 arg1
endif
while not insysmsg 'there is no metal'
    // mining logic ...
endwhile
return 'depleted'
```

## Nested Calls
Scripts can call other scripts, and each caller is stored on a call stack.
```
call 'Level1'

// Level1.razor
overhead 'In level 1'
call 'Level2'
overhead 'Back in level 1'
return 'lvl1done'

// Level2.razor
overhead 'In level 2'
return 'lvl2done'
```
Caller can inspect `_ret` after each call.

## Argument Reference
| Variable | Description |
|----------|-------------|
| `argc`   | Number of arguments passed |
| `arg0`..`argN` | Argument values (string) |
| `_ret`   | Return value from last completed call |

## Error Handling
- If script name cannot be found, a warning is shown and execution continues
- `return` with more than 1 argument raises a runtime error
- Calling with zero args sets `argc` = 0 and clears previous args (does not clear `_ret` unless call returns nothing)

## Best Practices
- Always document expected parameters at top of called script
- Use meaningful return codes (`'ok'`, `'skip'`, `'error'`)
- Clear temporary variables before returning if they should not persist
- Avoid infinite recursive calls (no max depth enforced yet)

## Mining Example Integration
Main loop calling movement helpers:
```
call 'Left_Cove_To_Forge'
// smelting logic...
call 'Left_Cove_Forge_Back_To_Mining'
```
Movement scripts ignore parameters; they just execute path logic.

## Limitations
- Parameters are strings; convert manually if numeric
- No named parameters yet (future enhancement)
- Return value is single string; for multi-values encode (e.g. CSV) or use global variables

## Planned Enhancements (Optional)
- Named parameters: `call 'Script' hp=50 mode='fast'`
- Local scoping for args (currently globals)
- Max call depth safeguard
- Structured return (list or map)

## Troubleshooting
| Issue | Cause | Fix |
|-------|-------|-----|
| `_ret` not set | Callee did not `return` with value | Add `return 'value'` |
| Args missing | Called script expected more args | Check `argc` before use |
| Caller stops | Runtime error in callee | Review error message & line |
| Infinite loop | Recursive calls without exit | Add base case / depth check |

## Quick Cheat Sheet
```
call 'Script' a b c      // run script with 3 args
argc                      // number of args
arg0 / arg1 / arg2        // first, second, third args
return 'done'             // return value
_ret                      // value received by caller
```

---
Updated: Parameter passing & return value support added.
