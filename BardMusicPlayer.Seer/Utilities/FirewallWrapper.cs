// Machina ~ FirewallWrapper.cs
// 
// Copyright © 2017 Ravahn - All Rights Reserved
// 
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.If not, see<http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using NetFwTypeLib;

namespace BardMusicPlayer.Seer.Utilities
{
	internal class FirewallWrapper
	{
        internal bool? IsFirewallEnabled()
		{
			try
			{
				var typeFromProgId = Type.GetTypeFromProgID("HNetCfg.FwMgr");
                return Activator.CreateInstance(typeFromProgId) is INetFwMgr netFwMgr && netFwMgr.LocalPolicy.CurrentProfile.FirewallEnabled;
            }
			catch (Exception ex)
			{
				if (ex.Message.Contains("800706D9"))
				{
					return false;
				}
                throw;
			}
		}

        internal bool IsFirewallRuleConfigured(string appName)
		{
			var flag = false;
			var typeFromProgId = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
			var netFwPolicy = Activator.CreateInstance(typeFromProgId) as INetFwPolicy2;
            var enumerator = netFwPolicy?.Rules.GetEnumerator();
			if (enumerator == null) return false;
            while (enumerator.MoveNext() && !flag) if (enumerator.Current is INetFwRule2 netFwRule && netFwRule.Name == appName && netFwRule.Enabled) flag = true;
            return flag;
		}

        internal void AddFirewallApplicationEntry(string appName, string executablePath)
        {
            var typeFromProgId = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            var netFwPolicy = (INetFwPolicy2) Activator.CreateInstance(typeFromProgId);
            var typeFromProgId2 = Type.GetTypeFromProgID("HNetCfg.FWRule");
            var netFwRule = (INetFwRule) Activator.CreateInstance(typeFromProgId2);
            netFwRule.ApplicationName = executablePath;
            netFwRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            netFwRule.Description = "BardMusicPlayer firewall rule";
            netFwRule.Enabled = true;
            netFwRule.InterfaceTypes = "All";
            netFwRule.Name = appName;
            netFwPolicy.Rules.Add(netFwRule);
        }

        internal void RemoveFirewallApplicationEntry(string appName)
        {
            var typeFromProgId = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            var netFwPolicy = (INetFwPolicy2) Activator.CreateInstance(typeFromProgId);
            var num = netFwPolicy.Rules.Cast<INetFwRule>().Count(rule => rule.Name == appName);
            if (num == 0) num++;
            for (var i = 0; i < num; i++) netFwPolicy.Rules.Remove(appName);
        }
    }
}
