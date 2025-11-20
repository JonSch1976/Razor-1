#region license
// Razor: An Ultima Online Assistant
// Copyright (c)2022 Razor Development Community on GitHub <https://github.com/markdwags/Razor>
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Assistant.Gumps.Internal;
using Assistant.Macros;
using Assistant.Scripts.Engine;
using Assistant.UI;
using FastColoredTextBoxNS;
using System.Text.RegularExpressions;

namespace Assistant.Scripts
{
    public static class ScriptManager
    {
        public static bool Recording { get; set; }
        private static bool ScriptPaused { get; set; }
        public static bool Paused => ScriptPaused;
        private static bool ScriptRunning { get; set; }
        public static bool Running => ScriptRunning;
        public static DateTime LastWalk { get; set; }
        public static bool SetVariableActive { get; set; }
        public static bool TargetFound { get; set; }
        public static string ScriptPath => Config.GetUserDirectory("Scripts");

        private static FastColoredTextBox ScriptEditor { get; set; }
        private static TreeView ScriptTree { get; set; }
        private static ListBox ScriptVariableList { get; set; }
        private static Script _queuedScript;
        private static string _queuedScriptName;
        private static bool _isScriptCall = false; // call sequencing
        private static AutocompleteMenu _autoCompleteMenu;
        private static List<AutocompleteItem> _autoCompleteItems = new List<AutocompleteItem>();
        public static bool BlockPopupMenu { get; set; }
        private static bool EnableHighlight { get; set; }

        private static List<RazorScript> _scriptList { get; set; }
        private static string _activeScriptName; // currently executing script
        private static Stopwatch Stopwatch { get; } = new Stopwatch();

        // Allow user to opt out of syntax coloring (restores "original" look)
        private static bool UseOriginalColors => Config.GetBool("UseOriginalScriptColors");

        #region Highlight state
        public enum HighlightType { Error, Execution }
        private static readonly Dictionary<HighlightType, List<int>> HighlightLines = new Dictionary<HighlightType, List<int>>();
        private static readonly Dictionary<HighlightType, Brush> HighlightLineColors = new Dictionary<HighlightType, Brush>
        {
            { HighlightType.Error, new SolidBrush(Color.Red) },
            { HighlightType.Execution, new SolidBrush(Color.Blue) }
        };
        private static HighlightType[] GetHighlightTypes() => (HighlightType[])Enum.GetValues(typeof(HighlightType));
        #endregion

        #region Styles (used only if UseOriginalColors == false)
        // Razor dark theme styles
        private static readonly Style _keywordStyle = new TextStyle(new SolidBrush(Color.FromArgb(253, 134, 30)), null, FontStyle.Regular); // keywords orange
        private static readonly Style _commandStyle = new TextStyle(new SolidBrush(Color.FromArgb(43, 144, 175)), null, FontStyle.Regular); // commands blue
        private static readonly Style _stringStyle = new TextStyle(new SolidBrush(Color.FromArgb(181, 241, 9)), null, FontStyle.Regular); // strings yellow/green
        private static readonly Style _numberStyle = new TextStyle(new SolidBrush(Color.FromArgb(174, 129, 255)), null, FontStyle.Regular); // numbers purple
        private static readonly Style _commentStyle = new TextStyle(new SolidBrush(Color.FromArgb(150, 150, 150)), null, FontStyle.Italic); // comments grey italic
        private static readonly Style _serialStyle = new TextStyle(new SolidBrush(Color.FromArgb(249, 37, 77)), null, FontStyle.Regular); // 0x serials red
        private static readonly Style _layerStyle = new TextStyle(new SolidBrush(Color.FromArgb(174, 129, 255)), null, FontStyle.Regular); // layer/reserved words (reuse number color)
        private static readonly Style _expressionStyle = new TextStyle(new SolidBrush(Color.FromArgb(249, 37, 77)), null, FontStyle.Regular); // expressions red (same as serial)
        private static readonly Style _sysmsgStyle = new TextStyle(new SolidBrush(Color.Red), null, FontStyle.Regular); // sysmsg red (legacy)
        private static readonly Style _overheadStyle = new TextStyle(new SolidBrush(Color.Blue), null, FontStyle.Regular); // overhead blue (legacy)
        #endregion

