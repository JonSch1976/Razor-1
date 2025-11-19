# Syntax-Highlighted Script Preview Tooltip Implementation

## Overview
This implementation adds a custom rich tooltip that displays Razor script content with full syntax highlighting in the autocomplete menu, matching the color scheme shown in your reference image.

## Files Created

### 1. `FastColoredTextBox\ScriptPreviewTooltip.cs`
A custom tooltip form that hosts a `FastColoredTextBox` control to display syntax-highlighted script previews.

**Key Features:**
- Dark theme matching your editor (background: `#2B2B2B`, editor: `#1E1E1E`)
- Full Razor script syntax highlighting with colors:
  - Keywords (if, while, etc.): Orange `#FD861E`
  - Commands (attack, cast, etc.): Blue `#2B90AF`
  - Strings: Yellow/Green `#B5F109`
  - Numbers: Purple `#AE81FF`
  - Comments: Grey `#969696`
  - Serials (0x...): Red `#F9254D`
- Line numbers enabled
- Auto-hide after 5 seconds
- Non-interactive (read-only, no caret)
- Topmost window that doesn't steal focus

## Files Modified

### 2. `FastColoredTextBox\AutocompleteMenu.cs`

**Changes to `AutocompleteListView` class:**

1. **Added field:**
   ```csharp
   private ScriptPreviewTooltip scriptPreviewTooltip;
   ```

2. **Constructor updated:**
   - Initializes `scriptPreviewTooltip = new ScriptPreviewTooltip()`
   - Hides preview when menu visibility changes

3. **Dispose method updated:**
   - Properly disposes of `scriptPreviewTooltip`

4. **New method `HideScriptPreview()`:**
   - Safely hides the rich tooltip

5. **`SafetyClose()` updated:**
   - Calls `HideScriptPreview()` before closing menu

6. **`SetToolTip()` method updated:**
   - Detects script previews by checking if title starts with "Script:"
   - Uses rich tooltip for scripts (with syntax highlighting)
   - Uses standard tooltip for other items
   - Calculates proper positioning to the right of the autocomplete menu

## Integration with ScriptManager

The `ScriptManager.cs` already includes the `GetScriptPreview()` method that:
- Creates tooltip text showing first 20 lines of script
- Adds "(Empty script)" for empty scripts
- Shows "... (N more lines)" if script is longer than preview

When you create autocomplete items for scripts:
```csharp
foreach (var rs in _scriptList ?? Enumerable.Empty<RazorScript>())
{
    string snippet = $"call '{rs.Name}'";
    string scriptPreview = GetScriptPreview(rs.Lines, 20);
    list.Add(new AutocompleteItem(snippet, -1, snippet, 
        $"Script: {rs.Name}",  // Title starts with "Script:"
        scriptPreview));        // Full script content
}
```

## How It Works

1. **User types** in the script editor
2. **Autocomplete menu appears** with script suggestions
3. **User navigates** with arrow keys to a script item
4. **`SetToolTip()` is called** with the focused item
5. **System detects** this is a script (title starts with "Script:")
6. **Rich tooltip shows**:
   - Positioned to the right of autocomplete menu
   - Contains mini FastColoredTextBox with script content
   - Full Razor syntax highlighting applied
   - Line numbers visible
   - Dark theme matching main editor
7. **Auto-hides** after 5 seconds or when menu closes

## Color Scheme Breakdown

The syntax highlighting matches your reference image:

| Element | Color | RGB | Example |
|---------|-------|-----|---------|
| Keywords | Orange | `#FD861E` (253,134,30) | `if`, `while`, `return` |
| Commands | Blue | `#2B90AF` (43,144,175) | `attack`, `cast`, `overhead` |
| Strings | Yellow-Green | `#B5F109` (181,241,9) | `'text'`, `"text"` |
| Numbers | Purple | `#AE81FF` (174,129,255) | `100`, `3.14` |
| Comments | Grey | `#969696` (150,150,150) | `// comment`, `# comment` |
| Serials | Red | `#F9254D` (249,37,77) | `0x12AB` |
| Background | Dark Grey | `#1E1E1E` (30,30,30) | Editor background |
| Border | Dark Grey | `#2B2B2B` (43,43,43) | Tooltip border |

## Testing the Feature

1. Open a script in Razor
2. Start typing `call` 
3. Press Ctrl+Space or let autocomplete appear
4. Use arrow keys to navigate to a script item
5. The rich tooltip should appear to the right showing the full script with colors

## Benefits

? **Matches your design** - Dark theme with exact color scheme  
? **Full syntax highlighting** - All Razor script elements colored  
? **Better preview** - Shows actual script, not plain text  
? **Non-intrusive** - Doesn't steal focus, auto-hides  
? **Consistent** - Uses same FastColoredTextBox as main editor  
? **Performant** - Reuses single tooltip instance  

## Future Enhancements

Possible improvements:
- Add scroll support for longer scripts
- Make tooltip size configurable
- Add fade-in/fade-out animations
- Support clicking to insert script at cursor
- Show tooltip on hover (not just arrow key navigation)

## Build Status

? All files compile successfully  
? No errors in FastColoredTextBox project  
? No errors in Razor project  
? Ready to test!
