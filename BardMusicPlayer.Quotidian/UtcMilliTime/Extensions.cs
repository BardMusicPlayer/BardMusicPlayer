/*
 * Copyright(c) 2019 John Kusumi
 * Licensed under the MIT license. See https://github.com/JPKusumi/UtcMilliTime/blob/master/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Quotidian.UtcMilliTime
{
    public static class Extensions
    {

        /// <summary>
        /// Transform a UtcMilliTime Int64 to a string in ISO-8601 format.
        /// </summary>
        /// <param name="timestamp">UtcMilliTime</param>
        /// <param name="suppressMilliseconds">boolean</param>
        /// <returns>string like 2019-08-10T22:08:14.102Z</returns>
        public static string ToIso8601String(this long timestamp, bool suppressMilliseconds = false)
        {
            if (suppressMilliseconds) return timestamp.ToUtcDateTime().ToString(Constants.iso_8601_without_milliseconds);
            return timestamp.ToUtcDateTime().ToString(Constants.iso_8601_with_milliseconds);
        }
        /// <summary>
        /// Transform a DateTime to UtcMilliTime. Fractional milliseconds are truncated
        /// </summary>
        /// <param name="given">DateTime</param>
        /// <returns>long</returns>
        public static long ToUtcMilliTime(this DateTime given) => (given.ToUniversalTime().Ticks / Constants.dotnet_ticks_per_millisecond) - Constants.dotnet_to_unix_milliseconds;
        /// <summary>
        /// Transform a DateTimeOffset to UtcMilliTime. Fractional milliseconds are truncated
        /// </summary>
        /// <param name="given">DateTimeOffset</param>
        /// <returns>long</returns>
        public static long ToUtcMilliTime(this DateTimeOffset given) => given.ToUnixTimeMilliseconds();
        /// <summary>
        /// Transform a TimeSpan interval to UtcMilliTime. Fractional milliseconds are truncated
        /// </summary>
        /// <param name="given">TimeSpan</param>
        /// <returns>long</returns>
        public static long ToUtcMilliTime(this TimeSpan given) => (long)given.TotalMilliseconds;
        /// <summary>
        /// Transform UnixTimeSeconds to UtcMilliTime. Multiplies by 1000 hence adds 3 digits
        /// </summary>
        /// <param name="unixtime">UnixTimeSeconds</param>
        /// <returns>long</returns>
        public static long ToUtcMilliTime(this long unixtime) => unixtime * 1000;
        /// <summary>
        /// Truncate the milliseconds part of UtcMilliTime. Divides by 1000 hence drops 3 digits
        /// </summary>
        /// <param name="timestamp">UtcMilliTime</param>
        /// <returns>long</returns>
        public static long ToUnixTime(this long timestamp) => timestamp / 1000;
        /// <summary>
        /// The whole number of days in an interval found in UtcMilliTime
        /// </summary>
        /// <param name="interval">UtcMilliTime</param>
        /// <returns>int</returns>
        public static int IntervalDays(this long interval) => (int)(interval / Constants.day_milliseconds);
        /// <summary>
        /// Whole hours in the remainder after days are removed from an interval
        /// </summary>
        /// <param name="interval">UtcMilliTime</param>
        /// <returns>int</returns>
        public static int IntervalHoursPart(this long interval) => (int)(interval % Constants.day_milliseconds) / Constants.hour_milliseconds;
        /// <summary>
        /// Whole minutes in the remainder after days and hours are removed from an interval
        /// </summary>
        /// <param name="interval">UtcMilliTime</param>
        /// <returns>int</returns>
        public static int IntervalMinutesPart(this long interval) => (int)(interval - (interval.IntervalDays() * Constants.day_milliseconds + interval.IntervalHoursPart() * Constants.hour_milliseconds)) / Constants.minute_milliseconds;
        /// <summary>
        /// Whole seconds in the remainder after removing days, hours, and minutes from an interval
        /// </summary>
        /// <param name="interval">UtcMilliTime</param>
        /// <returns>int</returns>
        public static int IntervalSecondsPart(this long interval) => (int)(interval - (interval.IntervalDays() * Constants.day_milliseconds + interval.IntervalHoursPart() * Constants.hour_milliseconds + interval.IntervalMinutesPart() * Constants.minute_milliseconds)) / Constants.second_milliseconds;
        /// <summary>
        /// The subsecond portion (rightmost 3 digits)
        /// </summary>
        /// <param name="timestamp">UtcMilliTime</param>
        /// <returns>short</returns>
        public static short MillisecondPart(this long timestamp) => (short)(timestamp - timestamp / 1000 * 1000);
        /// <summary>
        /// Transform a UtcMilliTime Int64 to a .NET DateTime of Kind = Utc
        /// </summary>
        /// <param name="timestamp">long</param>
        /// <returns>DateTime</returns>
        public static DateTime ToUtcDateTime(this long timestamp) => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timestamp);
        /// <summary>
        /// Transform a UtcMilliTime Int64 to a .NET DateTime of Kind = Local
        /// </summary>
        /// <param name="timestamp">long</param>
        /// <returns>DateTime</returns>
        public static DateTime ToLocalDateTime(this long timestamp) => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timestamp).ToLocalTime();
        /// <summary>
        /// Transform a UtcMilliTime Int64 to a .NET DateTimeOffset, still in UTC, offset=0
        /// </summary>
        /// <param name="timestamp">UtcMilliTime</param>
        /// <returns>DateTimeOffset</returns>
        public static DateTimeOffset ToDateTimeOffset(this long timestamp) => new DateTimeOffset(timestamp.ToUtcDateTime());
        /// <summary>
        /// Transform an interval from UtcMilliTime Int64 to a .NET TimeSpan. If interval is an absolute date, TimeSpan will be from 1970 to then
        /// </summary>
        /// <param name="interval">UtcMilliTime</param>
        /// <returns>TimeSpan</returns>
        public static TimeSpan ToTimeSpan(this long interval) => new TimeSpan(interval * Constants.dotnet_ticks_per_millisecond);
        /// <summary>
        /// Extension to fire and forget an asynchronous task
        /// </summary>
        /// <param name="task">The Task-returning async method being started</param>
        /// <param name="continueOnCapturedContext">Optional boolean for ConfigureAwait</param>
        /// <param name="onException">Optional delegate for exception handling</param>
#pragma warning disable RECS0165
        public static async void SafeFireAndForget(this System.Threading.Tasks.Task task, bool continueOnCapturedContext = true, Action<Exception> onException = null)
#pragma warning restore RECS0165
        {
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext);
            }
            catch (Exception ex) when (onException != null)
            {
                onException(ex);
            }
        }
    }
}