        // Regex sets for highlighting (initialized once)
        private static Regex _regexStrings1;
        private static Regex _regexStrings2;
        private static Regex _regexComments;
        private static Regex _regexNumbers;
        private static Regex _regexKeywords;
        private static Regex _regexCommands;
        private static Regex _regexSerials;
        private static Regex _regexLayers;
        private static Regex _regexExpressions;

        private static void InitHighlightRegexes()
        {
            if (_regexStrings1 != null) return; // already initialized
            var ro = RegexOptions.Compiled | RegexOptions.Multiline;
            _regexStrings1 = new Regex("([\"'])(?:(?=(\\\\?))\\2.)*?\\1", ro);
            _regexStrings2 = new Regex("'[^'\\\\]*(\\\\.[^'\\\\]*)*'", ro);
            _regexComments = new Regex("(//.*$|#.*$)", ro);
            _regexSerials = new Regex(@"0x[\da-fA-F]*", ro);
            _regexNumbers = new Regex(@"\b[+-]?[0-9]+(?:\.[0-9]+)?(?:[eE][+-]?[0-9]+)?\b", ro);
            _regexKeywords = new Regex(@"\b(if|elseif|else|endif|while|endwhile|for|foreach|endfor|break|continue|not|and|or|stop|replay|loop|as|in|return)\b", ro);
            _regexCommands = new Regex(@"\b(attack|cast|dress|undress|dressconfig|target|targettype|targetloc|targetrelloc|drop|waitfortarget|wft|dclick|dclicktype|dclickvar|usetype|useobject|droprelloc|lift|lifttype|waitforgump|gumpresponse|gumpclose|menu|menuresponse|waitformenu|promptresponse|waitforprompt|hotkey|say|msg|overhead|sysmsg|wait|pause|waitforstat|setability|setlasttarget|lasttarget|setvar|unsetvar|skill|useskill|walk|script|useonce|organizer|organize|org|restock|scav|scavenger|potion|clearsysmsg|clearjournal|whisper|yell|guild|alliance|emote|waitforsysmsg|wfsysmsg|clearall|virtue|interrupt|sound|music|classicuo|cuo|rename|getlabel|ignore|unignore|clearignore|cooldown|settimer|removetimer|createtimer|poplist|pushlist|removelist|createlist|clearlist|cleardragdrop|clearhands|sell)\b", ro);
            _regexLayers = new Regex(@"\b(RightHand|LeftHand|Shoes|Pants|Shirt|Head|Gloves|Ring|Talisman|Neck|Hair|Waist|InnerTorso|Bracelet|FacialHair|MiddleTorso|Earrings|Arms|Cloak|Backpack|OuterTorso|OuterLegs|InnerLegs|backpack|true|false|criminal|enemy|friend|friendly|grey|gray|innocent|murderer|red|blue|nonfriendly|cancel|clear|minutes|minute|min|seconds|second|sec)\b", ro);
            _regexExpressions = new Regex(@"\b(queued|position|insysmsg|insysmessage|findtype|findbuff|finddebuff|stam|maxstam|hp|maxhp|maxhits|hits|mana|maxmana|str|dex|int|poisoned|hidden|mounted|rhandempty|lhandempty|skill|count|counter|weight|dead|closest|close|rand|random|next|prev|previous|human|humanoid|monster|varexist|followers|maxfollowers|maxweight|targetexists|diffmana|diffstam|diffhits|diffhp|diffweight|blessed|invul|invuln|warmode|name|paralyzed|itemcount|poplist|atlist|listexists|list|inlist|timer|timerexists)\b", ro);
        }

