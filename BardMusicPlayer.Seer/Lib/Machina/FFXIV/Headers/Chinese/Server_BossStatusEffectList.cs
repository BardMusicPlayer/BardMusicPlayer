﻿// Copyright © 2021 Ravahn - All Rights Reserved
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY. without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see<http://www.gnu.org/licenses/>.

using System.Runtime.InteropServices;

namespace Machina.FFXIV.Headers.Chinese
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct Server_BossStatusEffectList
    {
        public Server_MessageHeader MessageHeader; // 8 DWORDS
        public fixed byte Effects2[30 * 3 * 4];
        public byte JobID;
        public byte Level1;
        public byte Level2;
        public byte Level3;
        public uint CurrentHP;
        public uint MaxHP;
        public ushort CurrentMP;
        public ushort MaxMP;
        public ushort Unknown1;
        public byte DamageShield;
        public byte Unknown2;
        public fixed byte Effects1[30 * 3 * 4];
        public uint Unknown3;
    }
}
