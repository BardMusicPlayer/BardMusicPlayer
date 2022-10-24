using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

// Fixes and colors chat accordingly

namespace BardMusicPlayer.Ui {
	static class BmpChatParser
	{
		public static string Fixup(Seer.Events.ChatLog item)
		{
			Regex rgx = new Regex("^([^ ]+ [^:]+):(.+)");
			string format = rgx.Replace(item.ChatLogLine, "$1: $2");

			switch(item.ChatLogCode) {
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
					break;
				}
				default: {
					break;
				}
			}
			return format;
		}

		public static KeyValuePair<string, System.Windows.Media.Color> FormatChat(Seer.Events.ChatLog item)
		{
			string format = BmpChatParser.Fixup(item);
			string timestamp = item.ChatLogTimeStamp.ToShortTimeString();

			System.Windows.Media.Color col = System.Windows.Media.Color.FromRgb(255,255,255);
			switch(item.ChatLogCode) {
				case "000E": { // Party
					col = System.Windows.Media.Color.FromArgb(255, 150, 150, 250);
					break;
				}
				case "000D": { // PM receive
					col = System.Windows.Media.Color.FromArgb(255, 150, 150, 250);
					break;
				}
				case "000C": { // PM Send
					col = System.Windows.Media.Color.FromArgb(255, 150, 150, 250);
					break;
				}
				case "001D": { // Emote
					col = System.Windows.Media.Color.FromArgb(255, 250, 150, 150);
					break;
				}
				case "001C": { // Custom emote
					col = System.Windows.Media.Color.FromArgb(255, 250, 150, 150);
					break;
				}
				case "000A": { // Say
					col = System.Windows.Media.Color.FromArgb(255, 240, 240, 240);
					break;
				}
				case "0839": { // System
					col = System.Windows.Media.Color.FromArgb(255, 20, 20, 20);
					break;
				}
				case "0018": { // FC
					col = System.Windows.Media.Color.FromArgb(255, 150, 200, 150);
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
					col = System.Windows.Media.Color.FromArgb(255, 200, 200, 150);
					break;
				}
				default: {
					col = System.Windows.Media.Color.FromArgb(255, 200, 200, 200);
					break;
				}
			}
			format = string.Format("[{0}] {1}", timestamp, format);
			return new KeyValuePair<string, System.Windows.Media.Color>(format, col);
		}

		public static void AppendText(this System.Windows.Controls.RichTextBox box, Seer.Events.ChatLog ev)
		{
            System.Windows.Media.BrushConverter bc = new System.Windows.Media.BrushConverter();
            System.Windows.Documents.TextRange tr = new System.Windows.Documents.TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
			var t = BmpChatParser.FormatChat(ev);
			tr.Text = t.Key;
			try
			{
				tr.ApplyPropertyValue(System.Windows.Documents.TextElement.ForegroundProperty, new System.Windows.Media.SolidColorBrush(t.Value));
			}
			catch (FormatException) { }
			box.AppendText("\r");
		}
	}
}