        #region Timer
        private class ScriptTimer : Timer
        {
            public ScriptTimer(int delay = 25) : base(TimeSpan.FromMilliseconds(delay), TimeSpan.FromMilliseconds(delay)) { }
            protected override void OnTick()
            {
                try
                {
                    if (!Client.Instance.ClientRunning)
                    {
                        if (ScriptRunning)
                        {
                            ScriptRunning = false;
                            Interpreter.StopScript();
                        }
                        return;
                    }
                    bool running;
                    if (_queuedScript != null)
                    {
                        if (_isScriptCall) _isScriptCall = false;
                        Script script = _queuedScript;
                        _activeScriptName = _queuedScriptName;
                        running = Interpreter.StartScript(script);
                        UpdateLineNumber(Interpreter.CurrentLine);
                        _queuedScript = null;
                    }
                    else
                    {
                        running = Interpreter.ExecuteScript();
                        if (running) UpdateLineNumber(Interpreter.CurrentLine);
                    }
                    if (running)
                    {
                        if (!ScriptRunning)
                        {
                            if (!Config.GetBool("ScriptDisablePlayFinish"))
                            {
                                if (!Config.GetBool("DisableScriptStopwatch")) Stopwatch.Start();
                                World.Player?.SendMessage(LocString.ScriptPlaying, _activeScriptName);
                            }
                            Assistant.Engine.MainWindow.LockScriptUI(true);
                            ScriptRunning = true;
                        }
                    }
                    else
                    {
                        if (ScriptRunning)
                        {
                            if (Interpreter.HasCalls)
                            {
                                Script callingScript = Interpreter.PopCall();
                                if (callingScript != null)
                                {
                                    _queuedScript = callingScript;
                                    _queuedScriptName = "Returning from call";
                                    return;
                                }
                            }
                            if (!Config.GetBool("ScriptDisablePlayFinish"))
                            {
                                if (!Config.GetBool("DisableScriptStopwatch"))
                                {
                                    Stopwatch.Stop();
                                    TimeSpan elapsed = Stopwatch.Elapsed;
                                    Stopwatch.Reset();
                                    World.Player?.SendMessage(LocString.ScriptFinishedStopwatch, _activeScriptName, elapsed.TotalMilliseconds);
                                }
                                else
                                    World.Player?.SendMessage(LocString.ScriptFinished, _activeScriptName);
                            }
                            Assistant.Engine.MainWindow.LockScriptUI(false);
                            ScriptRunning = false;
                            _activeScriptName = null;
                            ClearHighlightLine(HighlightType.Execution);
                        }
                    }
                }
                catch (Exception ex)
                {
                    World.Player?.SendMessage(MsgLevel.Error, $"Script Error: {ex.Message} (Line: {Interpreter.CurrentLine + 1})");
                    if (EnableHighlight) SetHighlightLine(Interpreter.CurrentLine, HighlightType.Error);
                    StopScript();
                    _activeScriptName = null;
                }
            }
        }
        private static ScriptTimer Timer { get; set; } = new ScriptTimer(Config.GetBool("DefaultScriptDelay") ? 25 : 0);
        public static void ResetTimer()
        {
            Timer.Stop();
            Timer = new ScriptTimer(Config.GetBool("DefaultScriptDelay") ? 25 : 0);
            Timer.Start();
        }
        #endregion

        #region Initialization
        /// <summary>
 /// This is called via reflection when the application starts up
 /// </summary>
 public static void Initialize()
 {
 HotKey.Add(HKCategory.Scripts, HKSubCat.None, LocString.StopScript, HotkeyStopScript);
 HotKey.Add(HKCategory.Scripts, HKSubCat.None, LocString.PauseScript, HotkeyPauseScript);
 HotKey.Add(HKCategory.Scripts, HKSubCat.None, LocString.ScriptDClickType, HotkeyDClickTypeScript);
 HotKey.Add(HKCategory.Scripts, HKSubCat.None, LocString.ScriptTargetType, HotkeyTargetTypeScript);


 _scriptList = new List<RazorScript>();

 // Load user scripts
 Recurse(null, Config.GetUserDirectory("Scripts"));

 // Also load optional CUO scripts tree if present (e.g., UORenaissance\Razor\CUO\Scripts)
 try
 {
 var cuoScriptsPath = System.IO.Path.Combine(Assistant.Engine.RootPath, "UORenaissance", "Razor", "CUO", "Scripts");
 if (System.IO.Directory.Exists(cuoScriptsPath))
 {
 Recurse(null, cuoScriptsPath);
 }
 }
 catch
 {
 // ignore optional path failures
 }
 
 foreach (var type in GetHighlightTypes()) HighlightLines[type] = new List<int>();

 Lexer.AllowLoop = Client.Instance.AllowBit(FeatureBit.LoopingMacros);
 }

