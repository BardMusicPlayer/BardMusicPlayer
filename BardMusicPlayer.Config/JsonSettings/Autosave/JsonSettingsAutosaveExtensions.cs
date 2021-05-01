/*
 * Copyright(c) 2017 Belash
 * Licensed under the MIT license. See https://github.com/Nucs/JsonSettings/blob/master/LICENSE for full license information.
 */

using System;
using System.Diagnostics;
using System.Linq;
using BardMusicPlayer.Config.JsonSettings.Interface;
using Castle.DynamicProxy;

namespace BardMusicPlayer.Config.JsonSettings.Autosave
{
    public static class JsonSettingsAutosaveExtensions
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

        public static ISettings EnableIAutosave<ISettings>(this JsonSettings settings) where ISettings : class
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            _generator = _generator ?? new ProxyGenerator();

            if (!(settings is ISettings))
                throw new InvalidCastException($"Settings class '{settings.GetType().FullName}' does not implement interface '{typeof(ISettings).FullName}'");

            return _generator.CreateInterfaceProxyWithTarget<ISettings>((ISettings)(object)settings, new JsonSettingsInterceptor(settings));
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