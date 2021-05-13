/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

using BardMusicPlayer.Siren.AlphaTab.Collections;

namespace BardMusicPlayer.Siren.AlphaTab.Model
{
    /// <summary>
    /// This public class represents a predefined string tuning.
    /// </summary>
    internal class Tuning
    {
        private static FastList<Tuning> _sevenStrings;
        private static FastList<Tuning> _sixStrings;
        private static FastList<Tuning> _fiveStrings;
        private static FastList<Tuning> _fourStrings;
        private static FastDictionary<int, Tuning> _defaultTunings;

        internal static string GetTextForTuning(int tuning, bool includeOctave)
        {
            var octave = tuning / 12;
            var note = tuning % 12;
            var notes = new[]
            {
                "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B"
            };
            var result = notes[note];
            if (includeOctave)
            {
                result += octave - 1;
            }

            return result;
        }

        /// <summary>
        /// Gets the default tuning for the given string count. 
        /// </summary>
        /// <param name="stringCount">The string count. </param>
        /// <returns>The tuning for the given string count or null if the string count is not defined. </returns>
        public static Tuning GetDefaultTuningFor(int stringCount)
        {
            if (_defaultTunings.ContainsKey(stringCount))
            {
                return _defaultTunings[stringCount];
            }

            return null;
        }

        /// <summary>
        /// Gets a list of all tuning presets for a given stirng count. 
        /// </summary>
        /// <param name="stringCount">The string count. </param>
        /// <returns>The list of known tunings for the given string count or an empty list if the string count is not defined. </returns>
        public static FastList<Tuning> GetPresetsFor(int stringCount)
        {
            switch (stringCount)
            {
                case 7:
                    return _sevenStrings;
                case 6:
                    return _sixStrings;
                case 5:
                    return _fiveStrings;
                case 4:
                    return _fourStrings;
            }

            return new FastList<Tuning>();
        }

        static Tuning()
        {
            Initialize();
        }

