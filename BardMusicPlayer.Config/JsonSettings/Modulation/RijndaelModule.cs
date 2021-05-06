/*
 * Copyright(c) 2017 Belash
 * Licensed under the MIT license. See https://github.com/Nucs/JsonSettings/blob/master/LICENSE for full license information.
 */

using System;
using System.Security;
using System.Security.Cryptography;
using BardMusicPlayer.Config.JsonSettings.Inline.Rijndael256;
using Rijndael = BardMusicPlayer.Config.JsonSettings.Inline.Rijndael256.Rijndael;

namespace BardMusicPlayer.Config.JsonSettings.Modulation
{
    /// <summary>
    ///     This module encrypts the configuration with Rijndael Algorithm, aka AES256.
    /// </summary>
    /// <remarks>This module uses internal class to perform the encryption and is not publicly exposed.<br></br>The password is stored as <see cref="SecureString"/> in memory.</remarks>
    public class RijndaelModule : Module
    {
        public static readonly SecureString EmptyString = "".ToSecureString();

        private Func<SecureString> _fetcher;

        internal KeySize KeySize { get; set; } = KeySize.Aes256;

        /// <summary>
        ///     The password passed during constructor stored as a <see cref="SecureString"/> in memory.
        /// </summary>
        public SecureString Password
        {
            get => _fetcher?.Invoke();
            set { _fetcher = () => value; }
        }

        public RijndaelModule(string password) : this(password?.ToSecureString()) { }

        public RijndaelModule(SecureString password) : this(() => password) { }

        public RijndaelModule(Func<string> passwordFetcher) : this(() => passwordFetcher?.Invoke()?.ToSecureString()) { }

        public RijndaelModule(Func<SecureString> passwordFetcher)
        {
            _fetcher = () =>
            {
                var ret = passwordFetcher() ?? EmptyString;
                if (!ret.IsReadOnly())
                    ret.MakeReadOnly();
                return ret;
            };
        }

        public override void Attach(JsonSettings socket)
        {
            base.Attach(socket);
            socket.Encrypt += _Encrypt;
            socket.Decrypt += _Decrypt;
        }

        public override void Deattach(JsonSettings socket)
        {
            base.Deattach(socket);
            socket.Encrypt -= _Encrypt;
            socket.Decrypt -= _Decrypt;
        }

        protected void _Encrypt(ref byte[] data) { data = Rijndael.Encrypt(data, Password.ToRawString(), Rng.GenerateRandomBytes(Rijndael.InitializationVectorSize), KeySize); }

        protected void _Decrypt(ref byte[] data)
        {
            try
            {
                data = Rijndael.DecryptBytes(data, Password.ToRawString(), KeySize);
            }
            catch (CryptographicException inner)
            {
                throw new JsonSettingsException("Password appears to be invalid.", inner);
            }
        }
    }
}