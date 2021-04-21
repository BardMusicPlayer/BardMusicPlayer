using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using BardMusicPlayer.Common;
using BardMusicPlayer.Seer.Reader.Memory.SignatureType;

namespace BardMusicPlayer.Seer.Reader.Memory {
    public static class ChatEntry {
        public static ChatLogItem Process(byte[] raw) {
            var chatLogEntry = new ChatLogItem();
            try {
                chatLogEntry.Bytes = raw;
                chatLogEntry.TimeStamp = UnixTimeStampToDateTime(int.Parse(ByteArrayToString(raw.Take(4).Reverse().ToArray()), NumberStyles.HexNumber));
                chatLogEntry.Code = ByteArrayToString(raw.Skip(4).Take(2).Reverse().ToArray());
                chatLogEntry.Raw = Encoding.UTF8.GetString(raw.ToArray());
                byte[] cleanable = raw.Skip(8).ToArray();
                var cleaned = new ChatCleaner(cleanable).Result;
                var cut = cleaned.Substring(1, 1) == ":"
                              ? 2
                              : 1;
                chatLogEntry.Line = cleaned.Substring(cut);//XMLCleaner.SanitizeXmlString(cleaned.Substring(cut));
                chatLogEntry.Line = new ChatCleaner(chatLogEntry.Line).Result;
                chatLogEntry.JP = IsJapanese(chatLogEntry.Line);

                chatLogEntry.Combined = $"{chatLogEntry.Code}:{chatLogEntry.Line}";
            } catch (Exception) {
                chatLogEntry.Bytes = Array.Empty<byte>();
                chatLogEntry.Raw = string.Empty;
                chatLogEntry.Line = string.Empty;
                chatLogEntry.Code = string.Empty;
                chatLogEntry.Combined = string.Empty;
            }

            return chatLogEntry;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private static string ByteArrayToString(byte[] raw) {
            var hex = new StringBuilder(raw.Length * 2);
            foreach (var b in raw) {
                hex.AppendFormat($"{b:X2}");
            }

            return hex.ToString();
        }

        private static bool IsJapanese(string line) {
            // 0x3040 -> 0x309F === Hirigana
            // 0x30A0 -> 0x30FF === Katakana
            // 0x4E00 -> 0x9FBF === Kanji
            return line.Any(c => c >= 0x3040 && c <= 0x309F) || line.Any(c => c >= 0x30A0 && c <= 0x30FF) || line.Any(c => c >= 0x4E00 && c <= 0x9FBF);
        }
    }

    internal class ChatCleaner {
        private const RegexOptions DefaultOptions = RegexOptions.Compiled | RegexOptions.ExplicitCapture;
        private static readonly Regex Checks = new Regex(@"^00(20|21|23|27|28|46|47|48|49|5C)$", DefaultOptions);

        private static bool _colorFound;

        private readonly Regex PlayerRegEx = new Regex(@"(?<full>\[[A-Z0-9]{10}(?<first>[A-Z0-9]{3,})20(?<last>[A-Z0-9]{3,})\](?<short>[\w']+\.? [\w']+\.?)\[[A-Z0-9]{12}\])", DefaultOptions);

        private string _result;

        public ChatCleaner(string line) {
            this.Result = this.ProcessName(line);
        }

        public ChatCleaner(byte[] bytes) {
            this.Result = this.ProcessFullLine(bytes).Trim();
        }

        public string Result {
            get => this._result;
            private set {
                this._result = value;
            }
        }

        private bool ColorFound {
            get => _colorFound;
            set {
                _colorFound = value;
            }
        }

