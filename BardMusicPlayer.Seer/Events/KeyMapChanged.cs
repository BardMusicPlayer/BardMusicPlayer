/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Collections.ObjectModel;
using BardMusicPlayer.Quotidian.Enums;
using BardMusicPlayer.Quotidian.Structs;

namespace BardMusicPlayer.Seer.Events
{
    public sealed class KeyMapChanged : SeerEvent
    {
        internal KeyMapChanged(EventSource readerBackendType, IDictionary<Instrument, Keys> instrumentKeys, IDictionary<InstrumentTone, Keys> instrumentToneKeys, IDictionary<NavigationMenuKey, Keys> navigationMenuKeys, IDictionary<InstrumentToneMenuKey, Keys> instrumentToneMenuKeys, IDictionary<NoteKey, Keys> noteKeys) : base(readerBackendType)
        {
            EventType = GetType();
            InstrumentKeys = new ReadOnlyDictionary<Instrument, Keys>(instrumentKeys);
            InstrumentToneKeys = new ReadOnlyDictionary<InstrumentTone, Keys>(instrumentToneKeys);
            NavigationMenuKeys = new ReadOnlyDictionary<NavigationMenuKey, Keys>(navigationMenuKeys);
            InstrumentToneMenuKeys = new ReadOnlyDictionary<InstrumentToneMenuKey, Keys>(instrumentToneMenuKeys);
            NoteKeys = new ReadOnlyDictionary<NoteKey, Keys>(noteKeys);
        }

        public IReadOnlyDictionary<Instrument, Keys> InstrumentKeys { get; }
        public IReadOnlyDictionary<InstrumentTone, Keys> InstrumentToneKeys { get; }
        public IReadOnlyDictionary<NavigationMenuKey, Keys> NavigationMenuKeys { get; }
        public IReadOnlyDictionary<InstrumentToneMenuKey, Keys> InstrumentToneMenuKeys { get; }
        public IReadOnlyDictionary<NoteKey, Keys> NoteKeys { get; }

        public override bool IsValid() => true;
    }
}