/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Melanchall.DryWetMidi.Interaction;

#pragma warning disable 1998
namespace BardMusicPlayer.Notate.Processor.Utilities
{
    internal static partial class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceNotesDictionary"></param>
        /// <returns></returns>
        internal static async Task<IEnumerable<Note>> ConcatNoteDictionaryToList(this Dictionary<int, Dictionary<long, Note>> sourceNotesDictionary) =>
            sourceNotesDictionary.SelectMany(note => note.Value).Select(note => note.Value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceNotesDictionary"></param>
        /// <returns></returns>
        internal static async Task<IEnumerable<Note>> ConcatNoteDictionaryToList(this Task<Dictionary<int, Dictionary<long, Note>>> sourceNotesDictionary) =>
            await ConcatNoteDictionaryToList(await sourceNotesDictionary);
    }
}
#pragma warning restore 1998