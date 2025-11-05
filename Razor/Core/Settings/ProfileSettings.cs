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

namespace Assistant.Core.Settings
{
    /// <summary>
    /// Strongly-typed profile-specific settings
    /// Replaces the Dictionary-based approach in Config.cs
    /// </summary>
    public class ProfileSettings
    {
        #region Display Settings
        
public bool ShowMobNames { get; set; }
        public bool ShowCorpseNames { get; set; }
        public bool DisplaySkillChanges { get; set; }
        public bool TitleBarDisplay { get; set; }
        public string TitleBarText { get; set; }
        public bool AutoSearch { get; set; }
public bool NoSearchPouches { get; set; }
        public bool SortCounters { get; set; }
        public bool TitlebarImages { get; set; }
        public bool HighlightReagents { get; set; }
        
        #endregion

        #region UI Settings
        
        public bool AlwaysOnTop { get; set; }
      public int Opacity { get; set; }
        public int WindowX { get; set; }
  public int WindowY { get; set; }
        public int WindowSizeX { get; set; }
      public int WindowSizeY { get; set; }
        public bool Systray { get; set; }
        
    #endregion

 #region Counter & Warning Settings
        
      public int CounterWarnAmount { get; set; }
        public bool CounterWarn { get; set; }
        public bool CountStealthSteps { get; set; }
        
        #endregion

 #region Color & Hue Settings
        
      public int SysColor { get; set; }
        public int WarningColor { get; set; }
public int ExemptColor { get; set; }
   public int SpeechHue { get; set; }
        public int BeneficialSpellHue { get; set; }
      public int HarmfulSpellHue { get; set; }
   public int NeutralSpellHue { get; set; }
        public bool ForceSpeechHue { get; set; }
        public bool ForceSpellHue { get; set; }
 public bool ShowNotoHue { get; set; }
        public int LTHilight { get; set; }
      
        #endregion

        #region Action & Targeting Settings
        
        public bool QueueActions { get; set; }
      public bool QueueTargets { get; set; }
        public int ObjectDelay { get; set; }
        public bool ObjectDelayEnabled { get; set; }
        public bool SmartLastTarget { get; set; }
        public bool RangeCheckLT { get; set; }
  public int LTRange { get; set; }
        public bool LastTargTextFlags { get; set; }
    public bool ActionStatusMsg { get; set; }
        
        #endregion

        #region Spell & Combat Settings
     
      public string SpellFormat { get; set; }
        public bool SpellUnequip { get; set; }
public bool PotionEquip { get; set; }
        public bool BlockHealPoison { get; set; }
        public bool PotionReequip { get; set; }
        public bool OverrideSpellFormat { get; set; }
        
   #endregion

        #region Auto-Actions
        
        public bool AutoOpenCorpses { get; set; }
        public int CorpseRange { get; set; }
      public bool BlockDismount { get; set; }
        public bool AutoOpenDoors { get; set; }
     public bool AutoOpenDoorWhenHidden { get; set; }
        public bool AutoFriend { get; set; }
        public bool AutoStack { get; set; }
        
        #endregion

        #region Screenshot Settings
        
  public bool CapFullScreen { get; set; }
        public string CapPath { get; set; }
        public bool CapTimeStamp { get; set; }
        public string ImageFormat { get; set; }
        public bool CaptureOwnDeath { get; set; }
     public bool CaptureOthersDeath { get; set; }
 public double CaptureOwnDeathDelay { get; set; }
        public double CaptureOthersDeathDelay { get; set; }
   
        #endregion

        #region Dress & Equipment
        
        public bool UndressConflicts { get; set; }
        
        #endregion

        #region Agent Settings
     
        public int SellAgentMax { get; set; }
      public bool BuyAgentsIgnoreGold { get; set; }
  
        #endregion

      #region Skills Settings
      
        public int SkillListCol { get; set; }
  public bool SkillListAsc { get; set; }
      public bool LogSkillChanges { get; set; }
        public bool DisplaySkillChangesOverhead { get; set; }
      
        #endregion

        #region Message & Filter Settings
        
