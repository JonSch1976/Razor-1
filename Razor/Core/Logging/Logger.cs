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
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;

namespace Assistant.Core.Logging
{
    /// <summary>
    /// Log levels for categorizing messages
    /// </summary>
    public enum LogLevel
 {
        Trace,
        Debug,
        Info,
        Warning,
Error,
        Fatal
    }

    /// <summary>
    /// Centralized logging system for Razor
    /// Thread-safe, with async file writing and configurable levels
    /// </summary>
    public static class Logger
    {
        private static readonly ConcurrentQueue<LogEntry> _logQueue = new ConcurrentQueue<LogEntry>();
    private static readonly Thread _writerThread;
   private static readonly AutoResetEvent _writeEvent = new AutoResetEvent(false);
        private static readonly object _fileLock = new object();
  
    private static LogLevel _minLevel = LogLevel.Info;
        private static string _logDirectory;
        private static string _currentLogFile;
        private static bool _isRunning = true;
     private static bool _logToFile = true;
        private static bool _logToDebug = true;
     private static StreamWriter _writer;

        static Logger()
    {
            _logDirectory = Path.Combine(Engine.RootPath, "Logs");
   EnsureLogDirectory();
  CreateNewLogFile();
       
  _writerThread = new Thread(ProcessLogQueue)
      {
         IsBackground = true,
    Name = "Razor Logger Thread"
         };
     _writerThread.Start();
  }

      /// <summary>
        /// Minimum log level to write (default: Info)
 /// </summary>
        public static LogLevel MinimumLevel
        {
   get => _minLevel;
            set => _minLevel = value;
    }

        /// <summary>
        /// Enable/disable file logging
        /// </summary>
     public static bool LogToFile
        {
       get => _logToFile;
          set => _logToFile = value;
        }

     /// <summary>
        /// Enable/disable debug output logging
        /// </summary>
        public static bool LogToDebug
        {
    get => _logToDebug;
 set => _logToDebug = value;
   }

        /// <summary>
        /// Log directory path
   /// </summary>
        public static string LogDirectory => _logDirectory;

  /// <summary>
     /// Current log file path
    /// </summary>
        public static string CurrentLogFile => _currentLogFile;

        #region Public Logging Methods

        public static void Trace(string message) => Log(LogLevel.Trace, message);
  public static void Trace(string message, params object[] args) => Log(LogLevel.Trace, string.Format(message, args));
  public static void Trace(Exception ex, string message = null) => LogException(LogLevel.Trace, ex, message);

        public static void Debug(string message) => Log(LogLevel.Debug, message);
      public static void Debug(string message, params object[] args) => Log(LogLevel.Debug, string.Format(message, args));
      public static void Debug(Exception ex, string message = null) => LogException(LogLevel.Debug, ex, message);

        public static void Info(string message) => Log(LogLevel.Info, message);
    public static void Info(string message, params object[] args) => Log(LogLevel.Info, string.Format(message, args));
 public static void Info(Exception ex, string message = null) => LogException(LogLevel.Info, ex, message);

        public static void Warning(string message) => Log(LogLevel.Warning, message);
        public static void Warning(string message, params object[] args) => Log(LogLevel.Warning, string.Format(message, args));
        public static void Warning(Exception ex, string message = null) => LogException(LogLevel.Warning, ex, message);

        public static void Error(string message) => Log(LogLevel.Error, message);
     public static void Error(string message, params object[] args) => Log(LogLevel.Error, string.Format(message, args));
        public static void Error(Exception ex, string message = null) => LogException(LogLevel.Error, ex, message);

        public static void Fatal(string message) => Log(LogLevel.Fatal, message);
     public static void Fatal(string message, params object[] args) => Log(LogLevel.Fatal, string.Format(message, args));
   public static void Fatal(Exception ex, string message = null) => LogException(LogLevel.Fatal, ex, message);

        #endregion

        #region Core Logging

      private static void Log(LogLevel level, string message, string category = null)
   {
        if (level < _minLevel)
      return;

     var entry = new LogEntry
  {
           Timestamp = DateTime.Now,
                Level = level,
 Message = message,
            Category = category ?? GetCallerCategory(),
                ThreadId = Thread.CurrentThread.ManagedThreadId
            };

            _logQueue.Enqueue(entry);
            _writeEvent.Set();
        }

        private static void LogException(LogLevel level, Exception ex, string message)
        {
if (ex == null)
         return;

            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(message))
 {
     sb.AppendLine(message);
   }

       sb.AppendLine($"Exception: {ex.GetType().Name}");
            sb.AppendLine($"Message: {ex.Message}");
            sb.AppendLine($"Stack Trace: {ex.StackTrace}");

