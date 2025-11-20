#region license
// Razor: An Ultima Online Assistant
// Copyright (c) 2022 Razor Development Community on GitHub <https://github.com/markdwags/Razor>
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System.Collections.Generic;
using Assistant.Core.Logging;

namespace Assistant.Scripts.Engine
{
    /// <summary>
    /// Manages script call stack for subroutine calls
    /// </summary>
    public class ScriptCallStack
    {
        public const int MaxDepth = 32; // maximum allowed nested calls
        private readonly Stack<ScriptCallFrame> _callStack = new Stack<ScriptCallFrame>();

        /// <summary>
        /// Push a new script call onto the stack
        /// </summary>
        public void Push(Script callingScript, string scriptName)
        {
            if (_callStack.Count >= MaxDepth)
            {
                Logger.Warning($"[ScriptCallStack] Max depth {MaxDepth} reached. Ignoring push for '{scriptName}'");
                return;
            }
            var frame = new ScriptCallFrame
            {
                Script = callingScript,
                ScriptName = scriptName,
                Line = callingScript.CurrentLine
            };

            _callStack.Push(frame);

            Logger.Debug($"[ScriptCallStack] Pushed '{scriptName}' onto call stack (depth: {_callStack.Count})");
        }

        /// <summary>
        /// Pop the most recent call from the stack
        /// </summary>
        public ScriptCallFrame Pop()
        {
            if (_callStack.Count == 0)
            {
                Logger.Warning("[ScriptCallStack] Attempted to pop empty call stack");
                return null;
            }

            var frame = _callStack.Pop();
            Logger.Debug($"[ScriptCallStack] Popped '{frame.ScriptName}' from call stack (depth: {_callStack.Count})");

            return frame;
        }

        /// <summary>
        /// Check if there are any calls on the stack
        /// </summary>
        public bool HasCalls => _callStack.Count > 0;

        /// <summary>
        /// Get the current call depth
        /// </summary>
        public int Depth => _callStack.Count;

        /// <summary>
        /// Clear the entire call stack
        /// </summary>
        public void Clear()
        {
            if (_callStack.Count > 0)
            {
                Logger.Debug($"[ScriptCallStack] Clearing call stack (depth: {_callStack.Count})");
            }
            _callStack.Clear();
        }

        /// <summary>
        /// Peek at the top of the stack without removing
        /// </summary>
        public ScriptCallFrame Peek()
        {
            return _callStack.Count > 0 ? _callStack.Peek() : null;
        }
    }

    /// <summary>
    /// Represents a single frame in the script call stack
    /// </summary>
    public class ScriptCallFrame
    {
        /// <summary>
        /// The script that made the call
        /// </summary>
        public Script Script { get; set; }

        /// <summary>
        /// Name of the script being called
        /// </summary>
        public string ScriptName { get; set; }

        /// <summary>
        /// Line number where the call was made
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Optional return value provided by the called script via 'return <value>'
        /// </summary>
        public string ReturnValue { get; set; }
    }
}
