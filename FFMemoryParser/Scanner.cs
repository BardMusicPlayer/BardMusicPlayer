// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scanner.cs" company="SyndicatedLife">
//   Copyright(c) 2018 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (http://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Scanner.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// Adapted for BMP

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace FFMemoryParser {

    public sealed class Scanner {
        private const int MemCommit = 0x1000;

        private const int PageExecuteReadwrite = 0x40;

        private const int PageExecuteWritecopy = 0x80;

        private const int PageGuard = 0x100;

        private const int PageNoAccess = 0x01;

        private const int PageReadwrite = 0x04;

        private const int PageWritecopy = 0x08;

        private const int WildCardChar = 63;

        private const int Writable =
            PageReadwrite | PageWritecopy | PageExecuteReadwrite | PageExecuteWritecopy | PageGuard;

        private static Lazy<Scanner> _instance = new Lazy<Scanner>(() => new Scanner());

        public static int FindSuperSignature(byte[] buffer, byte[] pattern) {
            var result = -1;
            if (buffer.Length <= 0 || pattern.Length <= 0 || buffer.Length < pattern.Length) {
                return result;
            }

            for (var i = 0; i <= buffer.Length - pattern.Length; i++) {
                if (buffer[i] != pattern[0]) {
                    continue;
                }

                if (buffer.Length > 1) {
                    var matched = true;
                    for (var y = 1; y <= pattern.Length - 1; y++) {
                        if (buffer[i + y] == pattern[y] || pattern[y] == WildCardChar) {
                            continue;
                        }

                        matched = false;
                        break;
                    }

                    if (!matched) {
                        continue;
                    }

                    result = i;
                    break;
                }

                result = i;
                break;
            }

            return result;
        }

        // returns not found
        public static List<Signature> ResolveSignatures(HookProcess proc, ref List<Signature> signatures) {
            const int bufferSize = 0x1200;
            const int regionIncrement = 0x1000;

            byte[] buffer = new byte[bufferSize];
            List<Signature> notFound = new List<Signature>(signatures);
            List<Signature> temp = new List<Signature>();
            var regionCount = 0;


            IntPtr baseAddress = proc.BaseAddress;
            IntPtr searchEnd = proc.EndAddress;
            IntPtr searchStart = baseAddress;

            while (searchStart.ToInt64() < searchEnd.ToInt64()) {
                try {
                    IntPtr lpNumberOfBytesRead;
                    var regionSize = new IntPtr(bufferSize);
                    if (IntPtr.Add(searchStart, bufferSize).ToInt64() > searchEnd.ToInt64()) {
                        regionSize = (IntPtr) (searchEnd.ToInt64() - searchStart.ToInt64());
                    }

                    if (UnsafeNativeMethods.ReadProcessMemory(proc.Handle, searchStart, buffer, regionSize, out lpNumberOfBytesRead)) {
                        foreach (Signature signature in notFound) {
                            string sig = signature.Value;
                            byte[] sigbytes = SignatureToByte(sig, WildCardChar);
                            var index = FindSuperSignature(buffer, sigbytes);
                            if (index < 0) {
                                temp.Add(signature);
                                continue;
                            }

                            var baseResult = new IntPtr((long) (baseAddress + regionCount * regionIncrement));
                            IntPtr searchResult = IntPtr.Add(baseResult, index + signature.Offset);

                            signature.baseAddress = new IntPtr(searchResult.ToInt64());
                        }

                        notFound = new List<Signature>(temp);
                        temp.Clear();
                    }

                    regionCount++;
                    searchStart = IntPtr.Add(searchStart, regionIncrement);
                }
                catch (Exception ex) {
                    //MemoryHandler.Instance.RaiseException(Logger, ex, true);
                    Console.WriteLine("Resolve signatures " + ex.ToString());
                }
            }
            // Clear out
            foreach (Signature signature in notFound) {
                signatures.Remove(signature);
            }

            List<Signature> fixedSignatures = new List<Signature>();
            foreach(Signature signature in signatures) {
                Signature sig = signature;
                if(!string.IsNullOrEmpty(sig.Type)) {
                    Type t = Type.GetType("FFMemoryParser." + sig.Type);
                    if (t != null) {
                        sig = (Signature) Activator.CreateInstance(t, sig);
                        sig.ResolvePointerPath(proc);
                        Console.WriteLine(string.Format("{0} {1} {2}", sig.Key, sig.baseAddress, sig.baseAddress));
                        fixedSignatures.Add(sig);
                        continue;
                    }
                }
                Console.WriteLine(string.Format("Could not find type for {0}", sig.Key));
            }
            signatures = fixedSignatures;
            return notFound;
        }

        public static byte[] SignatureToByte(string signature, byte wildcard) {
            byte[] pattern = new byte[signature.Length / 2];
            int[] hexTable = {
                0x00,
                0x01,
                0x02,
                0x03,
                0x04,
                0x05,
                0x06,
                0x07,
                0x08,
                0x09,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x0A,
                0x0B,
                0x0C,
                0x0D,
                0x0E,
                0x0F
            };
            try {
                for (int x = 0,
                         i = 0; i < signature.Length; i += 2, x += 1) {
                    if (signature[i] == wildcard) {
                        pattern[x] = wildcard;
                    }
                    else {
                        pattern[x] = (byte) ((hexTable[char.ToUpper(signature[i]) - '0'] << 4) | hexTable[char.ToUpper(signature[i + 1]) - '0']);
                    }
                }

                return pattern;
            }
            catch {
                return null;
            }
        }

        private static List<UnsafeNativeMethods.MEMORY_BASIC_INFORMATION> LoadRegions(HookProcess proc) {
            try {
                var _regions = new List<UnsafeNativeMethods.MEMORY_BASIC_INFORMATION>();
                IntPtr address = IntPtr.Zero;
                while (true) {
                    var info = new UnsafeNativeMethods.MEMORY_BASIC_INFORMATION();
                    var result = UnsafeNativeMethods.VirtualQueryEx(proc.Handle, address, out info, (uint)Marshal.SizeOf(info));
                    if (result == 0) {
                        break;
                    }

                    if (!proc.IsSystemModule(info.BaseAddress) && (info.State & MemCommit) != 0 && (info.Protect & Writable) != 0 && (info.Protect & PageGuard) == 0) {
                        _regions.Add(info);
                    } else {
                        //MemoryHandler.Instance.RaiseException(Logger, new Exception(info.ToString()));
                    }

                    unchecked {
                        switch (IntPtr.Size) {
                            case sizeof(int):
                                address = new IntPtr(info.BaseAddress.ToInt32() + info.RegionSize.ToInt32());
                                break;
                            default:
                                address = new IntPtr(info.BaseAddress.ToInt64() + info.RegionSize.ToInt64());
                                break;
                        }
                    }
                }
                return _regions;
            } catch (Exception ex) {
                //MemoryHandler.Instance.RaiseException(Logger, ex, true);
                Console.WriteLine("DURRR ", ex.ToString());
                return new List<UnsafeNativeMethods.MEMORY_BASIC_INFORMATION>();
            }
        }
    }
}