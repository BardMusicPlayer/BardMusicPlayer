// Copyright © 2021 Ravahn - All Rights Reserved
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

using Machina.FFXIV.Headers.Opcodes;

namespace Machina.FFXIV.Headers;

/// <summary>
/// Enumerates the known FFXIV server message types.  Note that some names were adopted from the Sapphire project
/// </summary>
public struct Server_MessageType
{
    public static readonly Server_MessageType StatusEffectList = OpcodeManager.Instance.CurrentOpcodes["StatusEffectList"];
    public static readonly Server_MessageType StatusEffectList2 = OpcodeManager.Instance.CurrentOpcodes["StatusEffectList2"];
    public static readonly Server_MessageType StatusEffectList3 = OpcodeManager.Instance.CurrentOpcodes["StatusEffectList3"];
    public static readonly Server_MessageType BossStatusEffectList = OpcodeManager.Instance.CurrentOpcodes["BossStatusEffectList"];
    public static readonly Server_MessageType Ability1 = OpcodeManager.Instance.CurrentOpcodes["Ability1"];
    public static readonly Server_MessageType Ability8 = OpcodeManager.Instance.CurrentOpcodes["Ability8"];
    public static readonly Server_MessageType Ability16 = OpcodeManager.Instance.CurrentOpcodes["Ability16"];
    public static readonly Server_MessageType Ability24 = OpcodeManager.Instance.CurrentOpcodes["Ability24"];
    public static readonly Server_MessageType Ability32 = OpcodeManager.Instance.CurrentOpcodes["Ability32"];
    public static readonly Server_MessageType ActorCast = OpcodeManager.Instance.CurrentOpcodes["ActorCast"];
    public static readonly Server_MessageType EffectResult = OpcodeManager.Instance.CurrentOpcodes["EffectResult"];
    public static readonly Server_MessageType EffectResultBasic = OpcodeManager.Instance.CurrentOpcodes["EffectResultBasic"];
    public static readonly Server_MessageType ActorControl = OpcodeManager.Instance.CurrentOpcodes["ActorControl"];
    public static readonly Server_MessageType ActorControlSelf = OpcodeManager.Instance.CurrentOpcodes["ActorControlSelf"];
    public static readonly Server_MessageType ActorControlTarget = OpcodeManager.Instance.CurrentOpcodes["ActorControlTarget"];
    public static readonly Server_MessageType UpdateHpMpTp = OpcodeManager.Instance.CurrentOpcodes["UpdateHpMpTp"];
    public static readonly Server_MessageType PlayerSpawn = OpcodeManager.Instance.CurrentOpcodes["PlayerSpawn"];
    public static readonly Server_MessageType NpcSpawn = OpcodeManager.Instance.CurrentOpcodes["NpcSpawn"];
    public static readonly Server_MessageType NpcSpawn2 = OpcodeManager.Instance.CurrentOpcodes["NpcSpawn2"];
    public static readonly Server_MessageType ActorMove = OpcodeManager.Instance.CurrentOpcodes["ActorMove"];
    public static readonly Server_MessageType ActorSetPos = OpcodeManager.Instance.CurrentOpcodes["ActorSetPos"];
    public static readonly Server_MessageType ActorGauge = OpcodeManager.Instance.CurrentOpcodes["ActorGauge"];
    public static readonly Server_MessageType PresetWaymark = OpcodeManager.Instance.CurrentOpcodes["PresetWaymark"];
    public static readonly Server_MessageType Waymark = OpcodeManager.Instance.CurrentOpcodes["Waymark"];
    public static readonly Server_MessageType SystemLogMessage = OpcodeManager.Instance.CurrentOpcodes["SystemLogMessage"];

    public ushort InternalValue { get; private set; }

    public override bool Equals(object obj)
    {
        Server_MessageType otherObj = (Server_MessageType)obj;
        return otherObj.InternalValue.Equals(InternalValue);
    }

    public static bool operator ==(Server_MessageType obj1, Server_MessageType obj2)
    {
        return obj1.InternalValue == obj2.InternalValue;
    }
    public static bool operator ==(ushort obj1, Server_MessageType obj2)
    {
        return obj1 == obj2.InternalValue;
    }
    public static bool operator ==(Server_MessageType obj1, ushort obj2)
    {
        return obj1.InternalValue == obj2;
    }

    public override int GetHashCode()
    {
        return InternalValue.GetHashCode();
    }

    public static bool operator !=(Server_MessageType obj1, Server_MessageType ojb2)
    {
        return !(obj1 == ojb2);
    }
    public static bool operator !=(ushort obj1, Server_MessageType ojb2)
    {
        return !(obj1 == ojb2);
    }
    public static bool operator !=(Server_MessageType obj1, ushort ojb2)
    {
        return !(obj1 == ojb2);
    }

    public static implicit operator Server_MessageType(ushort otherType)
    {
        return new Server_MessageType
        {
            InternalValue = otherType
        };
    }

    public static implicit operator ushort(Server_MessageType otherType)
    {
        return otherType.InternalValue;
    }
}