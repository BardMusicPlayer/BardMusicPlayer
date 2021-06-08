using Sharlayan.Core;
using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

// Fixes and colors chat accordingly

namespace FFBardMusicPlayer
{
    internal class BmpChatParser
    {
        public static string Fixup(ChatLogItem item)
        {
            var rgx = new Regex("^([^ ]+ [^:]+):(.+)");
            var format = rgx.Replace(item.Line, "$1: $2");

            switch (item.Code)
            {
                case "000E":
                {
                    // Party
                    var pid = (format[0] & 0xF) + 1;
                    format = $"[{pid}] {format.Substring(1)}";
                    break;
                }
                case "000D":
                {
                    // PM receive
                    if (format.IndexOf(": ") != -1)
                    {
                        format = format.Replace(": ", " >> ");
                    }

                    break;
                }
                case "000C":
                {
                    // PM Send
                    if (format.IndexOf(": ") != -1)
                    {
                        format = $">> {format}";
                    }

                    break;
                }
                case "001B":
                {
                    // Novice Network
                    format = $"[NN] {format}";
                    break;
                }
                case "001C":
                {
                    // Custom Emote
                    if (format.IndexOf(": ") != -1)
                    {
                        format = format.Replace(": ", "");
                    }

                    break;
                }
                case "001D":
                {
                    // Standard Emote
                    if (format.IndexOf(": ") != -1)
                    {
                        format = format.Substring(format.IndexOf(": ") + 2);
                    }

                    break;
                }
                case "0018":
                {
                    // FC
                    format = $"<FC> {format}";
                    break;
                }
                case "0010":
                case "0011":
                case "0012":
                case "0013":
                case "0014":
                case "0015":
                case "0016":
                case "0017":
                {
                    // LS
                    var ls = int.Parse(item.Code.Substring(3)) + 1;
                    format = $"[{ls}] {format}";
                    break;
                }
            }

            return format;
        }

        // Format for RTF
        public static string FormatRtf(string format, Color color = new Color(), bool bold = false)
        {
            var ftext = GetRtfUnicodeEscapedString(format);
            if (color.IsEmpty)
            {
                color = Color.White;
            }

            var col = $@"\red{color.R}\green{color.G}\blue{color.B}";
            if (bold)
            {
                ftext = string.Format("\\b" + ftext + "\\b0");
            }

            return $@"{{\rtf1 {{\colortbl ;{col};}}\cf1 {ftext} }}";
        }

        public static string FormatChat(ChatLogItem item)
        {
            var format = Fixup(item);
            var timestamp = item.TimeStamp.ToShortTimeString();

            var bold = false;
            var col = Color.White;
            switch (item.Code)
            {
                case "000E":
                {
                    // Party
                    col = Color.FromArgb(255, 150, 150, 250);
                    break;
                }
                case "000D":
                {
                    // PM receive
                    col = Color.FromArgb(255, 150, 150, 250);
                    break;
                }
                case "000C":
                {
                    // PM Send
                    col = Color.FromArgb(255, 150, 150, 250);
                    break;
                }
                case "001D":
                {
                    // Emote
                    col = Color.FromArgb(255, 250, 150, 150);
                    break;
                }
                case "001C":
                {
                    // Custom emote
                    col = Color.FromArgb(255, 250, 150, 150);
                    break;
                }
                case "000A":
                {
                    // Say
                    col = Color.FromArgb(255, 240, 240, 240);
                    break;
                }
                case "0839":
                {
                    // System
                    col = Color.FromArgb(255, 20, 20, 20);
                    break;
                }
                case "0018":
                {
                    // FC
                    col = Color.FromArgb(255, 150, 200, 150);
                    break;
                }
                case "0010":
                case "0011":
                case "0012":
                case "0013":
                case "0014":
                case "0015":
                case "0016":
                case "0017":
                {
                    col = Color.FromArgb(255, 200, 200, 150);
                    break;
                }
                default:
                {
                    col = Color.FromArgb(255, 200, 200, 200);
                    break;
                }
            }

            format = $"[{timestamp}] {format}";
            return FormatRtf(format, col, bold);
        }

        private static string GetRtfUnicodeEscapedString(string s)
        {
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                if (c == '\\' || c == '{' || c == '}')
                {
                    sb.Append($@"\{c}");
                }
                else if (c == '\n')
                {
                    sb.Append(@"\par");
                }
                else if (c <= 0x7f)
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append($"\\u{Convert.ToUInt32(c)}?");
                }
            }

            return sb.ToString();
        }
    }
}