/*
 * Copyright(c) 2017 Belash
 * Licensed under the MIT license. See https://github.com/Nucs/JsonSettings/blob/master/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;

namespace BardMusicPlayer.Config.JsonSettings.Modulation
{
    /// <summary>
    ///     A class that can be attached to and deattached from with <see cref="Module"/>s.
    /// </summary>
    public interface ISocket
    {
        /// <summary>
        ///     Attach a module to current socket.
        /// </summary>
        void Attach(Module t);
        /// <summary>
        ///     Deattach a module from any socket it was attached to.<br></br>This is merely a shortcut to <see cref="Module.Deattach"/>.
        /// </summary>
        void Deattach(Module t);
#if NET40
        ReadOnlyCollection<Module> Modules { get; }
#else
        IReadOnlyList<Module> Modules { get; }
#endif
        bool IsAttached(Func<Module, bool> checker);
        bool IsAttachedOfType<T>() where T : Module;
        bool IsAttachedOfType(Type t);

    }
}