            if (ex.InnerException != null)
   {
       sb.AppendLine($"Inner Exception: {ex.InnerException.GetType().Name}");
       sb.AppendLine($"Inner Message: {ex.InnerException.Message}");
            }

      Log(level, sb.ToString());
  }

        #endregion

        #region Background Processing

        private static void ProcessLogQueue()
        {
 while (_isRunning)
   {
              _writeEvent.WaitOne(1000); // Wait for signal or 1 second timeout

      while (_logQueue.TryDequeue(out LogEntry entry))
                {
WriteEntry(entry);
                }

  // Flush writer periodically
       lock (_fileLock)
                {
         _writer?.Flush();
   }
            }
        }

        private static void WriteEntry(LogEntry entry)
        {
       string formattedEntry = FormatEntry(entry);

      // Write to debug output
            if (_logToDebug)
            {
          System.Diagnostics.Debug.WriteLine(formattedEntry);
            }

            // Write to file
         if (_logToFile)
       {
              lock (_fileLock)
     {
   try
       {
     _writer?.WriteLine(formattedEntry);
          }
     catch
  {
     // Silently fail if we can't write to log
         }
       }
            }
        }

        private static string FormatEntry(LogEntry entry)
    {
     return $"{entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{entry.Level,-7}] [{entry.ThreadId,3}] {entry.Category}: {entry.Message}";
        }

      #endregion

        #region File Management

 private static void EnsureLogDirectory()
        {
            try
            {
          if (!Directory.Exists(_logDirectory))
        {
      Directory.CreateDirectory(_logDirectory);
         }
            }
            catch
   {
         // If we can't create log directory, disable file logging
       _logToFile = false;
    }
     }

        private static void CreateNewLogFile()
        {
 lock (_fileLock)
            {
         try
    {
          _writer?.Close();
             
     string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
         _currentLogFile = Path.Combine(_logDirectory, $"Razor_{timestamp}.log");
         
             _writer = new StreamWriter(_currentLogFile, true)
     {
          AutoFlush = false
      };

            _writer.WriteLine("=".PadRight(80, '='));
   _writer.WriteLine($"Razor Log Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
 _writer.WriteLine($"Version: {Engine.Version}");
           _writer.WriteLine("=".PadRight(80, '='));
         _writer.WriteLine();
                _writer.Flush();
  }
       catch
     {
   _logToFile = false;
           }
      }
        }

        /// <summary>
      /// Rotate log file (create new one)
    /// </summary>
      public static void RotateLogFile()
     {
            CreateNewLogFile();
        Info("Log file rotated");
        }

        #endregion

 #region Cleanup

   /// <summary>
        /// Clean up old log files (keep last N days)
    /// </summary>
  public static void CleanupOldLogs(int daysToKeep = 7)
        {
    try
 {
     if (!Directory.Exists(_logDirectory))
  return;

  DateTime cutoffDate = DateTime.Now.AddDays(-daysToKeep);
           
       foreach (string file in Directory.GetFiles(_logDirectory, "Razor_*.log"))
          {
         FileInfo fi = new FileInfo(file);
           if (fi.CreationTime < cutoffDate && fi.FullName != _currentLogFile)
 {
           try
            {
         fi.Delete();
            Debug($"Deleted old log file: {fi.Name}");
        }
      catch
        {
 // Ignore errors deleting old logs
    }
   }
         }
   }
            catch (Exception ex)
{
        Error(ex, "Failed to cleanup old logs");
     }
        }

        #endregion

  #region Utility

      private static string GetCallerCategory()
        {
            try
            {
var stackTrace = new System.Diagnostics.StackTrace(3, false);
        var frame = stackTrace.GetFrame(0);
                var method = frame?.GetMethod();
   
          if (method != null)
          {
   return $"{method.DeclaringType?.Name}.{method.Name}";
          }
            }
            catch
  {
      // Ignore
       }

return "Unknown";
        }

  /// <summary>
      /// Shutdown logger (flush and close)
        /// </summary>
        public static void Shutdown()
    {
        Info("Logger shutting down...");
       
      _isRunning = false;
        _writeEvent.Set();
            
            // Wait for writer thread to finish
         if (_writerThread != null && _writerThread.IsAlive)
     {
           _writerThread.Join(2000);
            }

       // Flush remaining entries
            while (_logQueue.TryDequeue(out LogEntry entry))
            {
    WriteEntry(entry);
            }

lock (_fileLock)
          {
    _writer?.Flush();
                _writer?.Close();
         _writer = null;
    }
        }

        #endregion

        #region Log Entry Structure

        private class LogEntry
        {
  public DateTime Timestamp { get; set; }
      public LogLevel Level { get; set; }
   public string Message { get; set; }
 public string Category { get; set; }
            public int ThreadId { get; set; }
        }

  #endregion
    }
}
