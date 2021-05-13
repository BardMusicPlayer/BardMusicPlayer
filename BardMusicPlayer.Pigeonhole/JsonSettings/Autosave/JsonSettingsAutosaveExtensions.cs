/*
 * Copyright(c) 2017 Eli Belash
 * Licensed under the MIT license. See https://github.com/Nucs/JsonSettings/blob/master/LICENSE for full license information.
 */

using System;
using System.Diagnostics;
using System.Linq;
using Castle.DynamicProxy;

namespace BardMusicPlayer.Pigeonhole.JsonSettings.Autosave
{
    internal static class JsonSettingsAutosaveExtensions
    {
        private static readonly string[] _jsonSettingsAbstractionVirtuals = { "FileName" };

        private static ProxyGenerator _generator;

        /// <summary>
        ///     Enables automatic saving when changing any <b>virtual properties</b>.
        /// </summary>
        /// <typeparam name="TSettings">A settings class implementing <see cref="JsonSettings"/></typeparam>
        /// <param name="settings">The settings class to wrap in a proxy.</param>
        /// <returns></returns>
        public static TSettings EnableAutosave<TSettings>(this TSettings settings) where TSettings : JsonSettings
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            //if it doesn't contain any virtual methods, notify developer about it.
            if (!settings.GetType().GetProperties().Where(p => _jsonSettingsAbstractionVirtuals.All(av => !p.Name.Equals(av))).Any(p => p.GetGetMethod().IsVirtual))
            {
                var msg = $"JsonSettings: During proxy creation of {settings.GetType().Name}, no virtual properties were found which will make Autosaving redundant.";
                Debug.WriteLine(msg);
                if (Debugger.IsAttached)
                    Console.Error.WriteLine(msg);
                return settings;
            }

            _generator = _generator ?? new ProxyGenerator();
            return _generator.CreateClassProxyWithTarget<TSettings>(settings, new JsonSettingsInterceptor((JsonSettings)(object)settings));
        }

        /// <summary>
        ///     Intercepts 
        /// </summary>
        [Serializable]
        public class JsonSettingsInterceptor : IInterceptor
        {
            private readonly ISavable _settings;

            public JsonSettingsInterceptor(JsonSettings settings)
            {
                _settings = settings;
                //_filename = filename;
            }

            public void Intercept(IInvocation invocation)
            {
                invocation.Proceed();
                if (invocation.Method.Name.StartsWith("set_", StringComparison.Ordinal))
                {
                    var propname = invocation.Method.Name.Substring(4);
                    if (_jsonSettingsAbstractionVirtuals.Any(av => av == propname))
                        return;

                    //save.
                    _settings.Save();
                }
            }
        }
    }
}