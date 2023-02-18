/*
 * Copyright(c) 2022 MoogleTroupe, GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BardMusicPlayer.Quotidian.Structs;
using Polly;

namespace BardMusicPlayer.Siren;

internal static class VSTLoader
{
    internal static async void UpdateAndLoadVST()
    {
        try
        {
            if (!Directory.Exists(BmpSiren.Instance._vstLocation)) Directory.CreateDirectory(BmpSiren.Instance._vstLocation);

            if (File.Exists(BmpSiren.Instance._vstLocation + @"\version"))
            {
                using TextReader reader = File.OpenText(BmpSiren.Instance._vstLocation + @"\version");
                _version = int.Parse(reader.ReadLine());
            }
            int version = int.Parse(await GetStringFromUrl(_baseURL + @"/version"));

            if (version > _version)
            {
                foreach (Instrument instrument in Instrument.All)
                {
                    Debug.WriteLine("Downloading VST file " + _baseURL + @"/vst_" + @instrument.Name.ToLower() + @".sf2");
                    using var sf2writer = new FileStream(BmpSiren.Instance._vstLocation + @"\vst_" + @instrument.Name.ToLower() + @".sf2", FileMode.Create, FileAccess.Write);
                    byte[] sf2 = await GetFileFromUrl(_baseURL + @"/vst_" + @instrument.Name.ToLower() + @".sf2");
                    sf2writer.Write(sf2, 0, sf2.Length);
                }

                _version = version;
                using var writer = new StreamWriter(BmpSiren.Instance._vstLocation + @"\version");
                writer.Write(_version.ToString());
            }

            foreach (Instrument instrument in Instrument.All) BmpSiren.Instance._player.LoadSoundFont(File.ReadAllBytes(BmpSiren.Instance._vstLocation + @"\vst_" + @instrument.Name.ToLower() + @".sf2"), true);
            BmpSiren.Instance._vstDownloaded = true;
        }
        catch (Exception)
        {
            // TODO: Create a solution for failed vst downloading
        }
    }
    private static readonly string _baseURL = "https://dl.bardmusicplayer.com/bmp/vst";
    private static int _version = 0;

    private static async Task<string> GetStringFromUrl(string url)
    {
        var httpClient = new HttpClient();
        return await Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: i => TimeSpan.FromMilliseconds(300))
            .ExecuteAsync(async () =>
            {
                using var httpResponse = await httpClient.GetAsync(url);
                httpResponse.EnsureSuccessStatusCode();
                return await httpResponse.Content.ReadAsStringAsync();
            });
    }

    private static async Task<byte[]> GetFileFromUrl(string url)
    {
        var httpClient = new HttpClient();
        return await Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: i => TimeSpan.FromMilliseconds(300))
            .ExecuteAsync(async () =>
            {
                using var httpResponse = await httpClient.GetAsync(url);
                httpResponse.EnsureSuccessStatusCode();
                return await httpResponse.Content.ReadAsByteArrayAsync();
            });
    }
}