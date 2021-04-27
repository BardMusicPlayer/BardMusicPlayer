using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BardMusicPlayer.Updater.Util
{
    internal static class Loader
    {
        private static readonly List<string> Loaded = new();
        internal static async Task<bool> Load(BmpVersion version, string exePath, string versionPath, string dataPath, string[] cliParameters)
        {
                if (Loaded.Contains(version.entryClass))
                    throw new Exception(version.entryClass + " from " + version.entryDll + " has already been loaded.");


                Type viewsType = null;
                foreach (var item in version.items.Where(item => item.load))
                {
                    byte[] hashBytes = Sha256.StringToBytes(item.sha256);
                    if (item.destination.Equals(version.entryDll))
                        viewsType = Assembly
                            .LoadFrom(versionPath + item.destination)
                            .GetType(version.entryClass);
                    else
                        Assembly.LoadFrom(versionPath + item.destination);
                }

                dynamic main = Activator.CreateInstance(viewsType ?? throw new InvalidOperationException("Unable to run " + version.entryClass + " from " + version.entryDll));
                Loaded.Add(version.entryClass);
                return await main.StartUp(version.beta, version.build, version.commit, exePath, versionPath, dataPath, cliParameters);
        }
    }
}
