/*
 * Copyright(c) 2023 MoogleTroupe, trotlinebeercan, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using BardMusicPlayer.Quotidian.Enums;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer.Utilities;
using BardMusicPlayer.Seer.Utilities.KnownFolder;

namespace BardMusicPlayer.Seer;

public partial class Game
{
    private void InitInformation()
    {
        GamePath        = GetGamePath();
        EnvironmentType = GetEnvironmentType();
        GameRegion      = GetGameRegion();
        ConfigPath      = GetConfigPath();
    }

    /// <summary>
    /// Contains the Process object for this Game. Set on creation of this Game.
    /// </summary>
    public Process Process { get; }

    /// <summary>
    /// Contains the Process Id for this Game. Set on creation of this Game.
    /// </summary>
    public int Pid { get; private set; }

    /// <summary>
    /// Contains the region of this Game. Set on creation of this Game.
    /// </summary>
    public GameRegion GameRegion { get; private set; }

    /// <summary>
    /// Contains the environment of this Game. Set on creation of this Game.
    /// </summary>
    public EnvironmentType EnvironmentType { get; private set; }

    /// <summary>
    /// Contains the path to the boot/game folders for this Game. Set on creation of this Game.
    /// </summary>
    public string GamePath { get; private set; }

    /// <summary>
    /// Contains the path to the game configuration files like ffxiv.cfg and ConfigId folders. Set on creation of this Game.
    /// </summary>
    public string ConfigPath { get; private set; }

    /// <summary>
    /// Shows the player's actor id. Updated by Sharlayan and Machina.
    /// </summary>
    public uint ActorId { get; private set; }

    /// <summary>
    /// Shows the player's name. Updated by Sharlayan and Machina.
    /// </summary>
    public string PlayerName { get; private set; } = "Unknown";

    /// <summary>
    /// Shows the player's home world. Updated by Sharlayan and Machina.
    /// </summary>
    public string HomeWorld { get; private set; } = "Unknown";

    /// <summary>
    /// Shows if the player is logged in.
    /// </summary>
    public bool IsLoggedIn { get; private set; }

    /// <summary>
    /// Shows the instrument held. Updated by Sharlayan and Machina.
    /// </summary>
    public Instrument InstrumentHeld { get; private set; } = Instrument.None;

    /// <summary>
    /// Shows the instrument tone held. Updated by Sharlayan and Machina.
    /// </summary>
    public InstrumentTone InstrumentToneHeld { get; private set; } = InstrumentTone.None;

    /// <summary>
    /// Shows if the chatbox is open for input. Updated by Sharlayan.
    /// </summary>
    public bool ChatStatus { get; private set; }

    /// <summary>
    /// Returns false if we know the player is not a bard.
    /// </summary>
    public bool IsBard { get; private set; } = true;

    /// <summary>
    /// Test
    /// </summary>
    public long ServerLatency { get; private set; }

    /// <summary>
    /// Indicates if gfx set to low
    /// </summary>
    public bool GfxSettingsLow { get; set; }

    /// <summary>
    /// Indicates if sound is on
    /// </summary>
    public bool SoundOn { get; set; }

    /// <summary>
    /// Contains nearby partymember list. Updated by Sharlayan & Machina. Currently only Sharlayan updates during logoff.
    /// Fields: uint ActorId, string PlayerName
    /// This dictionary is sorted on ActorId and can be compared to another Game's PartyMembers with .Equals() extension method.
    /// </summary>
    public IReadOnlyDictionary<uint, string> PartyMembers { get; private set; } =
        new ReadOnlyDictionary<uint, string>(new SortedDictionary<uint, string>());

    /// <summary>
    /// Contains the config path used by DatReader. Updated by Sharlayan.
    /// </summary>
    public string ConfigId { get; private set; } = "";

    /// <summary>
    /// Contains KeyMaps for instrument hotbar keys. Unbound keys = Keys.None. Updated by DatReader via ConfigId from Sharlayan.
    /// </summary>
    public IReadOnlyDictionary<Instrument, Keys> InstrumentKeys { get; private set; } =
        new ReadOnlyDictionary<Instrument, Keys>(Instrument.All.ToDictionary(instrument => instrument,
            _ => Keys.None));

    /// <summary>
    /// Contains KeyMaps for instrument tone hotbar keys. Unbound keys = Keys.None. Updated by DatReader via ConfigId from Sharlayan.
    /// </summary>
    public IReadOnlyDictionary<InstrumentTone, Keys> InstrumentToneKeys { get; private set; } =
        new ReadOnlyDictionary<InstrumentTone, Keys>(
            InstrumentTone.All.ToDictionary(instrumentTone => instrumentTone, _ => Keys.None));

    /// <summary>
    /// Contains KeyMaps for navigation menu keys. Unbound keys = Keys.None. Updated by DatReader via ConfigId from Sharlayan.
    /// </summary>
    public IReadOnlyDictionary<NavigationMenuKey, Keys> NavigationMenuKeys { get; private set; } =
        new ReadOnlyDictionary<NavigationMenuKey, Keys>(Enum.GetValues(typeof(NavigationMenuKey))
            .Cast<NavigationMenuKey>().ToDictionary(navigationMenuKey => navigationMenuKey, _ => Keys.None));

    /// <summary>
    /// Contains KeyMaps for instrument tone menu keys. Unbound keys = Keys.None. Updated by DatReader via ConfigId from Sharlayan.
    /// </summary>
    public IReadOnlyDictionary<InstrumentToneMenuKey, Keys> InstrumentToneMenuKeys { get; private set; } =
        new ReadOnlyDictionary<InstrumentToneMenuKey, Keys>(Enum.GetValues(typeof(InstrumentToneMenuKey))
            .Cast<InstrumentToneMenuKey>()
            .ToDictionary(instrumentToneMenuKey => instrumentToneMenuKey, _ => Keys.None));

    /// <summary>
    /// Contains KeyMaps for note keys. Unbound keys = Keys.None. Updated by DatReader via ConfigId from Sharlayan.
    /// </summary>
    public IReadOnlyDictionary<NoteKey, Keys> NoteKeys { get; private set; } =
        new ReadOnlyDictionary<NoteKey, Keys>(Enum.GetValues(typeof(NoteKey)).Cast<NoteKey>()
            .ToDictionary(noteKey => noteKey, _ => Keys.None));

    private string GetGamePath()
    {
        try
        {
            var gamePath = Process.Modules.Cast<ProcessModule>()
                .Aggregate("",
                    (current, module) =>
                        module.ModuleName.ToLower() switch
                        {
                            "ffxiv_dx11.exe" => Directory
                                .GetParent(Path.GetDirectoryName(module.FileName) ?? string.Empty)
                                ?.FullName,
                            _ => current
                        }
                );

            if (string.IsNullOrEmpty(gamePath))
            {
                throw new BmpSeerGamePathException(
                    "Cannot locate the running directory of this game's ffxiv_dx11.exe");
            }

            return gamePath + @"\";
        }
        catch (Exception ex)
        {
            throw new BmpSeerGamePathException(
                "Unexpected error while trying to locate the path to ffxiv_dx11.exe: " + Environment.NewLine +
                ex.Message);
        }
    }

    private EnvironmentType GetEnvironmentType()
    {
        try
        {
            var modules = Process.Modules;
            var environmentType = modules.Cast<ProcessModule>()
                .Aggregate(EnvironmentType.Normal, (current, module) => module.ModuleName.ToLower() switch
                {
                    "sbiedll.dll"    => EnvironmentType.Sandboxie,
                    "innerspace.dll" => EnvironmentType.InnerSpace,
                    _                => current
                });
            return environmentType;
        }
        catch (Exception ex)
        {
            throw new BmpSeerEnvironmentTypeException(
                "Unexpected error while trying to detect the environment of a running game: " +
                Environment.NewLine + ex.Message);
        }
    }

    private GameRegion GetGameRegion()
    {
        try
        {
            var gameRegion = GameRegion.Global;

            if (File.Exists(GamePath + @"boot\locales\ko.pak")) gameRegion = GameRegion.Korea;
            else if (Directory.Exists(GamePath + @"sdo")) gameRegion       = GameRegion.China;

            return gameRegion;
        }
        catch (Exception ex)
        {
            throw new BmpSeerGameRegionException(
                "Unexpected error while trying to detect the region of a running game: " + Environment.NewLine +
                ex.Message);
        }
    }

    private string GetConfigPath()
    {
        var partialConfigPath = GameRegion == GameRegion.Korea
            ? @"My Games\FINAL FANTASY XIV - KOREA\"
            : @"My Games\FINAL FANTASY XIV - A Realm Reborn\";
        var configPath = "";

        try
        {
            if (EnvironmentType == EnvironmentType.Sandboxie)
            {
                // Per sandboxie documentation, the configuration file can be in multiple locations.
                var sandboxieConfigFilePath = "";

                try
                {
                    if (File.Exists(
                            Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\Sandboxie.ini"))
                    {
                        sandboxieConfigFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Windows) +
                                                  @"\Sandboxie.ini";
                    }
                    else
                    {
                        sandboxieConfigFilePath = Process.GetProcesses()
                            .Where(process => process.ProcessName.ToLower().Equals("sbiectrl"))
                            .Select(sandboxieProcess => sandboxieProcess.Modules)
                            .Aggregate(sandboxieConfigFilePath, (current1, sandboxieModules) => sandboxieModules
                                .Cast<ProcessModule>()
                                .Aggregate(current1, (current, sandboxieModule) =>
                                    sandboxieModule.ModuleName.ToLower() switch
                                    {
                                        "sbiectrl.exe" => Path.GetDirectoryName(sandboxieModule.FileName) +
                                                          @"\Sandboxie.ini",
                                        _ => current
                                    }));
                    }
                }
                finally
                {
                    if (string.IsNullOrEmpty(sandboxieConfigFilePath))
                    {
                        throw new BmpSeerConfigPathException(
                            "This game is running in Sandboxie, however the Sandboxie.ini configuration file could not be found.");
                    }
                }

                // We only accept sandbox windows with the sandbox in the title like " [#] [bard1] FINAL FANTASY XIV [#] ", this will fail otherwise.
                var boxName = Process.MainWindowTitle.Split('[')[2].Split(']')[0];

                // Note: sandboxie is a legacy program and writes it's config file in 2-byte per character mode, or Unicode in c# terms.
                // There is a newer open source fork that may change this, we may have to deal with it later.
                var boxRoot = File.ReadLines(sandboxieConfigFilePath, Encoding.Unicode)
                    .First(line => line.StartsWith("BoxRootFolder"))
                    .Split('=').Last() + @"\Sandbox\" + boxName + @"\";

                if (Directory.Exists(boxRoot))
                {
                    if (GameRegion == GameRegion.China)
                    {
                        configPath = boxRoot + GamePath.Substring(0, 1) + @"\" +
                                     GamePath.Substring(2, GamePath.Length) + @"game\" + partialConfigPath;
                    }
                    else configPath = boxRoot + @"user\current\Documents\" + partialConfigPath;
                }
                else
                {
                    throw new BmpSeerConfigPathException(
                        "This game is running in Sandboxie, however the sandbox could not be found.");
                }
            }

            // Normal games + ISBoxer/Innerspace games.
            else
            {
                if (GameRegion == GameRegion.China) configPath = GamePath + @"game\" + partialConfigPath;
                else
                {
                    configPath = new KnownFolder(KnownFolderType.Documents, Process.WindowsIdentity()).Path + @"\" +
                                 partialConfigPath;
                }
            }

            if (string.IsNullOrEmpty(configPath) || !Directory.Exists(configPath))
            {
                throw new BmpSeerConfigPathException(
                    "Invalid config path for a running game: " + Environment.NewLine + configPath);
            }

            return configPath;
        }
        catch (Exception ex)
        {
            throw new BmpSeerConfigPathException(
                "Unexpected error while trying to locate the config path for a running game: "
                + Environment.NewLine + "PartialConfigPath: " + partialConfigPath
                + Environment.NewLine + "ConfigPath: " + configPath
                + Environment.NewLine + ex.Message);
        }
    }
}