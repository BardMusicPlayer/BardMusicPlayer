/*
 * Copyright(c) 2017 Belash
 * Licensed under the MIT license. See https://github.com/Nucs/JsonSettings/blob/master/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace BardMusicPlayer.Config.JsonSettings.Modulation
{
    /// <summary>
    ///     A subclass to manage modulation of a JsonSettings
    /// </summary>
    public class ModuleSocket : ISocket, IDisposable
    {
        private JsonSettings _settings { get; set; }

        public ModuleSocket(JsonSettings settings)
        {
            _settings = settings;
        }

#if NET40
        public ReadOnlyCollection<Module> Modules {
#else
        public IReadOnlyList<Module> Modules
        {
#endif
            get
            {
                lock (_modules)
                    return _modules.ToList().AsReadOnly();
            }
        }

        public bool IsAttached(Func<Module, bool> checker)
        {
            return Modules.Any(checker);
        }

        public bool IsAttachedOfType<T>() where T : Module
        {
            return IsAttachedOfType(typeof(T));
        }

        public bool IsAttachedOfType(Type t)
        {
            return IsAttached(m => m.GetType() == t);
        }

        protected readonly List<Module> _modules = new List<Module>();

        public void Attach(Module t)
        {
            if (_isdisposed)
                throw new ObjectDisposedException("Can't attach, this object is already disposed.");
            t.Attach(_settings);
            lock (_modules)
                _modules.Add(t);
        }

        public void Deattach(Module t)
        {
            t.Deattach(_settings);
            lock (_modules)
                _modules.Remove(t);
        }

        /// <summary>
        ///     Will invoke attach to a freshly new object of type <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T">A module class</typeparam>
        /// <param name="args">The arguments that'll be passed to the constructor</param>
        public T Attach<T>(params object[] args) where T : Module
        {
            var t = (Module)Activator.CreateInstance(typeof(T), args);
            Attach(t);
            return (T)t;
        }

        private bool _isdisposed = false;

        public void Dispose()
        {
            if (_isdisposed)
                return;
            _isdisposed = true;
            foreach (var module in _modules.ToArray())
            {
                module.Dispose();
            }
            _settings = null;
        }
    }
}