        private static void Initialize()
        {
            _sevenStrings = new FastList<Tuning>();
            _sixStrings = new FastList<Tuning>();
            _fiveStrings = new FastList<Tuning>();
            _fourStrings = new FastList<Tuning>();
            _defaultTunings = new FastDictionary<int, Tuning>();

            _defaultTunings[7] = new Tuning("Guitar 7 strings",
                new[]
                {
                    64, 59, 55, 50, 45, 40, 35
                },
                true);
            _sevenStrings.Add(_defaultTunings[7]);

            _defaultTunings[6] = new Tuning("Guitar Standard Tuning",
                new[]
                {
                    64, 59, 55, 50, 45, 40
                },
                true);
            _sixStrings.Add(_defaultTunings[6]);

            _sixStrings.Add(new Tuning("Guitar Tune down ½ step",
                new[]
                {
                    63, 58, 54, 49, 44, 39
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Tune down 1 step",
                new[]
                {
                    62, 57, 53, 48, 43, 38
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Tune down 2 step",
                new[]
                {
                    60, 55, 51, 46, 41, 36
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Dropped D Tuning",
                new[]
                {
                    64, 59, 55, 50, 45, 38
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Dropped D Tuning variant",
                new[]
                {
                    64, 57, 55, 50, 45, 38
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Double Dropped D Tuning",
                new[]
                {
                    62, 59, 55, 50, 45, 38
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Dropped E Tuning",
                new[]
                {
                    66, 61, 57, 52, 47, 40
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Dropped C Tuning",
                new[]
                {
                    62, 57, 53, 48, 43, 36
                },
                false));

            _sixStrings.Add(new Tuning("Guitar Open C Tuning",
                new[]
                {
                    64, 60, 55, 48, 43, 36
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open Cm Tuning",
                new[]
                {
                    63, 60, 55, 48, 43, 36
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open C6 Tuning",
                new[]
                {
                    64, 57, 55, 48, 43, 36
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open Cmaj7 Tuning",
                new[]
                {
                    64, 59, 55, 52, 43, 36
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open D Tuning",
                new[]
                {
                    62, 57, 54, 50, 45, 38
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open Dm Tuning",
                new[]
                {
                    62, 57, 53, 50, 45, 38
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open D5 Tuning",
                new[]
                {
                    62, 57, 50, 50, 45, 38
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open D6 Tuning",
                new[]
                {
                    62, 59, 54, 50, 45, 38
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open Dsus4 Tuning",
                new[]
                {
                    62, 57, 55, 50, 45, 38
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open E Tuning",
                new[]
                {
                    64, 59, 56, 52, 47, 40
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open Em Tuning",
                new[]
                {
                    64, 59, 55, 52, 47, 40
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open Esus11 Tuning",
                new[]
                {
                    64, 59, 55, 52, 45, 40
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open F Tuning",
                new[]
                {
                    65, 60, 53, 48, 45, 41
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open G Tuning",
                new[]
                {
                    62, 59, 55, 50, 43, 38
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open Gm Tuning",
                new[]
                {
                    62, 58, 55, 50, 43, 38
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open G6 Tuning",
                new[]
                {
                    64, 59, 55, 50, 43, 38
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open Gsus4 Tuning",
                new[]
                {
                    62, 60, 55, 50, 43, 38
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open A Tuning",
                new[]
                {
                    64, 61, 57, 52, 45, 40
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Open Am Tuning",
                new[]
                {
                    64, 60, 57, 52, 45, 40
                },
                false));
            _sixStrings.Add(new Tuning("Guitar Nashville Tuning",
                new[]
                {
                    64, 59, 67, 62, 57, 52
                },
                false));
            _sixStrings.Add(new Tuning("Bass 6 Strings Tuning",
                new[]
                {
                    48, 43, 38, 33, 28, 23
                },
                false));
            _sixStrings.Add(new Tuning("Lute or Vihuela Tuning",
                new[]
                {
                    64, 59, 54, 50, 45, 40
                },
                false));

            _defaultTunings[5] = new Tuning("Bass 5 Strings Tuning",
                new[]
                {
                    43, 38, 33, 28, 23
                },
                true);
            _fiveStrings.Add(_defaultTunings[5]);
            _fiveStrings.Add(new Tuning("Banjo Dropped C Tuning",
                new[]
                {
                    62, 59, 55, 48, 67
                },
                false));
            _fiveStrings.Add(new Tuning("Banjo Open D Tuning",
                new[]
                {
                    62, 57, 54, 50, 69
                },
                false));
            _fiveStrings.Add(new Tuning("Banjo Open G Tuning",
                new[]
                {
                    62, 59, 55, 50, 67
                },
                false));
            _fiveStrings.Add(new Tuning("Banjo G Minor Tuning",
                new[]
                {
                    62, 58, 55, 50, 67
                },
                false));
            _fiveStrings.Add(new Tuning("Banjo G Modal Tuning",
                new[]
                {
                    62, 57, 55, 50, 67
                },
                false));

            _defaultTunings[4] = new Tuning("Bass Standard Tuning",
                new[]
                {
                    43, 38, 33, 28
                },
                true);
            _fourStrings.Add(_defaultTunings[4]);
            _fourStrings.Add(new Tuning("Bass Tune down ½ step",
                new[]
                {
                    42, 37, 32, 27
                },
                false));
            _fourStrings.Add(new Tuning("Bass Tune down 1 step",
                new[]
                {
                    41, 36, 31, 26
                },
                false));
            _fourStrings.Add(new Tuning("Bass Tune down 2 step",
                new[]
                {
                    39, 34, 29, 24
                },
                false));
            _fourStrings.Add(new Tuning("Bass Dropped D Tuning",
                new[]
                {
                    43, 38, 33, 26
                },
                false));
            _fourStrings.Add(new Tuning("Ukulele C Tuning",
                new[]
                {
                    45, 40, 36, 43
                },
                false));
            _fourStrings.Add(new Tuning("Ukulele G Tuning",
                new[]
                {
                    52, 47, 43, 38
                },
                false));
            _fourStrings.Add(new Tuning("Mandolin Standard Tuning",
                new[]
                {
                    64, 57, 50, 43
                },
                false));
            _fourStrings.Add(new Tuning("Mandolin or Violin Tuning",
                new[]
                {
                    76, 69, 62, 55
                },
                false));
            _fourStrings.Add(new Tuning("Viola Tuning",
                new[]
                {
                    69, 62, 55, 48
                },
                false));
            _fourStrings.Add(new Tuning("Cello Tuning",
                new[]
                {
                    57, 50, 43, 36
                },
                false));
        }

        /// <summary>
        /// Tries to find a known tuning by a given list of tuning values. 
        /// </summary>
        /// <param name="strings">The values defining the tuning. </param>
        /// <returns>The known tuning. </returns>
        public static Tuning FindTuning(int[] strings)
        {
            var tunings = GetPresetsFor(strings.Length);
            for (int t = 0, tc = tunings.Count; t < tc; t++)
            {
                var tuning = tunings[t];
                var equals = true;
                for (int i = 0, j = strings.Length; i < j; i++)
                {
                    if (strings[i] != tuning.Tunings[i])
                    {
                        equals = false;
                        break;
                    }
                }

                if (equals)
                {
                    return tuning;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets or sets whether this is the standard tuning for this number of strings. 
        /// </summary>
        public bool IsStandard { get; set; }

        /// <summary>
        /// Gets or sets the name of the tuning. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the values for each string of the instrument. 
        /// </summary>
        public int[] Tunings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tuning"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="tuning">The tuning.</param>
        /// <param name="isStandard">if set to <c>true</c> [is standard].</param>
        public Tuning(string name, int[] tuning, bool isStandard)
        {
            IsStandard = isStandard;
            Name = name;
            Tunings = tuning;
        }
    }
}
