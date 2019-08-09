using Sharlayan.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FFBardMusicPlayer {
	class BmpChatParser {

		public static string Fixup(ChatLogItem item) {
			string format = item.Line;
			Regex rgx = new Regex("^([^ ]+ [^:]+):(.+)");
			format = rgx.Replace(format, "$1: $2");

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
		public static string FormatRtf(string format, string col = @"\red255\green255\blue255", bool bold = false) {
			string ftext = GetRtfUnicodeEscapedString(format);
			if(bold) {
				ftext = string.Format("\\b" + ftext + "\\b0");
			}
			return string.Format(@"{{\rtf1 {{\colortbl ;{0};}}\cf1 {1} }}", col, ftext);
		}

		public static string FormatChat(ChatLogItem item) {

			string format = BmpChatParser.Fixup(item);
			string timestamp = item.TimeStamp.ToShortTimeString();

			bool bold = false;
			string col = string.Empty;
			switch(item.Code) {
				case "000E": { // Party
					col = @"\red150\green150\blue250";
					break;
				}
				case "000D": { // PM receive
					col = @"\red150\green150\blue250";
					break;
				}
				case "000C": { // PM Send
					col = @"\red150\green150\blue250";
					break;
				}
				case "001D": { // Emote
					col = @"\red250\green150\blue150";
					break;
				}
				case "001C": { // Custom emote
					col = @"\red250\green150\blue150";
					break;
				}
				case "000A": { // Say
					col = @"\red240\green240\blue240";
					break;
				}
				case "0839": { // System
					col = @"\red20\green20\blue20";
					break;
				}
				case "0018": { // FC
					col = @"\red150\green200\blue150";
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
					col = @"\red200\green200\blue150";
					break;
				}
				default: {
					col = @"\red200\green200\blue200";
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
				else if(c <= 0x7f)
					sb.Append(c);
				else
					sb.Append("\\u" + Convert.ToUInt32(c) + "?");
			}
			return sb.ToString();
		}
	}
}