        #endregion

        #region Hotkeys handlers (unchanged)
        private static void HotkeyStopScript() => StopScript();
        private static void HotkeyPauseScript()
        {
            if (!ScriptRunning) return;
            if (ScriptPaused)
            {
                ResumeScript();
                World.Player.SendMessage(MsgLevel.Force, Language.Format(LocString.ResumeScriptMessage, Interpreter.CurrentLine), false);
            }
            else
            {
                World.Player.SendMessage(MsgLevel.Force, Language.Format(LocString.PauseScriptMessage, Interpreter.CurrentLine), false);
                PauseScript();
            }
        }
        private static void HotkeyTargetTypeScript()
        {
            if (World.Player == null) return;
            World.Player.SendMessage(MsgLevel.Force, LocString.ScriptTargetType);
            Targeting.OneTimeTarget(OnTargetTypeScript);
        }
        private static void HotkeyDClickTypeScript()
        {
            if (World.Player == null) return;
            World.Player.SendMessage(MsgLevel.Force, LocString.ScriptTargetType);
            Targeting.OneTimeTarget(OnDClickTypeScript);
        }
        private static void OnTargetTypeScript(bool loc, Serial serial, Point3D pt, ushort itemId)
        {
            Item item = World.FindItem(serial);
            if (item != null && item.Serial.IsItem && item.Movable && item.Visible)
            {
                string cmd = $"targettype '{item.ItemID.ItemData.Name}'";
                Clipboard.SetDataObject(cmd);
                World.Player.SendMessage(MsgLevel.Force, Language.Format(LocString.ScriptCopied, cmd), false);
            }
            else
            {
                Mobile m = World.FindMobile(serial);
                if (m != null)
                {
                    string cmd = $"targettype '{m.Body}'";
                    Clipboard.SetDataObject(cmd);
                    World.Player.SendMessage(MsgLevel.Force, Language.Format(LocString.ScriptCopied, cmd), false);
                }
            }
        }
        private static void OnDClickTypeScript(bool loc, Serial serial, Point3D pt, ushort itemId)
        {
            Item item = World.FindItem(serial);
            if (item != null && item.Serial.IsItem && item.Movable && item.Visible)
            {
                string cmd = $"dclicktype '{item.ItemID.ItemData.Name}'";
                Clipboard.SetDataObject(cmd);
                World.Player.SendMessage(MsgLevel.Force, Language.Format(LocString.ScriptCopied, cmd), false);
            }
            else
            {
                Mobile m = World.FindMobile(serial);
                if (m != null)
                {
                    string cmd = $"dclicktype '{m.Body}'";
                    Clipboard.SetDataObject(cmd);
                    World.Player.SendMessage(MsgLevel.Force, Language.Format(LocString.ScriptCopied, cmd), false);
                }
            }
        }
        #endregion

        #region Script control
        public static void StopScript()
        {
            _queuedScript = null;
            ScriptPaused = false;
            Interpreter.StopScript();
        }
        public static void PauseScript() { ScriptPaused = true; Interpreter.PauseScript(); }
        public static void ResumeScript() { ScriptPaused = false; Interpreter.ResumeScript(); }
        #endregion

        #region Autocomplete
        private static void BuildAutoCompleteItems()
        {
            if (_autoCompleteMenu == null) return;
            var list = new List<AutocompleteItem>
            {
                new AutocompleteItem("call", -1, "call", "call", "Usage: call 'script' – run sub-script."),
                new AutocompleteItem("return", -1, "return", "return", "Return from called script")
            };
            foreach (var rs in _scriptList ?? Enumerable.Empty<RazorScript>())
            {
                string snippet = $"call '{rs.Name}'";
                // Create tooltip showing the actual script content (first 20 lines)
                string scriptPreview = GetScriptPreview(rs.Lines, 20);
                list.Add(new AutocompleteItem(snippet, -1, snippet, $"Script: {rs.Name}", scriptPreview));
            }
            _autoCompleteItems = list;
            _autoCompleteMenu.Items.SetAutocompleteItems(list);
        }
        
