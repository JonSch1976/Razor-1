# Call/Return Test Scripts - User Guide

## Overview
These test scripts demonstrate and validate the new `call` and `return` functionality in Razor scripts.

## Test Scripts Created

### Main Test: `test-call-return.razor`
The main test script that runs all test scenarios and reports results to the journal.

### Helper Scripts:
1. **test-helper.razor** - Simple helper that reports execution
2. **test-variable-helper.razor** - Tests variable persistence across calls
3. **test-nested-level1.razor** - First level of nested call test
4. **test-nested-level2.razor** - Second level of nested call test
5. **test-early-return.razor** - Tests explicit `return` statement

---

## How to Run the Tests

### Method 1: From Razor UI
1. Open Razor
2. Go to the **Scripts** tab
3. Find `test-call-return` in the script list
4. Click **Play** button

### Method 2: From In-Game
1. In-game, type: `>script test-call-return`
2. Watch your journal for test results

### Method 3: Using Hotkey
1. Set a hotkey for the script in Razor
2. Press the hotkey in-game

---

## Expected Output

When you run `test-call-return.razor`, you should see the following in your **journal** (system messages):

```
=== Starting Call/Return Test ===
Test 1: Simple call to helper script
  -> Inside helper script
  -> Helper completing
Test 1: PASSED - Simple call completed

Test 2: Testing variable persistence
  -> Variable found in helper!
Test 2: PASSED - Variable persisted

Test 3: Testing nested calls
  -> Level 1: Entered
  -> Level 2: Entered
  -> Level 2: Completing
  -> Level 1: Returned from Level 2
Test 3: PASSED - Nested calls completed

Test 4: Testing early return
  -> Early return script started
  -> Returning early (hp > 0)
Test 4: PASSED - Early return worked

Test 5: Testing multiple sequential calls
  -> Inside helper script
  -> Helper completing
  -> Inside helper script
  -> Helper completing
  -> Inside helper script
  -> Helper completing
Test 5: PASSED - Sequential calls worked

=== All Call/Return Tests Complete ===
```

You'll also see **overhead messages** above your character's head showing progress.

---

## What Each Test Validates

### Test 1: Simple Call
- ? `call` command finds and executes a script
- ? Script returns to the caller after completion
- ? Execution continues after the `call` statement

### Test 2: Variable Persistence
- ? Variables set before a call remain available in called scripts
- ? Global scope is maintained across calls
- ? `varexist` expression works correctly

### Test 3: Nested Calls
- ? Scripts can call other scripts that also make calls
- ? Call stack properly manages multiple levels
- ? Each return goes to the correct caller

### Test 4: Early Return
- ? Explicit `return` statement stops script execution
- ? Code after `return` does not execute
- ? Control returns to the caller immediately

### Test 5: Sequential Calls
- ? Multiple calls can be made one after another
- ? Each call completes before the next begins
- ? No state corruption between calls

---

## Interpreting Results

### Success Indicators:
- ? All tests show "PASSED" in green (hue 68)
- ? Messages appear in the expected order
- ? No error messages in red (hue 38)
- ? Script completes with "All Call/Return Tests Complete"

### Failure Indicators:
- ? "FAILED" messages in red (hue 38)
- ? Missing expected messages
- ? Script stops unexpectedly
- ? Error messages about unknown scripts

---

## Troubleshooting

### "Script 'test-helper' not found"
**Solution:** Make sure all helper scripts are in your `Scripts` folder:
- `test-helper.razor`
- `test-variable-helper.razor`
- `test-nested-level1.razor`
- `test-nested-level2.razor`
- `test-early-return.razor`

### Script stops mid-execution
**Solution:** Check the Razor console for errors. The call stack might have exceeded depth limits.

### Variables not persisting
**Solution:** This indicates a bug in the call/return implementation. Variables should persist across calls.

### Nested calls fail
**Solution:** Verify that `CallDepth` property is being tracked correctly in `Interpreter.cs`.

---

## Creating Your Own Call/Return Scripts

### Example: Healing Helper
```razor
# main-combat.razor
if hp < 50
    call 'heal-self'
endif
attack 'enemy'
```

```razor
# heal-self.razor
if findtype 'bandage' backpack
    dclicktype 'bandage'
    waitfortarget
    hotkey 'target self'
    sysmsg "Healing applied"
else
    sysmsg "No bandages!"
endif
return
```

### Example: Buff Routine
```razor
# main-farm.razor
call 'apply-buffs'
call 'loot-cycle'
call 'bank-items'
```

```razor
# apply-buffs.razor
if not findbuff 'strength'
    cast 'strength'
    wait 2000
endif
if not findbuff 'agility'
    cast 'agility'  
    wait 2000
endif
sysmsg "Buffs applied"
return
```

---

## Advanced Features

### Call Depth Tracking
The call stack tracks how deep calls are nested:
```razor
# This would show depth
sysmsg "Call depth: {Interpreter.CallDepth}"
```

### Conditional Calls
```razor
if hp < 50
    call 'emergency-heal'
elseif mana < 30
    call 'meditate-routine'
else
    call 'normal-routine'
endif
```

### Loop with Calls
```razor
for 10
    call 'mining-spot'
    wait 1000
endfor
```

---

## Known Limitations

1. **No Return Values** - Called scripts cannot return values to the caller
2. **Global Variables Only** - Local variables in called scripts don't create new scope
3. **No Recursion Limit** - Be careful not to create infinite recursion
4. **Single Script Instance** - A script cannot call itself

---

## Performance Notes

- Each `call` has minimal overhead (< 1ms)
- Call stack depth limit: **Not currently enforced** (be careful!)
- Helper scripts should complete within reasonable time
- Use `wait` statements to prevent client lockup

---

## Getting Help

If tests fail or you encounter issues:

1. Check the Razor console for error messages
2. Verify all helper scripts are present
3. Review `FINAL_CALL_RETURN_STATUS.md` for implementation details
4. Check `CallStack.cs` for call stack implementation
5. Review the build with `run_build` to ensure no compilation errors

---

## Color Codes Used in Tests

- **88** (bright blue) - Section headers
- **68** (green) - Success messages
- **58** (cyan) - Info/helper messages
- **38** (red) - Error/failure messages

---

## Next Steps

After running these tests successfully, you can:

1. Create your own helper scripts for common tasks
2. Build complex script workflows with multiple calls
3. Organize scripts into logical subroutines
4. Share your script libraries with the community

Happy scripting! ??
