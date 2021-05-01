/*
 * Copyright(c) 2017 Belash
 * Licensed under the MIT license. See https://github.com/Nucs/JsonSettings/blob/master/LICENSE for full license information.
 */

using System.Security;

namespace BardMusicPlayer.Config.JsonSettings.Interface
{
    public interface IEncryptedSavable : ISavable
    {
        /// <summary>
        ///     The password which is used to encrypt and decrypt the file.
        /// </summary>
        SecureString Password { get; }
    }
}