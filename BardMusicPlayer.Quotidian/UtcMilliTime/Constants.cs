/*
 * Copyright(c) 2019 John Kusumi
 * Licensed under the MIT license. See https://github.com/JPKusumi/UtcMilliTime/blob/master/LICENSE for full license information.
 */

namespace BardMusicPlayer.Quotidian.UtcMilliTime
{
    public static class Constants
    {
        public const short bytes_per_buffer = 48;
        public const short udp_port_number = 123;
        public const short second_milliseconds = 1000;
        public const short three_seconds = 3000;
        public const short dotnet_ticks_per_millisecond = 10000;
        public const int minute_milliseconds = 60000;
        public const int hour_milliseconds = 3600000;
        public const int day_milliseconds = 86400000;
        public const long ntp_to_unix_milliseconds = 2208988800000;
        public const long dotnet_to_unix_milliseconds = 62135596800000;
        public const string fallback_server = "time.cloudflare.com";
        public const string iso_8601_without_milliseconds = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";
        public const string iso_8601_with_milliseconds = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
    }
}
