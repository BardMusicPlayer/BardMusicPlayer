using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BardMusicPlayer.Quotidian.Enums;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer.Utilities;
using BardMusicPlayer.Seer.Utilities.KnownFolder;
using static System.Net.Mime.MediaTypeNames;

namespace BardMusicPlayer.Seer
{
    public partial class Game
    {
        /// <summary>
        /// Return true if low settings
        /// </summary>
        /// <returns></returns>
        private bool CheckIfGfxIsLow()
        {
            int number = 5;
            if (File.Exists(ConfigPath+"FFXIV.cfg"))
            {
                using (StreamReader sr = File.OpenText(ConfigPath + "FFXIV.cfg"))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
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
                }
            }

            return number == 0 ? true : false;
        }

        private int GetCfgIntSetting(string SettingsString)
        {
            int resultString = Convert.ToInt32(Regex.Replace(SettingsString.Split('\t')[1], "[^.0-9]", ""));
            return resultString;
        }
    }
}