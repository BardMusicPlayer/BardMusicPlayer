/*
 * Copyright(c) 2017 Belash
 * Licensed under the MIT license. See https://github.com/Nucs/JsonSettings/blob/master/LICENSE for full license information.
 */

using System;

namespace BardMusicPlayer.Config.JsonSettings.Modulation
{

    /// <summary>
    ///     Will convert text to base64, not pure json.
    /// </summary>
    public class Base64Module : Module
    {

        public Base64Module() { }

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

        protected void _Encrypt(ref byte[] data)
        {
            data = JsonSettings.Encoding.GetBytes(Convert.ToBase64String(data));
        }

        protected void _Decrypt(ref byte[] data)
        {
            data = Convert.FromBase64String(JsonSettings.Encoding.GetString(data));
        }


    }
}