        public bool FilterSnoopMsg { get; set; }
        public bool FilterSystemMessages { get; set; }
  public bool FilterRazorMessages { get; set; }
        public double FilterDelay { get; set; }
     public bool FilterOverheadMessages { get; set; }
        public int MessageLevel { get; set; }
        public bool EnableTextFilter { get; set; }

        #endregion

        #region Status Bar & Overhead Settings
        
        public bool OldStatBar { get; set; }
        public bool ShowHealth { get; set; }
        public string HealthFmt { get; set; }
        public bool ShowPartyStats { get; set; }
        public string PartyStatFmt { get; set; }
        public bool ShowOverheadMessages { get; set; }
 public string OverheadFormat { get; set; }
 public int OverheadStyle { get; set; }

        #endregion

        #region Light & Season Settings
        
        public int LightLevel { get; set; }
        public int MaxLightLevel { get; set; }
        public int MinLightLevel { get; set; }
  public bool MinMaxLightLevelEnabled { get; set; }
     public int Season { get; set; }
 
        #endregion

        #region Security & Privacy
        
        public bool RememberPwds { get; set; }
        public bool LogPacketsByDefault { get; set; }
        
        #endregion

    #region Hotkey & Macro Settings
        
public bool HotKeyStop { get; set; }
        public bool DiffTargetByType { get; set; }
 public bool StepThroughMacro { get; set; }
    public bool MacroActionDelay { get; set; }
        public bool DisableMacroPlayFinish { get; set; }
        
        #endregion

     #region Game Client Settings
 
        public bool ForceSizeEnabled { get; set; }
   public int ForceSizeX { get; set; }
        public int ForceSizeY { get; set; }
        public bool Negotiate { get; set; }
        public string ForceIP { get; set; }
     public int ForcePort { get; set; }

        #endregion

        #region Advanced Features
        
public bool EnableUOAAPI { get; set; }
    public bool SmoothWalk { get; set; }
        public bool BlockOpenCorpsesTwice { get; set; }
    public bool BlockTradeRequests { get; set; }
     public bool BlockPartyInvites { get; set; }
        public bool AutoAcceptParty { get; set; }
        
      #endregion

        #region Target Filters
     
 public bool TargetFilterEnabled { get; set; }
        public bool RangeCheckTargetByType { get; set; }
        public bool RangeCheckDoubleClick { get; set; }
  public bool NextPrevTargetIgnoresFriends { get; set; }
        public bool OnlyNextPrevBeneficial { get; set; }
public bool FriendlyBeneficialOnly { get; set; }
        public bool NonFriendlyHarmfulOnly { get; set; }
        public bool NextPrevAlphabetical { get; set; }
   
        #endregion

        #region Bandage Settings
     
        public bool ShowBandageTimer { get; set; }
        public string ShowBandageTimerFormat { get; set; }
      public int ShowBandageTimerLocation { get; set; }
        public bool OnlyShowBandageTimerEvery { get; set; }
        public int OnlyShowBandageTimerSeconds { get; set; }
        public int ShowBandageTimerHue { get; set; }
        public bool ShowBandageStart { get; set; }
     public string BandageStartMessage { get; set; }
        public bool ShowBandageEnd { get; set; }
        public string BandageEndMessage { get; set; }
        
  #endregion

#region Buff/Debuff Settings
        
        public bool ShowBuffDebuffOverhead { get; set; }
        public string BuffDebuffFormat { get; set; }
        public int BuffDebuffSeconds { get; set; }
        public int BuffHue { get; set; }
        public int DebuffHue { get; set; }
 public bool DisplayBuffDebuffEvery { get; set; }
        public string BuffDebuffFilter { get; set; }
   public bool BuffDebuffEveryXSeconds { get; set; }
        public bool OverrideBuffDebuffFormat { get; set; }
        public bool ShowBuffDebuffGump { get; set; }
        public bool ShowBuffDebuffIcons { get; set; }
  public int ShowBuffDebuffWidth { get; set; }
        public int ShowBuffDebuffHeight { get; set; }
        public int ShowBuffDebuffSort { get; set; }
  public bool UseBlackBuffDebuffBg { get; set; }
        public int ShowBuffDebuffTimeType { get; set; }
        
