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

using System;
using System.Windows.Forms;

namespace Assistant.Core.Logging
{
    /// <summary>
    /// Extension methods and helpers for logging
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Log and show error message box
      /// </summary>
    public static void ErrorWithMessageBox(string title, string message, params object[] args)
        {
    string formattedMessage = args.Length > 0 ? string.Format(message, args) : message;
        Logger.Error($"[{title}] {formattedMessage}");
     MessageBox.Show(formattedMessage, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

  /// <summary>
        /// Log and show error message box with exception
        /// </summary>
        public static void ErrorWithMessageBox(string title, Exception ex, string message = null)
  {
            Logger.Error(ex, message);
            
      string displayMessage = message ?? ex.Message;
  MessageBox.Show(displayMessage, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Log and show warning message box
        /// </summary>
        public static void WarningWithMessageBox(string title, string message, params object[] args)
{
            string formattedMessage = args.Length > 0 ? string.Format(message, args) : message;
            Logger.Warning($"[{title}] {formattedMessage}");
            MessageBox.Show(formattedMessage, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

     /// <summary>
        /// Log and show info message box
        /// </summary>
        public static void InfoWithMessageBox(string title, string message, params object[] args)
    {
    string formattedMessage = args.Length > 0 ? string.Format(message, args) : message;
            Logger.Info($"[{title}] {formattedMessage}");
            MessageBox.Show(formattedMessage, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
      }

        /// <summary>
        /// Log exception with context
        /// </summary>
        public static void LogException(this Exception ex, string context)
{
  Logger.Error(ex, $"Exception in {context}");
        }

        /// <summary>
        /// Safe execute with error logging
      /// </summary>
      public static void SafeExecute(Action action, string context)
        {
  try
       {
    action();
          }
            catch (Exception ex)
        {
    Logger.Error(ex, $"Error executing {context}");
            }
        }

  /// <summary>
        /// Safe execute with error logging and return value
        /// </summary>
     public static T SafeExecute<T>(Func<T> func, string context, T defaultValue = default(T))
        {
       try
   {
     return func();
            }
     catch (Exception ex)
         {
      Logger.Error(ex, $"Error executing {context}");
      return defaultValue;
 }
 }
    }

    /// <summary>
    /// Category-specific loggers for better organization
    /// </summary>
    public static class LogCategories
    {
        public static class Network
      {
      public static void PacketSent(byte packetId, int length)
            {
Logger.Trace($"Packet sent: 0x{packetId:X2}, Length: {length}");
  }

  public static void PacketReceived(byte packetId, int length)
 {
          Logger.Trace($"Packet received: 0x{packetId:X2}, Length: {length}");
            }

    public static void ConnectionError(Exception ex)
            {
        Logger.Error(ex, "Network connection error");
            }
   }

        public static class Macros
    {
          public static void MacroStarted(string macroName)
   {
    Logger.Info($"Macro started: {macroName}");
       }

      public static void MacroFinished(string macroName, TimeSpan duration)
          {
 Logger.Info($"Macro finished: {macroName}, Duration: {duration.TotalSeconds:F2}s");
            }

    public static void MacroError(string macroName, Exception ex)
      {
        Logger.Error(ex, $"Error in macro: {macroName}");
     }
     }

        public static class Scripts
        {
            public static void ScriptStarted(string scriptName)
            {
        Logger.Info($"Script started: {scriptName}");
       }

        public static void ScriptFinished(string scriptName, TimeSpan duration)
         {
       Logger.Info($"Script finished: {scriptName}, Duration: {duration.TotalSeconds:F2}s");
            }

            public static void ScriptError(string scriptName, int lineNumber, Exception ex)
            {
   Logger.Error(ex, $"Script error in '{scriptName}' at line {lineNumber}");
            }
        }

        public static class Configuration
     {
   public static void ProfileLoaded(string profileName)
            {
     Logger.Info($"Profile loaded: {profileName}");
            }

          public static void ProfileSaved(string profileName)
         {
              Logger.Info($"Profile saved: {profileName}");
            }

      public static void SettingChanged(string setting, object oldValue, object newValue)
  {
        Logger.Debug($"Setting changed: {setting}, Old: {oldValue}, New: {newValue}");
       }

            public static void ConfigError(Exception ex)
   {
     Logger.Error(ex, "Configuration error");
            }
        }

        public static class UI
 {
    public static void FormOpened(string formName)
     {
      Logger.Debug($"Form opened: {formName}");
            }

      public static void FormClosed(string formName)
            {
        Logger.Debug($"Form closed: {formName}");
            }

         public static void UIError(string formName, Exception ex)
{
                Logger.Error(ex, $"UI error in {formName}");
   }
        }
    }
}
