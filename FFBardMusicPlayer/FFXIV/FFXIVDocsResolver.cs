/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Diagnostics;
using FFBardMusicPlayer.FFXIV.MyDocumentsResolver;
using ServiceStack;

namespace FFBardMusicPlayer.FFXIV
{
    internal class FFXIVDocsResolver
    {
        internal static string GetPath()
        {
            var path = "";
            foreach (var process in Process.GetProcessesByName("ffxiv_dx11"))
            {
                try
                {
                    path = new KnownFolder(KnownFolderType.Documents, process.WindowsIdentity()).Path;
                }
                catch (Exception)
                {
                    // Ignore.
                }
            }

            return path.IsNullOrEmpty() ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : path;
        }
    }
}