    #endregion

      #region Mobile Graphics Filters
        
        public bool FilterDragonGraphics { get; set; }
     public int DragonGraphic { get; set; }
        public bool FilterDrakeGraphics { get; set; }
        public int DrakeGraphic { get; set; }
  public bool FilterDaemonGraphics { get; set; }
    public int DaemonGraphic { get; set; }
        public bool FilterWyrmGraphics { get; set; }
        public int WyrmGraphic { get; set; }
        
    #endregion

        #region Sound Settings
        
        public bool SoundFilterEnabled { get; set; }
 public bool ShowFilteredSound { get; set; }
    public bool ShowPlayingSoundInfo { get; set; }
        public bool ShowMusicInfo { get; set; }
        public bool PlayEmoteSound { get; set; }
        
 #endregion

        #region Damage Tracking
     
 public bool ShowDamageDealt { get; set; }
        public bool ShowDamageDealtOverhead { get; set; }
        public bool ShowDamageTaken { get; set; }
        public bool ShowDamageTakenOverhead { get; set; }
        
 #endregion

 #region Overhead Indicators
        
        public bool ShowAttackTargetOverhead { get; set; }
    public bool ShowAttackTargetNewOnly { get; set; }
        public bool ShowTargetSelfLastClearOverhead { get; set; }
 public bool ShowTextTargetIndicator { get; set; }
        public string TargetIndicatorFormat { get; set; }
        public int TargetIndicatorHue { get; set; }
  public string StealthStepsFormat { get; set; }
        public bool StealthOverhead { get; set; }
        public bool ShowFriendOverhead { get; set; }
        public bool ShowPartyFriendOverhead { get; set; }
        public bool HighlightFriend { get; set; }
        
        #endregion

        #region Waypoint Settings
        
  public bool ShowWaypointOverhead { get; set; }
        public bool ShowWaypointDistance { get; set; }
 public int ShowWaypointSeconds { get; set; }
 public bool ClearWaypoint { get; set; }
        public int HideWaypointDistance { get; set; }
        public bool CreateWaypointOnDeath { get; set; }
        
      #endregion

     #region Container & Label Settings
        
        public bool ShowContainerLabels { get; set; }
        public string ContainerLabelFormat { get; set; }
        public int ContainerLabelColor { get; set; }
        public int ContainerLabelStyle { get; set; }
   public bool ShowStaticWalls { get; set; }
        public bool ShowStaticWallLabels { get; set; }
        
   #endregion

      #region Script Settings
     
        public bool AutoSaveScript { get; set; }
        public bool AutoSaveScriptPlay { get; set; }
 public bool ScriptDisablePlayFinish { get; set; }
 public bool DisableScriptTooltips { get; set; }
        public bool DefaultScriptDelay { get; set; }
        public bool EnableHighlight { get; set; }
        public bool DisableScriptStopwatch { get; set; }
   
        #endregion

    #region Advanced Display
        
        public bool ShowInRazorTitleBar { get; set; }
   public string RazorTitleBarText { get; set; }
   public bool GoldPerDisplay { get; set; }
public bool CaptureMibs { get; set; }
        
        #endregion

        #region Map Settings
  
 public int MapX { get; set; }
      public int MapY { get; set; }
        public int MapW { get; set; }
        public int MapH { get; set; }
        
        #endregion

        #region Cooldown Settings
     
      public int CooldownHeight { get; set; }
     public int CooldownWidth { get; set; }
      
      #endregion

  /// <summary>
        /// Create a new ProfileSettings with default values
        /// </summary>
        public ProfileSettings()
        {
       SetDefaults();
        }

