using System.Collections.Generic;
using Stylet;

namespace BardMusicPlayer.Ui.Utilities
{
    public static class EnumerableExtensions
    {
        public static BindableCollection<T> ToBindableCollection<T>(this IEnumerable<T> enumerable) =>
            new(enumerable);
    }
}