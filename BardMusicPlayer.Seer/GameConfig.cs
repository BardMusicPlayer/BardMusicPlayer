/*
 * Copyright(c) 2025 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace BardMusicPlayer.Seer
{
    public sealed partial class Game
    {
        private static int GetCfgIntSetting(string SettingsString)
        {
            var resultString = Convert.ToInt32(Regex.Replace(SettingsString.Split('\t')[1], "[^.0-9]", ""));
            return resultString;
        }

        private static string SetCfgSetting(string SettingsString, string newValue)
        {
            SettingsString = SettingsString.Replace('\t' + SettingsString.Split('\t')[1], '\t' + newValue);
            return SettingsString;
        }

        #region Checks
        /// <summary>
        ///     Return true if low settings
        /// </summary>
        /// <returns></returns>
        private bool CheckIfGfxIsLow()
        {
            var number = 5;
            if (!File.Exists(ConfigPath + "FFXIV.cfg")) return false;

            using (var sr = File.OpenText(ConfigPath + "FFXIV.cfg"))
            {
                while (sr.ReadLine() is { } s)
                {
                    if (s.Contains("DisplayObjectLimitType")) number -= GetCfgIntSetting(s);

                    if (s.Contains("OcclusionCulling_DX11")) number -= GetCfgIntSetting(s);

                    if (s.Contains("ReflectionType_DX11")) number -= GetCfgIntSetting(s);

                    if (s.Contains("GrassQuality_DX11")) number -= GetCfgIntSetting(s);

                    if (s.Contains("SSAO_DX11")) number -= GetCfgIntSetting(s);
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
            bool SoundOn = true;
            using (var sr = File.OpenText(ConfigPath + "FFXIV.cfg"))
            {
                while (sr.ReadLine() is { } s)
                {
                    if (s.Contains("IsSndMaster"))
                        SoundOn = GetCfgIntSetting(s) == 0 ? true : false;
                }
                sr.Close();
                sr.Dispose();
            }
            return SoundOn;
        }
        #endregion

        private bool CreateBackupConfig()
        {
            var pid = System.Diagnostics.Process.GetCurrentProcess().Id;
            if (!File.Exists(ConfigPath + "FFXIV.cfg"))
                return false;

            if (File.Exists(ConfigPath + "FFXIV.save"))
                return true;

            StreamReader reader = new StreamReader(ConfigPath + "FFXIV.cfg");
            string input = reader.ReadToEnd();
            reader.Close();

            FileStream backupFile = File.Open(ConfigPath + "FFXIV.save", FileMode.Create);
            StreamWriter writer = new StreamWriter(backupFile);
            writer.Write(input);
            writer.Close();
            backupFile.Close();

            return true;
        }


        public bool SetSoundOnOffLegacy(bool on)
        {
            if (!CreateBackupConfig()) return false;

            string configData = File.ReadAllText(ConfigPath + "FFXIV.cfg");
            FileStream configFile = File.Open(ConfigPath + "FFXIV.cfg", FileMode.Create);
            using (StreamWriter writer = new StreamWriter(configFile))
            {
                using (StringReader stringreader = new StringReader(configData))
                {
                    string line = string.Empty;
                    do
                    {
                        line = stringreader.ReadLine();
                        if (line != null)
                        {
                            switch (line.Split('\t')[0])
                            {
                                case "IsSndMaster":
                                    line = SetCfgSetting(line, on ? "0" : "1");
                                    break;
                            }
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

            string configData = File.ReadAllText(ConfigPath + "FFXIV.cfg");
            FileStream configFile = File.Open(ConfigPath + "FFXIV.cfg", FileMode.Create);
            using (StreamWriter writer = new StreamWriter(configFile))
            {
                using (StringReader stringreader = new StringReader(configData))
                {
                    string line = string.Empty;
                    do
                    {
                        line = stringreader.ReadLine();
                        if (line != null)
                        {
                            switch (line.Split('\t')[0])
                            {
                                case "AntiAliasing_DX11":
                                case "TextureAnisotropicQuality_DX11":
                                case "SSAO_DX11":
                                case "DistortionWater_DX11":
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
                                case "ParallaxOcclusion_DX11":
                                case "Tessellation_DX11":
                                case "GlareRepresentation_DX11":
                                case "GrassEnableDynamicInterference":
                                case "TextureRezoType":
                                case "ShadowLightValidType":
                                case "GraphicsRezoUpscaleType":
                                    line = SetCfgSetting(line, "0");
                                    break;

                                case "Glare_DX11":
                                case "LodType_DX11":
                                case "OcclusionCulling_DX11":
                                case "ShadowLOD_DX11":
                                case "ShadowBgLOD":
                                case "DynamicRezoType":
                                    line = SetCfgSetting(line, "1");
                                    break;

                                case "Fps":
                                case "TextureFilterQuality_DX11":
                                    line = SetCfgSetting(line, "2");
                                    break;

                                case "DisplayObjectLimitType":
                                    line = SetCfgSetting(line, "4");
                                    break;

                                case "WindowDispNum":
                                    line = SetCfgSetting(line, "5");
                                    break;
                                case "GraphicsRezoScale":
                                    line = SetCfgSetting(line, "50");
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

        public void RestoreGFXSettings()
        {
            if (!File.Exists(ConfigPath + "FFXIV.save")) 
                return;

            StreamReader reader = new StreamReader(ConfigPath + "FFXIV.save");
            string input = reader.ReadToEnd();
            reader.Close();

            FileStream backupFile = File.Open(ConfigPath + "FFXIV.cfg", FileMode.Create);
            StreamWriter writer = new StreamWriter(backupFile);
            writer.Write(input);
            writer.Close();
            backupFile.Close();

            SetSoundOnOffLegacy(SoundOn);
        }

        public void RestoreOldConfig()
        {
            if (!File.Exists(ConfigPath + "FFXIV.save"))
                return;

            StreamReader reader = new StreamReader(ConfigPath + "FFXIV.save");
            string input = reader.ReadToEnd();
            reader.Close();

            FileStream backupFile = File.Open(ConfigPath + "FFXIV.cfg", FileMode.Create);
            StreamWriter writer = new StreamWriter(backupFile);
            writer.Write(input);
            writer.Close();
            backupFile.Close();

            File.Delete(ConfigPath + "FFXIV.save");
        }
    }
}