        /// <summary>
        /// Get a preview of the script (first N lines) for tooltip display
        /// </summary>
        private static string GetScriptPreview(string[] lines, int maxLines)
        {
            if (lines == null || lines.Length == 0)
                return "(Empty script)";
            
            int previewLines = Math.Min(lines.Length, maxLines);
            var preview = new System.Text.StringBuilder();
            
            for (int i = 0; i < previewLines; i++)
            {
                preview.AppendLine(lines[i]);
            }
            
            if (lines.Length > maxLines)
                preview.AppendLine($"... ({lines.Length - maxLines} more lines)");
            
            return preview.ToString();
        }
        
        private static void RefreshAutoCompleteScripts() => BuildAutoCompleteItems();
        #endregion

        #region Editor init & highlighting
        private static void ApplyFullSyntaxHighlight()
        {
            if (ScriptEditor == null) return;
            if (!UseOriginalColors)
            {
                InitHighlightRegexes();
                var range = ScriptEditor.Range;
                range.ClearStyle(_keywordStyle, _commandStyle, _numberStyle, _stringStyle, _commentStyle, _serialStyle, _layerStyle, _expressionStyle, _sysmsgStyle, _overheadStyle);
                range.SetStyle(_stringStyle, _regexStrings1);
                range.SetStyle(_stringStyle, _regexStrings2);
                range.SetStyle(_commentStyle, _regexComments);
                range.SetStyle(_serialStyle, _regexSerials);
                range.SetStyle(_numberStyle, _regexNumbers);
                range.SetStyle(_keywordStyle, _regexKeywords);
                range.SetStyle(_commandStyle, _regexCommands);
                range.SetStyle(_layerStyle, _regexLayers);
                range.SetStyle(_expressionStyle, _regexExpressions);
                // legacy sysmsg / overhead start-of-line emphasis (keep bold colors if desired)
                range.SetStyle(_sysmsgStyle, @"(?m)^\s*sysmsg\b");
                range.SetStyle(_overheadStyle, @"(?m)^\s*overhead\b");
            }

            // Always try the built-in SyntaxHighlighter path as a fallback (ensures colors even if UseOriginalColors is true)
            try
            {
                if (ScriptEditor.SyntaxHighlighter == null)
                {
                    ScriptEditor.SyntaxHighlighter = new SyntaxHighlighter(ScriptEditor);
                    ScriptEditor.SyntaxHighlighter.InitStyleSchema(FastColoredTextBoxNS.Language.Razor);
                }
                ScriptEditor.SyntaxHighlighter.RazorSyntaxHighlight(ScriptEditor.Range);
            }
            catch
            {
                // Do not crash the UI if the highlighter fails
            }
        }
        public static void InitScriptEditor()
        {
            if (ScriptEditor == null) return;

            // Ensure the editor is configured for Razor highlighting
            try
            {
                ScriptEditor.Language = FastColoredTextBoxNS.Language.Razor;
                if (ScriptEditor.SyntaxHighlighter == null)
                {
                    ScriptEditor.SyntaxHighlighter = new SyntaxHighlighter(ScriptEditor);
                }
                ScriptEditor.SyntaxHighlighter.InitStyleSchema(FastColoredTextBoxNS.Language.Razor);
            }
            catch { }

            _autoCompleteMenu = new AutocompleteMenu(ScriptEditor)
            {
                SearchPattern = @"[\w\.:=!<>]",
                AllowTabKey = true,
                ToolTipDuration = 5000,
                AppearInterval = 100
            };
            BuildAutoCompleteItems();
            ScriptEditor.TextChanged -= ScriptEditorOnTextChanged;
            ScriptEditor.TextChanged += ScriptEditorOnTextChanged;
            ApplyFullSyntaxHighlight();
        }
        private static void ScriptEditorOnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!UseOriginalColors)
            {
                InitHighlightRegexes();
                e.ChangedRange.ClearStyle(_keywordStyle, _commandStyle, _numberStyle, _stringStyle, _commentStyle, _serialStyle, _layerStyle, _expressionStyle, _sysmsgStyle, _overheadStyle);
                e.ChangedRange.SetStyle(_stringStyle, _regexStrings1);
                e.ChangedRange.SetStyle(_stringStyle, _regexStrings2);
                e.ChangedRange.SetStyle(_commentStyle, _regexComments);
                e.ChangedRange.SetStyle(_serialStyle, _regexSerials);
                e.ChangedRange.SetStyle(_numberStyle, _regexNumbers);
                e.ChangedRange.SetStyle(_keywordStyle, _regexKeywords);
                e.ChangedRange.SetStyle(_commandStyle, _regexCommands);
                e.ChangedRange.SetStyle(_layerStyle, _regexLayers);
                e.ChangedRange.SetStyle(_expressionStyle, _regexExpressions);
                e.ChangedRange.SetStyle(_sysmsgStyle, @"(?m)^\s*sysmsg\b");
                e.ChangedRange.SetStyle(_overheadStyle, @"(?m)^\s*overhead\b");
            }

