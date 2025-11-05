# ? Call/Return Test Suite - COMPLETE

## ?? What Was Created

### Test Scripts (6 files):
1. **test-call-return.razor** - Main comprehensive test suite
2. **test-helper.razor** - Simple helper script
3. **test-variable-helper.razor** - Variable persistence test
4. **test-nested-level1.razor** - First nesting level
5. **test-nested-level2.razor** - Second nesting level
6. **test-early-return.razor** - Early return test

### Documentation (3 files):
1. **TEST_GUIDE.md** - Complete usage guide (1,400+ lines)
2. **QUICK_TEST_REFERENCE.md** - Quick reference card
3. **TEST_SUITE_SUMMARY.md** - This file

---

## ?? Purpose

These scripts test the new `call` and `return` functionality by:
- Calling helper scripts and returning properly
- Verifying variable scope across calls
- Testing nested calls (2 levels deep)
- Validating explicit `return` statement
- Running multiple sequential calls

---

## ?? How to Run

### Simple Method:
```
1. Open Razor
2. Click Scripts tab
3. Find "test-call-return"
4. Click Play button
```

### In-Game Method:
```
>script test-call-return
```

---

## ?? Expected Output

You'll see messages in your **journal** (system messages):

```
=== Starting Call/Return Test ===       [Blue]
Test 1: Simple call to helper script    [Green]
  -> Inside helper script     [Cyan]
  -> Helper completing    [Cyan]
Test 1: PASSED - Simple call completed   [Green]

Test 2: Testing variable persistence     [Green]
    -> Variable found in helper!    [Cyan]
Test 2: PASSED - Variable persisted      [Green]

Test 3: Testing nested calls   [Green]
  -> Level 1: Entered     [Cyan]
  -> Level 2: Entered           [Cyan]
  -> Level 2: Completing         [Cyan]
  -> Level 1: Returned from Level 2      [Cyan]
Test 3: PASSED - Nested calls completed  [Green]

Test 4: Testing early return   [Green]
  -> Early return script started[Cyan]
  -> Returning early (hp > 0) [Cyan]
Test 4: PASSED - Early return worked     [Green]

Test 5: Testing multiple sequential calls [Green]
  -> Inside helper script       [Cyan]
  -> Helper completing    [Cyan]
  -> Inside helper script                [Cyan]
  -> Helper completing               [Cyan]
  -> Inside helper script         [Cyan]
  -> Helper completing     [Cyan]
Test 5: PASSED - Sequential calls worked [Green]

=== All Call/Return Tests Complete ===   [Blue]
```

Plus **overhead messages** above your character showing progress!

---

## ? Success Criteria

All 5 tests should show **"PASSED"** in green:
- ? Test 1: Simple call/return
- ? Test 2: Variable persistence
- ? Test 3: Nested calls
- ? Test 4: Early return
- ? Test 5: Sequential calls

---

## ?? What Gets Tested

### Test 1: Basic Functionality
- Can call another script
- Execution returns to caller
- Continues after call statement

### Test 2: State Management
- Variables persist across calls
- Global scope maintained
- `varexist` works correctly

### Test 3: Call Stack
- Supports nested calls
- Proper return order (LIFO)
- Multiple stack frames handled

### Test 4: Control Flow
- `return` stops script immediately
- Code after return doesn't execute
- Returns to caller at correct position

### Test 5: Sequential Execution
- Multiple calls in sequence
- Each completes before next starts
- No state corruption

---

## ?? Message Types

The tests use different message types:

### sysmsg (System Messages to Journal):
```razor
sysmsg "Test message" 68  # Green success
sysmsg "Error message" 38 # Red error
sysmsg "Info message" 58  # Cyan info
sysmsg "Header" 88        # Blue header
```

### overhead (Messages Above Character):
```razor
overhead "Calling helper..."
overhead "Returned from helper!"
```

Both types appear in:
- System message area (lower left)
- Journal window (if open)
- Razor log file

---

## ??? Troubleshooting

### Problem: "Script not found" errors
**Solution:** Ensure all 6 test scripts are in `Razor/Scripts/` folder

### Problem: Tests don't complete
**Solution:** Check Razor console for errors; verify call stack implementation

### Problem: Variables lost across calls
**Solution:** Bug in implementation; variables should persist globally

### Problem: Nested calls fail
**Solution:** Call stack might have depth issues; check `CallStack.cs`

---

## ?? File Locations

All files created in: `Razor/Scripts/`

```
Razor/
??? Scripts/
    ??? test-call-return.razor
    ??? test-helper.razor
    ??? test-variable-helper.razor
    ??? test-nested-level1.razor
    ??? test-nested-level2.razor
    ??? test-early-return.razor
    ??? TEST_GUIDE.md
    ??? QUICK_TEST_REFERENCE.md
    ??? TEST_SUITE_SUMMARY.md (this file)
```

---

## ?? Learning Examples

After running tests, use these as templates:

### Example 1: Combat Helper
```razor
# main-combat.razor
if hp < 50
    call 'heal-self'
endif
attack 'enemy'
```

### Example 2: Buff Routine
```razor
# main-buff.razor
call 'check-buffs'
call 'apply-missing-buffs'
sysmsg "Buffs complete!"
```

### Example 3: Conditional Routing
```razor
# main-logic.razor
if gold < 1000
    call 'farm-gold'
elseif bags full
    call 'bank-items'
else
    call 'continue-quest'
endif
```

---

## ?? Related Documentation

- **Implementation Details:** `FINAL_CALL_RETURN_STATUS.md`
- **Call Stack Code:** `Razor/Scripts/Engine/CallStack.cs`
- **Command Handlers:** `Razor/Scripts/Commands.cs`
- **Keywords Reference:** `help/docs/guide/keywords.md`

---

## ?? Next Steps

1. **Run the tests** to verify implementation
2. **Check journal** for PASSED messages
3. **Create your own** helper scripts
4. **Build workflows** with multiple calls
5. **Share with community** if useful!

---

## ?? Pro Tips

- Use `clearsysmsg` before running tests for clean output
- Enable journal window to see all messages easily
- Color codes help identify message types quickly
- Test scripts are safe to run repeatedly
- Modify them to test your specific scenarios

---

## ? Status

**Implementation:** ? Complete  
**Build Status:** ? Success  
**Test Scripts:** ? Created  
**Documentation:** ? Complete  
**Ready to Test:** ? YES

---

**Happy Testing! ??**

The call/return feature is now fully implemented and ready for testing.
Run `test-call-return` to validate everything works as expected!
