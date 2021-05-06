/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models
{
    internal class Signature {
        private Regex _regularExpress;

        public Signature() {
            Key = string.Empty;
            Value = string.Empty;
            RegularExpress = null;
            SigScanAddress = IntPtr.Zero;
            PointerPath = null;
        }

        internal MemoryHandler MemoryHandler { get; set; }

        public bool ASMSignature { get; set; }

        public string Key { get; set; }

        [JsonIgnore]
        public int Offset => Value.Length / 2;

        public List<long> PointerPath { get; set; }

        public Regex RegularExpress {
            get => _regularExpress;
            set {
                if (value != null) _regularExpress = value;
            }
        }

        [JsonIgnore]
        public IntPtr SigScanAddress { get; set; }

        public string Value { get; set; }

        public static implicit operator IntPtr(Signature signature) {
            return signature.GetAddress();
        }

        public IntPtr GetAddress() {
            IntPtr baseAddress;
            var IsASMSignature = false;
            if (SigScanAddress != IntPtr.Zero) {
                baseAddress = SigScanAddress; // Scanner should have already applied the base offset
                if (ASMSignature) IsASMSignature = true;
            }
            else {
                if (PointerPath == null || PointerPath.Count == 0) {
                    return IntPtr.Zero;
                }

                baseAddress = MemoryHandler.GetStaticAddress(0);
            }

            if (PointerPath == null || PointerPath.Count == 0) {
                return baseAddress;
            }

            return MemoryHandler.ResolvePointerPath(PointerPath, baseAddress, IsASMSignature);
        }
	}
}