        /// <summary>
     /// Set all settings to their default values
  /// </summary>
        public void SetDefaults()
        {
       // Display
        ShowMobNames = false;
     ShowCorpseNames = false;
            DisplaySkillChanges = false;
    TitleBarDisplay = true;
            TitleBarText = Client.IsOSI 
  ? @"UO - {char} {crimtime}- {mediumstatbar} {bp} {bm} {gl} {gs} {mr} {ns} {ss} {sa} {aids}"
      : @"UO - {char}";
 AutoSearch = true;
            NoSearchPouches = true;
   SortCounters = true;
            TitlebarImages = true;
    HighlightReagents = true;

          // UI
 AlwaysOnTop = false;
            Opacity = 100;
         WindowX = 400;
 WindowY = 400;
   WindowSizeX = 546;
         WindowSizeY = 411;
            Systray = false;

   // Counters
            CounterWarnAmount = 5;
     CounterWarn = true;
    CountStealthSteps = true;

    // Colors
      SysColor = 0x0044;
  WarningColor = 0x0025;
  ExemptColor = 0x0480;
   SpeechHue = 0x03B1;
            BeneficialSpellHue = 0x0005;
        HarmfulSpellHue = 0x0058;
 NeutralSpellHue = 0x03B1;
            ForceSpeechHue = false;
ForceSpellHue = true;
    ShowNotoHue = true;
     LTHilight = 0;

            // Actions
            QueueActions = false;
    QueueTargets = false;
          ObjectDelay = 600;
ObjectDelayEnabled = true;
        SmartLastTarget = false;
         RangeCheckLT = true;
  LTRange = 12;
            LastTargTextFlags = true;
   ActionStatusMsg = true;

     // Spells
   SpellFormat = @"{power} [{spell}]";
       SpellUnequip = true;
     PotionEquip = false;
    BlockHealPoison = true;
            PotionReequip = true;
 OverrideSpellFormat = true;

            // Auto-actions
  AutoOpenCorpses = false;
            CorpseRange = 2;
            BlockDismount = false;
         AutoOpenDoors = true;
          AutoOpenDoorWhenHidden = false;
   AutoFriend = false;
         AutoStack = false;

            // Screenshots
 CapFullScreen = false;
         CapPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures), "RazorScreenShots");
      CapTimeStamp = true;
       ImageFormat = "jpg";
   CaptureOwnDeath = false;
        CaptureOthersDeath = false;
            CaptureOwnDeathDelay = 0.5;
            CaptureOthersDeathDelay = 0.5;

            // Dress
UndressConflicts = true;

     // Agents
SellAgentMax = 99;
            BuyAgentsIgnoreGold = false;

 // Skills
            SkillListCol = -1;
    SkillListAsc = false;
            LogSkillChanges = false;
            DisplaySkillChangesOverhead = false;

            // Filters
    FilterSnoopMsg = true;
            FilterSystemMessages = false;
            FilterRazorMessages = false;
    FilterDelay = 3.5;
            FilterOverheadMessages = false;
            MessageLevel = 0;
          EnableTextFilter = false;

            // Status
  OldStatBar = false;
            ShowHealth = true;
   HealthFmt = "[{0}%]";
    ShowPartyStats = true;
 PartyStatFmt = "[{0}% / {1}%]";
         ShowOverheadMessages = false;
    OverheadFormat = "[{msg}]";
            OverheadStyle = 1;

          // Light & Season
            LightLevel = 31;
MaxLightLevel = 31;
          MinLightLevel = 0;
            MinMaxLightLevelEnabled = false;
         Season = 5;

          // Security
            RememberPwds = false;
   LogPacketsByDefault = false;

            // Hotkeys/Macros
            HotKeyStop = false;
            DiffTargetByType = false;
            StepThroughMacro = false;
            MacroActionDelay = Client.IsOSI;
        DisableMacroPlayFinish = false;

          // Game Client
     ForceSizeEnabled = false;
            ForceSizeX = 1000;
        ForceSizeY = 800;
        Negotiate = true;
    ForceIP = string.Empty;
            ForcePort = 0;

       // Advanced
 EnableUOAAPI = true;
SmoothWalk = false;
            BlockOpenCorpsesTwice = false;
    BlockTradeRequests = false;
      BlockPartyInvites = false;
        AutoAcceptParty = false;

