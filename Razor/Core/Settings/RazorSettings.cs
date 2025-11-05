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
using System.IO;

namespace Assistant.Core.Settings
{
    /// <summary>
    /// Strongly-typed application settings that apply globally to Razor (not profile-specific)
    /// </summary>
    public class RazorSettings
    {
  private static RazorSettings _instance;
        private static readonly object _lock = new object();

        public static RazorSettings Instance
        {
            get
            {
     if (_instance == null)
 {
          lock (_lock)
      {
      if (_instance == null)
            {
           _instance = new RazorSettings();
          }
   }
                }
         return _instance;
          }
  }

        // Paths
        public string UODataDirectory { get; set; }
      public string UOClientPath { get; set; }
        public string BackupPath { get; set; }

   // Server Settings
 public string LastServer { get; set; }
      public int LastPort { get; set; }
      public int LastServerId { get; set; }

        // Security
        public bool ClientEncrypted { get; set; }
        public bool ServerEncrypted { get; set; }

        // UI Settings
        public bool ShowWelcome { get; set; }
        public string DefaultLanguage { get; set; }
    public string LastProfile { get; set; }

        // Agent Limits
        public int MaxOrganizerAgents { get; set; }
        public int MaxBuyAgents { get; set; }
        public int MaxRestockAgents { get; set; }

        // Other
        public bool ImportProfilesAndMacros { get; set; }
        public string BackupTime { get; set; }
        public string UId { get; set; }

        private RazorSettings()
    {
        // Set default values
   SetDefaults();
        }

        private void SetDefaults()
        {
         UODataDirectory = @"D:\Games\UO";
            UOClientPath = @"D:\Games\UO\client.exe";
            LastPort = 2593;
      LastProfile = "default";
       LastServer = "127.0.0.1";
    LastServerId = 0;
ClientEncrypted = true;
            ServerEncrypted = false;
            ShowWelcome = true;
         UId = string.Empty;
     MaxOrganizerAgents = 20;
  MaxBuyAgents = 20;
     MaxRestockAgents = 20;
      ImportProfilesAndMacros = true;
            BackupPath = @".\Backup";
          DefaultLanguage = "ENU";
          BackupTime = string.Empty;
        }

        /// <summary>
     /// Load settings from Config (backwards compatibility)
      /// </summary>
        public void LoadFromConfig()
        {
 UODataDirectory = Config.GetAppSetting<string>("UODataDir") ?? UODataDirectory;
            UOClientPath = Config.GetAppSetting<string>("UOClient") ?? UOClientPath;
            LastPort = Config.GetAppSetting<int>("LastPort");
            LastProfile = Config.GetAppSetting<string>("LastProfile") ?? LastProfile;
            LastServer = Config.GetAppSetting<string>("LastServer") ?? LastServer;
   LastServerId = Config.GetAppSetting<int>("LastServerId");
    ClientEncrypted = Config.GetAppSetting<int>("ClientEncrypted") == 1;
            ServerEncrypted = Config.GetAppSetting<int>("ServerEncrypted") == 1;
       ShowWelcome = Config.GetAppSetting<int>("ShowWelcome") == 1;
    MaxOrganizerAgents = Config.GetAppSetting<int>("MaxOrganizerAgents");
 MaxBuyAgents = Config.GetAppSetting<int>("MaxBuyAgents");
  MaxRestockAgents = Config.GetAppSetting<int>("MaxRestockAgents");
        ImportProfilesAndMacros = Config.GetAppSetting<bool>("ImportProfilesAndMacros");
          BackupPath = Config.GetAppSetting<string>("BackupPath") ?? BackupPath;
    DefaultLanguage = Config.GetAppSetting<string>("DefaultLanguage") ?? DefaultLanguage;
    BackupTime = Config.GetAppSetting<string>("BackupTime") ?? BackupTime;
   UId = Config.GetAppSetting<string>("UId") ?? UId;
      }

      /// <summary>
        /// Save settings back to Config (backwards compatibility)
        /// </summary>
        public void SaveToConfig()
        {
    Config.SetAppSetting("UODataDir", UODataDirectory);
  Config.SetAppSetting("UOClient", UOClientPath);
            Config.SetAppSetting("LastPort", LastPort.ToString());
      Config.SetAppSetting("LastProfile", LastProfile);
            Config.SetAppSetting("LastServer", LastServer);
            Config.SetAppSetting("LastServerId", LastServerId.ToString());
      Config.SetAppSetting("ClientEncrypted", ClientEncrypted ? "1" : "0");
            Config.SetAppSetting("ServerEncrypted", ServerEncrypted ? "1" : "0");
            Config.SetAppSetting("ShowWelcome", ShowWelcome ? "1" : "0");
        Config.SetAppSetting("MaxOrganizerAgents", MaxOrganizerAgents.ToString());
            Config.SetAppSetting("MaxBuyAgents", MaxBuyAgents.ToString());
       Config.SetAppSetting("MaxRestockAgents", MaxRestockAgents.ToString());
    Config.SetAppSetting("ImportProfilesAndMacros", ImportProfilesAndMacros.ToString());
            Config.SetAppSetting("BackupPath", BackupPath);
  Config.SetAppSetting("DefaultLanguage", DefaultLanguage);
      Config.SetAppSetting("BackupTime", BackupTime);
      Config.SetAppSetting("UId", UId);
        }
    }
}