        private string ProcessFullLine(byte[] bytes) {
            var line = HttpUtility.HtmlDecode(Encoding.UTF8.GetString(bytes.ToArray())).Replace("  ", " ");
            try {
                List<byte> autoTranslateList = new List<byte>();
                List<byte> newList = new List<byte>();
                for (var x = 0; x < bytes.Count(); x++) {
                    if (bytes[x] == 238) {
                        var byteString = $"{bytes[x]}{bytes[x + 1]}{bytes[x + 2]}";
                        switch (byteString) {
                            case "238129156":
                                x += 3;
                                break;
                        }
                    }

                    if (bytes[x] == 2) {
                        var byteString = $"{bytes[x]}{bytes[x + 1]}{bytes[x + 2]}{bytes[x + 3]}";
                        switch (byteString) {
                            case "22913":
                            case "21613":
                            case "22213":
                                x += 4;
                                break;
                        }
                    }

                    switch (bytes[x]) {
                        case 2:
                            // 2 46 5 7 242 2 210 3
                            // 2 29 1 3
                            var length = bytes[x + 2];
                            var limit = length - 1;
                            if (length > 1) {
                                x = x + 3;
                                /*
                                autoTranslateList.Add(Convert.ToByte('['));
                                byte[] translated = new byte[limit];
                                Buffer.BlockCopy(bytes, x, translated, 0, limit);
                                foreach (var b in translated) {
                                    autoTranslateList.AddRange(Encoding.UTF8.GetBytes(b.ToString("X2")));
                                }

                                autoTranslateList.Add(Convert.ToByte(']'));
                                var aCheckStr = string.Empty;
								Console.WriteLine(string.Format("AUTO TRANSLATE: {0}", Encoding.UTF8.GetString(autoTranslateList.ToArray())));

                                // var checkedAt = autoTranslateList.GetRange(1, autoTranslateList.Count - 1).ToArray();
                                if (string.IsNullOrWhiteSpace(aCheckStr)) {
                                    // TODO: implement showing or using in the chatlog
                                }
                                else {
                                    newList.AddRange(Encoding.UTF8.GetBytes(aCheckStr));
                                }

                                autoTranslateList.Clear();
								*/
                                byte[] translated = new byte[limit];
                                Buffer.BlockCopy(bytes, x, translated, 0, limit);
                                Array.Reverse(translated);

                                ulong id = 0;
                                if (limit == 2) {
                                    id = BitConverter.TryToUInt16(translated, 0);
                                }
                                if (limit == 4) {
                                    id = BitConverter.TryToUInt32(translated, 0);
                                }
                                if (id != 0) {
                                    if (CompletionLookup.TryGetCompletion(id, out string completion)) {
                                        string c = string.Format("{{{0}}}", completion);
                                        newList.AddRange(Encoding.UTF8.GetBytes(c));
                                    }
                                }
                                x += limit;
                            } else {
                                x = x + 4;
                                newList.Add(32);
                                newList.Add(bytes[x]);
                            }

                            break;
                        default:
                            newList.Add(bytes[x]);
                            break;
                    }
                }

                // var cleanedList = newList.Where(v => (v >= 0x0020 && v <= 0xD7FF) || (v >= 0xE000 && v <= 0xFFFD) || v == 0x0009 || v == 0x000A || v == 0x000D);
                var cleaned = HttpUtility.HtmlDecode(Encoding.UTF8.GetString(newList.ToArray())).Replace("  ", " ");

                autoTranslateList.Clear();
                newList.Clear();

                cleaned = Regex.Replace(cleaned, @"", "⇒");
                cleaned = Regex.Replace(cleaned, @"", "[HQ]");
                cleaned = Regex.Replace(cleaned, @"", string.Empty);
                cleaned = Regex.Replace(cleaned, @"�", string.Empty);
                cleaned = Regex.Replace(cleaned, @"\[+0([12])010101([\w]+)?\]+", string.Empty);
                cleaned = Regex.Replace(cleaned, @"\[+CF010101([\w]+)?\]+", string.Empty);
                cleaned = Regex.Replace(cleaned, @"\[+..FF\w{6}\]+|\[+EC\]+", string.Empty);
                cleaned = Regex.Replace(cleaned, @"\u001f", ":");
                cleaned = Regex.Replace(cleaned, @"\[\]+", string.Empty);

                line = cleaned;
            } catch (Exception ex) {
            }

            return line;
        }

        private string ProcessName(string cleaned) {
            var line = cleaned;
            try {
                // cleanup name if using other settings
                Match playerMatch = this.PlayerRegEx.Match(line);
                if (playerMatch.Success) {
                    var fullName = playerMatch.Groups[1].Value;
                    var firstName = playerMatch.Groups[2].Value.FromHex();
                    var lastName = playerMatch.Groups[3].Value.FromHex();
                    var player = $"{firstName} {lastName}";

                    // remove double placement
                    cleaned = line.Replace($"{fullName}:{fullName}", "•name•");

                    // remove single placement
                    cleaned = cleaned.Replace(fullName, "•name•");
                    switch (Regex.IsMatch(cleaned, @"^([Vv]ous|[Dd]u|[Yy]ou)")) {
                        case true:
                            cleaned = cleaned.Substring(1).Replace("•name•", string.Empty);
                            break;
                        case false:
                            cleaned = cleaned.Replace("•name•", player);
                            break;
                    }
                }

                cleaned = Regex.Replace(cleaned, @"[\r\n]+", string.Empty);
                cleaned = Regex.Replace(cleaned, @"[\x00-\x1F]+", string.Empty);
                line = cleaned;
            } catch (Exception ex) {
            }

            return line;
        }
    }
}