   // Target Filters
   TargetFilterEnabled = false;
          RangeCheckTargetByType = false;
         RangeCheckDoubleClick = false;
    NextPrevTargetIgnoresFriends = false;
OnlyNextPrevBeneficial = false;
 FriendlyBeneficialOnly = false;
         NonFriendlyHarmfulOnly = false;
     NextPrevAlphabetical = false;

       // Bandages
    ShowBandageTimer = false;
            ShowBandageTimerFormat = "Bandage: {count}s";
            ShowBandageTimerLocation = 0;
   OnlyShowBandageTimerEvery = false;
            OnlyShowBandageTimerSeconds = 1;
    ShowBandageTimerHue = 88;
            ShowBandageStart = false;
    BandageStartMessage = "Bandage: Starting";
     ShowBandageEnd = false;
        BandageEndMessage = "Bandage: Ending";

     // Buff/Debuff
     ShowBuffDebuffOverhead = true;
        BuffDebuffFormat = "[{action}{name} {duration}]";
            BuffDebuffSeconds = 20;
 BuffHue = 88;
            DebuffHue = 338;
   DisplayBuffDebuffEvery = false;
            BuffDebuffFilter = string.Empty;
     BuffDebuffEveryXSeconds = false;
          OverrideBuffDebuffFormat = false;
       ShowBuffDebuffGump = false;
  ShowBuffDebuffIcons = true;
     ShowBuffDebuffWidth = 100;
        ShowBuffDebuffHeight = 18;
  ShowBuffDebuffSort = 2;
            UseBlackBuffDebuffBg = false;
   ShowBuffDebuffTimeType = 0;

   // Mobile Filters
          FilterDragonGraphics = false;
            DragonGraphic = 0;
 FilterDrakeGraphics = false;
   DrakeGraphic = 0;
   FilterDaemonGraphics = false;
            DaemonGraphic = 0;
     FilterWyrmGraphics = false;
  WyrmGraphic = 0;

            // Sound
          SoundFilterEnabled = false;
            ShowFilteredSound = false;
            ShowPlayingSoundInfo = false;
         ShowMusicInfo = false;
        PlayEmoteSound = false;

          // Damage Tracking
            ShowDamageDealt = false;
    ShowDamageDealtOverhead = false;
 ShowDamageTaken = false;
            ShowDamageTakenOverhead = false;

            // Overhead Indicators
      ShowAttackTargetOverhead = true;
            ShowAttackTargetNewOnly = true;
            ShowTargetSelfLastClearOverhead = true;
            ShowTextTargetIndicator = false;
            TargetIndicatorFormat = "* Target *";
      TargetIndicatorHue = 10;
            StealthStepsFormat = "Steps: {step}";
      StealthOverhead = false;
            ShowFriendOverhead = false;
         ShowPartyFriendOverhead = false;
            HighlightFriend = false;

      // Waypoints
   ShowWaypointOverhead = true;
            ShowWaypointDistance = true;
            ShowWaypointSeconds = 10;
            ClearWaypoint = false;
            HideWaypointDistance = 4;
            CreateWaypointOnDeath = false;

            // Containers/Labels
            ShowContainerLabels = false;
         ContainerLabelFormat = "[{label}] ({type})";
        ContainerLabelColor = 88;
     ContainerLabelStyle = 1;
       ShowStaticWalls = false;
            ShowStaticWallLabels = false;

            // Scripts
      AutoSaveScript = false;
            AutoSaveScriptPlay = false;
        ScriptDisablePlayFinish = false;
   DisableScriptTooltips = false;
            DefaultScriptDelay = true;
        EnableHighlight = false;
            DisableScriptStopwatch = false;

            // Display Advanced
      ShowInRazorTitleBar = false;
            RazorTitleBarText = "{name} on {account} ({profile} - {shard}) - Razor v{version}";
         GoldPerDisplay = false;
            CaptureMibs = false;

            // Map
        MapX = 200;
            MapY = 200;
  MapW = 200;
   MapH = 200;

            // Cooldown
            CooldownHeight = 28;
    CooldownWidth = 110;
        }
    }
}
