using System.Collections.Generic;

namespace BardMusicPlayer.Quotidian.Enums
{
    public class KeyTranslation
    {
        public static Dictionary<string, Keys> ASCIIToGame = new Dictionary<string, Keys>
        {
            { "1", Keys.D1 },
            { "2", Keys.D2 },
            { "3", Keys.D3 },
            { "4", Keys.D4 },
            { "5", Keys.D5 },
            { "6", Keys.D6 },
            { "7", Keys.D7 },
            { "8", Keys.D8 },
            { "9", Keys.D9 },
            { "0", Keys.D0 },
            { "A", Keys.A },
            { "D", Keys.D },
            { "E", Keys.E },
            { "Q", Keys.Q },
            { "S", Keys.S },
            { "W", Keys.W },
            { " ", Keys.Space }
        };
    }
}