            // Fallback to built-in highlighter
            try
            {
                ScriptEditor.SyntaxHighlighter?.RazorSyntaxHighlight(e.ChangedRange);
            }
            catch { }
        }
        #endregion

        #region Script load/save
        public static RazorScript AddScript(string file)
        {
            var script = new RazorScript
            {
                Lines = File.ReadAllLines(file),
                Name = Path.GetFileNameWithoutExtension(file),
                Path = file,
                Category = Path.GetDirectoryName(file).Equals(Config.GetUserDirectory("Scripts")) ? string.Empty : Path.GetDirectoryName(file.Replace(Config.GetUserDirectory("Scripts"), "").TrimStart(Path.DirectorySeparatorChar)).Replace("/", "\\")
            };
            AddHotkey(script);
            _scriptList.Add(script);
            RefreshAutoCompleteScripts();
            return script;
        }
        public static void RemoveScript(RazorScript script)
        {
            RemoveHotkey(script);
            _scriptList.Remove(script);
            RefreshAutoCompleteScripts();
        }
        public static void RedrawScripts()
        {
            ScriptTree.SafeAction(s =>
            {
                s.BeginUpdate();
                s.Nodes.Clear();
                Recurse(s.Nodes, Config.GetUserDirectory("Scripts"));
                s.EndUpdate();
                s.Refresh();
            });
            RedrawScriptVariables();
            RefreshAutoCompleteScripts();
        }
        private static void Recurse(TreeNodeCollection nodes, string path)
        {
            try
            {
                foreach (var file in Directory.GetFiles(path, "*.razor").OrderBy(f => f))
                {
                    RazorScript script = _scriptList.FirstOrDefault(r => r.Path.Equals(file)) ?? AddScript(file);
                    if (nodes != null) nodes.Add(new TreeNode(script.Name) { Tag = script });
                }
            }
            catch { }
            try
            {
                foreach (var dir in Directory.GetDirectories(path))
                {
                    if (!string.IsNullOrEmpty(dir) && !dir.EndsWith(".") && !dir.EndsWith(".."))
                    {
                        if (nodes != null)
                        {
                            TreeNode d = new TreeNode($"[{Path.GetFileName(dir)}]") { Tag = dir };
                            nodes.Add(d);
                            Recurse(d.Nodes, dir);
                        }
                        else Recurse(null, dir);
                    }
                }
            }
            catch { }
        }
        public static TreeNode GetScriptDirNode()
        {
            if (ScriptTree?.SelectedNode == null) return null;
            if (ScriptTree.SelectedNode.Tag is string) return ScriptTree.SelectedNode;
            if (!(ScriptTree.SelectedNode.Parent?.Tag is string)) return null;
            return ScriptTree.SelectedNode.Parent;
        }
        public static void AddScriptNode(TreeNode node)
        {
            if (ScriptTree == null || node == null) return;
            if (node.Tag is string) ScriptTree.Nodes.Add(node); else ScriptTree.SelectedNode?.Nodes.Add(node);
            ScriptTree.SelectedNode = node;
        }
        #endregion

        #region Play / Call
        public static void PlayScript(string scriptName)
        {
            foreach (var rs in _scriptList)
                if (rs.ToString().IndexOf(scriptName, StringComparison.OrdinalIgnoreCase) != -1)
                { PlayScript(rs.Lines, scriptName); break; }
        }
        public static void PlayScript(string[] lines, string name)
        {
            if (World.Player == null || lines == null) return;
            if (MacroManager.Playing || MacroManager.StepThrough) MacroManager.Stop();
            StopScript(); EnableHighlight = false; SetVariableActive = false;
            if (_queuedScript != null || !Client.Instance.ClientRunning) return;
            try
            {
                Script script = new Script(Lexer.Lex(lines));
                _queuedScript = script; _queuedScriptName = name;
            }
            catch (SyntaxError se)
            { World.Player.SendMessage(MsgLevel.Error, $"{se.Message}: '{se.Line}' (Line #{se.LineNumber + 1})"); }
        }
        public static void PlayScriptFromUI(string[] lines, string name, bool highlight)
        {
            if (World.Player == null || ScriptEditor == null || lines == null) return;
            EnableHighlight = highlight; if (EnableHighlight) ClearAllHighlightLines();
            if (MacroManager.Playing || MacroManager.StepThrough) MacroManager.Stop();
            StopScript(); SetVariableActive = false;
            if (_queuedScript != null || !Client.Instance.ClientRunning) return;
            try
            {
                Script script = new Script(Lexer.Lex(lines));
                _queuedScript = script; _queuedScriptName = name;
            }
            catch (SyntaxError se)
            {
                World.Player.SendMessage(MsgLevel.Error, $"{se.Message}: '{se.Line}' (Line #{se.LineNumber + 1})");
                if (EnableHighlight) SetHighlightLine(se.LineNumber, HighlightType.Error);
            }
        }
        public static void CallScript(string[] lines, string name)
        {
            CallScript(lines, name, null);
        }
        public static void CallScript(string[] lines, string name, List<string> parameters)
        {
            if (World.Player == null || lines == null || !Client.Instance.ClientRunning) return;
            try
            {
                // Guard against exceeding max depth
                if (Interpreter.CallDepth >= ScriptCallStack.MaxDepth)
                {
                    World.Player?.SendMessage(MsgLevel.Error, $"Max call depth {ScriptCallStack.MaxDepth} reached. Call to '{name}' canceled.");
                    return;
                }
                Script newScript = new Script(Lexer.Lex(lines));
                if (_activeScriptName != null && !_isScriptCall)
                {
                    Interpreter.PushCall(Interpreter.GetActiveScript(), _activeScriptName);
                    Interpreter.SuspendScript();
                }
                _isScriptCall = true;
                _queuedScript = newScript;
                _queuedScriptName = name;
                // Store parameters globally as arg0,arg1,... and argc
                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Count; i++)
                        Interpreter.SetVariable($"arg{i}", parameters[i], true);
                    Interpreter.SetVariable("argc", parameters.Count.ToString(), true);
                }
                else
                {
                    if (Interpreter.ExistAlias("argc")) Interpreter.ClearAlias("argc");
                }
            }
            catch (SyntaxError se)
            {
                World.Player.SendMessage(MsgLevel.Error, $"{se.Message}: '{se.Line}' (Line #{se.LineNumber + 1})");
                _isScriptCall = false;
            }
        }
        public static void ReturnFromCall()
        {
            if (Interpreter.HasCalls)
            {
                Script callingScript = Interpreter.PopCall();
                if (callingScript != null)
                { _queuedScript = callingScript; _queuedScriptName = "Returning from call"; ScriptRunning = true; }
            }
            else StopScript();
        }
        #endregion

        #region Highlight helpers
        private static void AddHighlightLine(int iline, HighlightType type) { HighlightLines[type].Add(iline); RefreshHighlightLines(); }
        private static void SetHighlightLine(int iline, HighlightType type) { ClearHighlightLine(type); AddHighlightLine(iline, type); }
        public static void ClearHighlightLine(HighlightType type) { HighlightLines[type].Clear(); RefreshHighlightLines(); }
        public static void ClearAllHighlightLines() { foreach (var t in GetHighlightTypes()) HighlightLines[t].Clear(); RefreshHighlightLines(); }
        private static void RefreshHighlightLines()
        {
            if (ScriptEditor == null) return;
            for (int i = 0; i < ScriptEditor.LinesCount; i++) ScriptEditor[i].BackgroundBrush = ScriptEditor.BackBrush;
            foreach (var kv in HighlightLines) foreach (int line in kv.Value) ScriptEditor[line].BackgroundBrush = HighlightLineColors[kv.Key];
            ScriptEditor.Invalidate();
        }
        private static void UpdateLineNumber(int lineNum)
        {
            if (!EnableHighlight) return;
            SetHighlightLine(lineNum, HighlightType.Execution);
            ScriptEditor.Selection.Start = new Place(0, lineNum);
            ScriptEditor.DoSelectionVisible();
        }
        #endregion

        #region Variables / UI
        public static void RedrawScriptVariables()
        {
            ScriptVariableList?.SafeAction(s =>
            {
                s.BeginUpdate(); s.Items.Clear();
                foreach (var kv in ScriptVariables.Variables) s.Items.Add($"{kv.Key} ({kv.Value})");
                s.EndUpdate();
            });
        }
        public static void SetEditor(FastColoredTextBox editor)
        {
            ScriptEditor = editor; InitScriptEditor(); if (SelectedScript != null) SetEditorText(SelectedScript);
        }
        public static RazorScript SelectedScript { get; set; }
        public static void SetEditorText(RazorScript selected)
        {
            SelectedScript = selected;
            ScriptEditor.Text = string.Join("\n", SelectedScript.Lines);
            ApplyFullSyntaxHighlight();
        }
        public static void SetControls(FastColoredTextBox editor, TreeView tree, ListBox vars)
        {
            ScriptEditor = editor; ScriptTree = tree; ScriptVariableList = vars; 
            InitScriptEditor();
        }
        public static void OnLogin()
        {
            Commands.Register(); AgentCommands.Register(); SpeechCommands.Register(); TargetCommands.Register();
            Aliases.Register(); Expressions.Register(); Timer.Start();
        }
        public static void OnLogout()
        {
            StopScript(); Timer.Stop(); Assistant.Engine.MainWindow.LockScriptUI(false);
        }
        public static void StartEngine() => Timer.Start();
        public static string GetCurrentScriptName() => _activeScriptName ?? _queuedScriptName;
        #endregion

        #region Misc helpers
        private static void AddHotkey(RazorScript script) => HotKey.Add(HKCategory.Scripts, HKSubCat.None, Language.Format(LocString.PlayScript, script), OnHotKey, script);
        private static void RemoveHotkey(RazorScript script) => HotKey.Remove(Language.Format(LocString.PlayScript, script.ToString()));
        public static void OnHotKey(ref object state) { var script = (RazorScript)state; PlayScript(script.Lines, script.Name); }
        public static bool AddToScript(string command)
        {
            if (!Recording) return false;
            ScriptEditor?.AppendText(command + Environment.NewLine); return true;
        }
        public static void Error(bool quiet, string statement, string message, bool throwError = false)
        { if (!quiet) World.Player?.SendMessage(MsgLevel.Error, $"{statement}: {message}"); }
        public static List<ASTNode> ParseArguments(ref ASTNode node)
        { var list = new List<ASTNode>(); while (node != null) { list.Add(node); node = node.Next(); } return list; }
        public static void GetGumpInfo(string[] param)
        {
            Targeting.OneTimeTarget(OnGetItemInfoTarget);
            Client.Instance.SendToClient(new UnicodeMessage(0xFFFFFFFF, -1, MessageType.Regular, 0x3B2, 3,
                Language.CliLocName, "System", "Select an item or mobile to view/inspect"));
        }
        private static void OnGetItemInfoTarget(bool ground, Serial serial, Point3D pt, ushort gfx)
        {
            Item item = World.FindItem(serial);
            if (item == null)
            {
                Mobile mobile = World.FindMobile(serial); if (mobile == null) return; new ItemInfoGump(item).SendGump();
            }
            else new ItemInfoGump(item).SendGump();
        }
        #endregion

        // Provide public accessor for all scripts (required by Commands.cs)
        public static List<RazorScript> GetAllScripts()
        {
            return _scriptList ?? new List<RazorScript>();
        }
    }
}