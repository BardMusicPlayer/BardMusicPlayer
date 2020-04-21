using Sharlayan.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FFBardMusicPlayer {
	class BmpChatParser {

		public static string Fixup(ChatLogItem item) {
			Regex rgx = new Regex("^([^ ]+ [^:]+):(.+)");
			string format = rgx.Replace(item.Line, "$1: $2");

			switch(item.Code) {
				case "000E": { // Party
					int pid = (int) (format[0] & 0xF) + 1;
					format = string.Format("[{0}] {1}", pid, format.Substring(1));
					break;
				}
				case "000D": { // PM receive
					if(format.IndexOf(": ") != -1) {
						format = format.Replace(": ", " >> ");
					}
					break;
				}
				case "000C": { // PM Send
					if(format.IndexOf(": ") != -1) {
						format = ">> " + format;
					}
					break;
				}
				case "001B": { // Novice Network
					format = "[NN] " + format;
					break;
				}
				case "001C": { // Custom Emote
					if(format.IndexOf(": ") != -1) {
						format = format.Replace(": ", "");
					}
					break;
				}
				case "001D": { // Standard Emote
					if(format.IndexOf(": ") != -1) {
						format = format.Substring(format.IndexOf(": ") + 2);
					}
					break;
				}
				case "0018": { // FC
					format = string.Format("<FC> {0}", format);
					break;
				}
				case "0010":
				case "0011":
				case "0012":
				case "0013":
				case "0014":
				case "0015":
				case "0016":
				case "0017": { // LS
					int ls = int.Parse(item.Code.Substring(3)) + 1;
					format = string.Format("[{0}] {1}", ls, format);
					break;
				}
				default: {
					break;
				}
			}
			return format;
		}

		// Format for RTF
		public static string FormatRtf(string format, Color color = new Color(), bool bold = false) {
			string ftext = GetRtfUnicodeEscapedString(format);
			if(color.IsEmpty) {
				color = Color.White;
			}
			string col = string.Format(@"\red{0}\green{1}\blue{2}", color.R, color.G, color.B);
			if(bold) {
				ftext = string.Format("\\b" + ftext + "\\b0");
			}
			return string.Format(@"{{\rtf1 {{\colortbl ;{0};}}\cf1 {1} }}", col, ftext);
		}

		public static string FormatChat(ChatLogItem item) {

			string format = BmpChatParser.Fixup(item);
			string timestamp = item.TimeStamp.ToShortTimeString();

			bool bold = false;
			Color col = Color.White;
			switch(item.Code) {
				case "000E": { // Party
					col = Color.FromArgb(255, 150, 150, 250);
					break;
				}
				case "000D": { // PM receive
					col = Color.FromArgb(255, 150, 150, 250);
					break;
				}
				case "000C": { // PM Send
					col = Color.FromArgb(255, 150, 150, 250);
					break;
				}
				case "001D": { // Emote
					col = Color.FromArgb(255, 250, 150, 150);
					break;
				}
				case "001C": { // Custom emote
					col = Color.FromArgb(255, 250, 150, 150);
					break;
				}
				case "000A": { // Say
					col = Color.FromArgb(255, 240, 240, 240);
					break;
				}
				case "0839": { // System
					col = Color.FromArgb(255, 20, 20, 20);
					break;
				}
				case "0018": { // FC
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
				case "0017": {
					col = Color.FromArgb(255, 200, 200, 150);
					break;
				}
				default: {
					col = Color.FromArgb(255, 200, 200, 200);
					break;
				}
			}
			format = string.Format("[{0}] {1}", timestamp, format);
			return BmpChatParser.FormatRtf(format, col, bold);
		}

		private static string GetRtfUnicodeEscapedString(string s) {
			var sb = new StringBuilder();
			foreach(var c in s) {
				if(c == '\\' || c == '{' || c == '}')
					sb.Append(@"\" + c);
				else if(c == '\n')
					sb.Append(@"\par");
				else if(c <= 0x7f)
					sb.Append(c);
				else
					sb.Append("\\u" + Convert.ToUInt32(c) + "?");
			}
			return sb.ToString();
		}
	}
}
