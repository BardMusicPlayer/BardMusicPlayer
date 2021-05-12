/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Utilities;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Core
{
	internal class ChatCleaner : INotifyPropertyChanged {
        private const RegexOptions DefaultOptions = RegexOptions.Compiled | RegexOptions.ExplicitCapture;

        private static readonly Regex Checks = new(@"^00(20|21|23|27|28|46|47|48|49|5C)$", DefaultOptions);

        private static bool _colorFound;

        private readonly Regex _playerRegEx = new(@"(?<full>\[[A-Z0-9]{10}(?<first>[A-Z0-9]{3,})20(?<last>[A-Z0-9]{3,})\](?<short>[\w']+\.? [\w']+\.?)\[[A-Z0-9]{12}\])", DefaultOptions);

        private string _result;

        private readonly MemoryHandler _memoryHandler;

        public ChatCleaner(MemoryHandler memoryHandler, string line)
        {
            _memoryHandler = memoryHandler;
            Result = ProcessName(line);
        }

        public ChatCleaner(MemoryHandler memoryHandler, byte[] bytes) {
            _memoryHandler = memoryHandler;
            Result = ProcessFullLine(bytes).Trim();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public string Result {
            get => _result;
            private set {
                _result = value;
                RaisePropertyChanged();
            }
        }

        private bool ColorFound {
            get => _colorFound;
            set {
                _colorFound = value;
                RaisePropertyChanged();
            }
        }

        private string ProcessFullLine(byte[] bytes) {
            var line = HttpUtility.HtmlDecode(Encoding.UTF8.GetString(bytes.ToArray())).Replace("  ", " ");
            try {
                var autoTranslateList = new List<byte>();
                var newList = new List<byte>();
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
                                x += 3;
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
								var translated = new byte[limit];
								Buffer.BlockCopy(bytes, x, translated, 0, limit);
								Array.Reverse(translated);

                                ulong id = limit switch
                                {
                                    2 => SBitConverter.TryToUInt16(translated, 0),
                                    4 => SBitConverter.TryToUInt32(translated, 0),
                                    _ => 0
                                };
                                if(id != 0) {
									if(CompletionLookup.TryGetCompletion(id, out var completion)) {
										var c = string.Format("{{{0}}}", completion);
										newList.AddRange(Encoding.UTF8.GetBytes(c));
									}
								}
                                x += limit;
                            }
                            else {
                                x += 4;
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
            }
            catch (Exception ex) {
                _memoryHandler.RaiseException(ex);
            }

            return line;
        }

        private string ProcessName(string cleaned) {
            var line = cleaned;
            try {
                // cleanup name if using other settings
                var playerMatch = _playerRegEx.Match(line);
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
            }
            catch (Exception ex) {
                _memoryHandler?.RaiseException(ex);
            }

            return line;
        }

        private void RaisePropertyChanged([CallerMemberName] string caller = "") {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }
	}
}
