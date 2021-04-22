using System;
using System.IO;
using System.Security;
using BardMusicPlayer.Config.JsonSettings.Inline;
using BardMusicPlayer.Config.JsonSettings.Modulation;

namespace BardMusicPlayer.Config.JsonSettings.Fluent
{
    public static class FluentJsonSettings
    {
        internal static T _withFileName<T>(this T _instance, string filename, bool throwless = false) where T : JsonSettings
        {
            _instance.FileName = JsonSettings.ResolvePath(_instance, filename, throwless);
            return _instance;
        }

        public static T WithFileName<T>(this T _instance, string filename) where T : JsonSettings
        {
            return _withFileName(_instance, filename);
        }

        public static T WithFileName<T>(this T _instance, FileInfo filename) where T : JsonSettings
        {
            return _instance.WithFileName(filename?.FullName);
        }

        /// <summary>
        ///     Load custom made module.
        /// </summary>
        /// <param name="_instance">Self</param>
        /// <param name="module">The module instance</param>
        /// <returns>Self</returns>
        public static T WithModule<T>(this T _instance, Module module) where T : JsonSettings
        {
            _instance.Modulation.Attach(module);
            return _instance;
        }

        /// <summary>
        ///     Loads a freshly constructed module with the following arguments.
        /// </summary>
        /// <param name="_instance">Self</param>
        /// <param name="args">The arguments to fit into a constructor.</param>
        /// <returns>Self</returns>
        public static T WithModule<T, TMod>(this T _instance, params object[] args) where TMod : Module where T : JsonSettings
        {
            var t = (Module)typeof(TMod).CreateInstance(args);

            return _instance.WithModule(t);
        }

        /// <summary>
        ///     Fluently do something with self.
        /// </summary>
        /// <returns>Self</returns>
        public static T WithDefaultValues<T>(this T _instance, Action<T> @do) where T : JsonSettings
        {
            if (@do == null) throw new ArgumentNullException(nameof(@do));
            @do(_instance);
            return _instance;
        }

        /// <summary>
        ///     Attaches <see cref="RijndaelModule"/> that uses custom Rijndael256 library.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_instance"></param>
        /// <param name="password">A fetcher/getter/generator/action-pointer to the password source</param>
        /// <returns>Self</returns>
        public static T WithEncryption<T>(this T _instance, string password) where T : JsonSettings
        {
            return _instance.WithEncryption(password?.ToSecureString());
        }

        /// <summary>
        ///     Attaches <see cref="RijndaelModule"/> that uses custom Rijndael256 library.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_instance"></param>
        /// <param name="password">A fetcher/getter/generator/action-pointer to the password source</param>
        /// <returns>Self</returns>
        public static T WithEncryption<T>(this T _instance, SecureString password) where T : JsonSettings
        {
            return _instance.WithModule<T, RijndaelModule>(password);
        }

        /// <summary>
        ///     Attaches <see cref="RijndaelModule"/> that uses custom Rijndael256 library.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_instance"></param>
        /// <param name="password">A fetcher/getter/generator/action-pointer to the password source</param>
        /// <returns>Self</returns>
        public static T WithEncryption<T>(this T _instance, Func<string> password) where T : JsonSettings
        {
            return _instance.WithEncryption(() => password?.Invoke()?.ToSecureString());
        }

        /// <summary>
        ///     Attaches <see cref="RijndaelModule"/> that uses custom Rijndael256 library.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_instance"></param>
        /// <param name="password">A fetcher/getter/generator/action-pointer to the password source</param>
        /// <returns>Self</returns>
        public static T WithEncryption<T>(this T _instance, Func<SecureString> password) where T : JsonSettings
        {
            return _instance.WithModule<T, RijndaelModule>(password);
        }

        /// <summary>
        ///     Attaches <see cref="RijndaelModule"/> that uses custom Rijndael256 library.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_instance"></param>
        /// <param name="password">A fetcher/getter/generator/action-pointer to the password source</param>
        /// <returns>Self</returns>
        public static T WithEncryption<T>(this T _instance, Func<T, string> password) where T : JsonSettings
        {
            return _instance.WithEncryption(() => password?.Invoke(_instance));
        }

        /// <summary>
        ///     Attaches <see cref="RijndaelModule"/> that uses custom Rijndael256 library.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_instance"></param>
        /// <param name="password">A fetcher/getter/generator/action-pointer to the password source</param>
        /// <returns>Self</returns>
        public static T WithEncryption<T>(this T _instance, Func<T, SecureString> password) where T : JsonSettings
        {
            return _instance.WithModule<T, RijndaelModule>((Func<SecureString>)(() => password?.Invoke(_instance)));
        }

        public static T WithBase64<T>(this T _instance) where T : JsonSettings
        {
            return _instance.WithModule<T, Base64Module>();
        }

        /// <summary>
        ///     Performs Load on the JsonSettings, used after calling <see cref="JsonSettings.Configure"/> to perform a load at the end.
        /// </summary>
        /// <param name="_instance">The instance of type JsonSettings</param>
        /// <param name="filename">Specific file to load (ignores <see cref="JsonSettings.FileName"/>)</param>
        /// <returns>Self</returns>
        public static T LoadNow<T>(this T _instance, string filename) where T : JsonSettings
        {
            _instance.Load(filename);
            return _instance;
        }

        /// <summary>
        ///     Performs Load on the JsonSettings, used after calling <see cref="JsonSettings.Configure"/> to perform a load at the end.<br></br>
        ///     Uses <see cref="JsonSettings.FileName"/>.
        /// </summary>
        /// <param name="_instance">The instance of type JsonSettings</param>
        /// <returns>Self</returns>
        public static T LoadNow<T>(this T _instance) where T : JsonSettings
        {
            _instance.Load();
            return _instance;
        }
    }
}