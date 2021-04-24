using System;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;

namespace BardMusicPlayer.Updater.Util
{
    internal class Downloader
    {
        internal async Task<string> GetStringFromUrl(string url)
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

        internal async Task<byte[]> GetFileFromUrl(string url)
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


    /* 
    internal class Downloader : IDisposable
    {
        private readonly string _url;
        private readonly IProgress<ProgressChangedEvent> _progress;
        private readonly CancellationToken _cancellationToken;

        internal Downloader(string url, CancellationToken cancellationToken = default)
        {
            _url = url;
            _progress = new Progress<ProgressChangedEvent>(NotifyProgressChanged);
            _cancellationToken = cancellationToken;
        }

        internal struct ProgressChangedEvent
        {
            internal ProgressChangedEvent(float percentage, long currentBytes, long totalBytes)
            {
                Percentage = percentage;
                CurrentBytes = currentBytes;
                TotalBytes = totalBytes;
            }
            internal float Percentage;
            internal long CurrentBytes;
            internal long TotalBytes;
        }
        internal delegate void ProgressChangedHandler(ProgressChangedEvent progressChangedEvent);
        internal event ProgressChangedHandler ProgressChanged;
        private void NotifyProgressChanged(ProgressChangedEvent progressChangedEvent) => ProgressChanged?.Invoke(progressChangedEvent);

        internal async Task<string> GetStringAsync()
        {
            using var streamReader = new StreamReader( await GetStreamAsync() );
            return await streamReader.ReadToEndAsync();
        }
        internal async Task<byte[]> GetBytesAsync() => (await GetStreamAsync()).ToArray();
        internal async Task<MemoryStream> GetStreamAsync()
        {
            using var httpClient = new HttpClient();
            return await Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: i => TimeSpan.FromMilliseconds(300))
                .ExecuteAsync(async () =>
                {
                    using var httpResponse = await httpClient.GetAsync(_url, HttpCompletionOption.ResponseHeadersRead, _cancellationToken);
                    httpResponse.EnsureSuccessStatusCode();
                    var totalBytes = httpResponse.Content.Headers.ContentLength;
                    if (!totalBytes.HasValue) throw new HttpRequestException("Zero Content Length");

                    using var sourceStream = await httpResponse.Content.ReadAsStreamAsync();
                    using MemoryStream destinationStream = new();
                    
                    if (!sourceStream.CanRead)
                        throw new HttpRequestException ("Cannot read stream.");
                    if (!destinationStream.CanWrite)
                        throw new HttpRequestException ("Cannot write stream.");

                    var buffer = new byte[81920];
                    long currentBytes = 0;
                    int bytesRead;
                    while ((bytesRead = await sourceStream.ReadAsync (buffer, 0, buffer.Length, _cancellationToken).ConfigureAwait(false)) != 0)
                    {
                        await destinationStream.WriteAsync (buffer, 0, bytesRead, _cancellationToken).ConfigureAwait(false);
                        currentBytes += bytesRead;
                        _progress.Report(new ProgressChangedEvent(currentBytes / totalBytes.Value * 100f, currentBytes, totalBytes.Value));
                    }

                    destinationStream.Position = 0;
                    return destinationStream;
                });
        }

        ~Downloader() => Dispose();
        public void Dispose()
        {
        }
    }
    */
}
