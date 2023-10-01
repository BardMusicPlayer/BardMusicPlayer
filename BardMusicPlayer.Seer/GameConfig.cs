/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System.Text.RegularExpressions;

namespace BardMusicPlayer.Seer;

public partial class Game
{
    private static int GetCfgIntSetting(string settingsString)
    {
        var resultString = Convert.ToInt32(Regex.Replace(settingsString.Split('\t')[1], "[^.0-9]", ""));
        return resultString;
    }

    private static string SetCfgSetting(string settingsString, string newValue)
    {
        settingsString = settingsString.Replace('\t' + settingsString.Split('\t')[1], '\t' + newValue);
        return settingsString;
    }

    #region Checks
    /// <summary>
    /// Return true if low settings
    /// </summary>
    /// <returns></returns>
    private bool CheckIfGfxIsLow()
    {
        var number = 5;
        if (File.Exists(ConfigPath+"FFXIV.cfg"))
        {
            using var sr = File.OpenText(ConfigPath + "FFXIV.cfg");
            while (sr.ReadLine() is { } s)
            {
                if (s.Contains("DisplayObjectLimitType"))
                    number -= GetCfgIntSetting(s);

                if (s.Contains("WaterWet_DX11"))
                    number -= GetCfgIntSetting(s);

                if (s.Contains("OcclusionCulling_DX11"))
                    number -= GetCfgIntSetting(s);

                if (s.Contains("ReflectionType_DX11"))
                    number -= GetCfgIntSetting(s);

                if (s.Contains("GrassQuality_DX11"))
                    number -= GetCfgIntSetting(s);

                if (s.Contains("SSAO_DX11"))
                    number -= GetCfgIntSetting(s);
            }
            sr.Close();
            sr.Dispose();
        }

        return number == 0;
    }

    /// <summary>
    ///     check if the master sound is enabled
    /// </summary>
    /// <returns></returns>
    private bool CheckIfSoundIsOn()
    {
        var soundOn = true;
        using var sr = File.OpenText(ConfigPath + "FFXIV.cfg");
        while (sr.ReadLine() is { } s)
        {
            if (s.Contains("IsSndMaster"))
                soundOn = GetCfgIntSetting(s) == 0;
        }
        sr.Close();
        sr.Dispose();
        return soundOn;
    }
    #endregion

    private bool CreateBackupConfig()
    {
        var pid = System.Diagnostics.Process.GetCurrentProcess().Id;
        if (!File.Exists(ConfigPath + "FFXIV.cfg"))
            return false;

        if (File.Exists(ConfigPath + "FFXIV.save"))
            return true;

        var reader = new StreamReader(ConfigPath + "FFXIV.cfg");
        var input = reader.ReadToEnd();
        reader.Close();

        var backupFile = File.Open(ConfigPath + "FFXIV.save", FileMode.Create);
        var writer = new StreamWriter(backupFile);
        writer.Write(input);
        writer.Close();
        backupFile.Close();
        return true;
    }

    public bool SetSoundOnOffLegacy(bool on)
    {
        if (!CreateBackupConfig()) return false;

        var configData = File.ReadAllText(ConfigPath + "FFXIV.cfg");

        var configFile = File.Open(ConfigPath + "FFXIV.cfg", FileMode.Create);
        using (var writer = new StreamWriter(configFile))
        {
            using (var stringreader = new StringReader(configData))
            {
                string line;
                do
                {
                    line = stringreader.ReadLine();
                    if (line != null)
                    {
                        line = line.Split('\t')[0] switch
                        {
                            "IsSndMaster" => SetCfgSetting(line, on ? "0" : "1"),
                            _             => line
                        };
                        writer.WriteLine(line);
                    }
                } while (line != null) ;
            }
            writer.Close();
        }
        configFile.Close();
        return true;
    }

    /// <summary>
    /// Set GFX to low
    /// </summary>
    /// <returns></returns>
    public bool SetGfxLow()
    {
        if (!CreateBackupConfig()) return false;

        var configData = File.ReadAllText(ConfigPath + "FFXIV.cfg");
        var configFile = File.Open(ConfigPath + "FFXIV.cfg", FileMode.Create);
        using (var writer = new StreamWriter(configFile))
        {
            using (var stringreader = new StringReader(configData))
            {
                string line;
                do
                {
                    line = stringreader.ReadLine();
                    if (line != null)
                    {
                        switch (line.Split('\t')[0])
                        {
                            case "DisplayObjectLimitType":
                                line = SetCfgSetting(line, "4");
                                break;
                            case "AntiAliasing_DX11":
                            case "TextureFilterQuality_DX11":
                            case "TextureAnisotropicQuality_DX11":
                            case "SSAO_DX11":
                            case "Glare_DX11":
                            case "DistortionWater_DX11":
                            case "DepthOfField_DX11":
                            case "RadialBlur_DX11":
                            case "GrassQuality_DX11":
                            case "TranslucentQuality_DX11":
                            case "ShadowSoftShadowType_DX11":
                            case "ShadowTextureSizeType_DX11":
                            case "ShadowCascadeCountType_DX11":
                            case "ShadowVisibilityTypeSelf_DX11":
                            case "ShadowVisibilityTypeParty_DX11":
                            case "ShadowVisibilityTypeOther_DX11":
                            case "ShadowVisibilityTypeEnemy_DX11":
                            case "PhysicsTypeSelf_DX11":
                            case "PhysicsTypeParty_DX11":
                            case "PhysicsTypeOther_DX11":
                            case "PhysicsTypeEnemy_DX11":
                            case "ReflectionType_DX11":
                            case "WaterWet_DX11":
                            case "ParallaxOcclusion_DX11":
                            case "Tessellation_DX11":
                            case "GlareRepresentation_DX11":
                                line = SetCfgSetting(line, "0");
                                break;
                            case "LodType_DX11":
                            case "OcclusionCulling_DX11":
                            case "ShadowLOD_DX11":
                                line = SetCfgSetting(line, "1");
                                break;
                            case "MapResolution_DX11":
                                line = SetCfgSetting(line, "2");
                                break;
                        }
                        writer.WriteLine(line);
                    }

                } while (line != null);
            }
            writer.Close();
        }
        configFile.Close();
        return true;
    }

    public void RestoreGfxSettings()
    {
        if (!File.Exists(ConfigPath + "FFXIV.save")) 
            return;

        var reader = new StreamReader(ConfigPath + "FFXIV.save");
        var input = reader.ReadToEnd();
        reader.Close();

        var backupFile = File.Open(ConfigPath + "FFXIV.cfg", FileMode.Create);
        var writer = new StreamWriter(backupFile);
        writer.Write(input);
        writer.Close();
        backupFile.Close();
        SetSoundOnOffLegacy(SoundOn);
    }

    private void RestoreOldConfig()
    {
        if (!File.Exists(ConfigPath + "FFXIV.save"))
            return;

        var reader = new StreamReader(ConfigPath + "FFXIV.save");
        var input = reader.ReadToEnd();
        reader.Close();

        var backupFile = File.Open(ConfigPath + "FFXIV.cfg", FileMode.Create);
        var writer = new StreamWriter(backupFile);
        writer.Write(input);
        writer.Close();
        backupFile.Close();

        File.Delete(ConfigPath + "FFXIV.save");
    }
}