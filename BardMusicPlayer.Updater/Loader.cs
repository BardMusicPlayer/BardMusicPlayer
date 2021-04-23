using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Linq;
using System.Reflection;

namespace BardMusicPlayer.Updater
{
    internal static class Loader
    {
        private static readonly List<string> Loaded = new();
        private static readonly object Lock = new();
        internal static bool Load(Version version, string exePath, string versionPath, string dataPath, string[] cliParameters)
        {
            lock (Lock)
            {
                if (Loaded.Contains(version.entryClass))
                    throw new Exception(version.entryClass + " from " + version.entryDll + " is already loaded.");


                Type viewsType = null;
                foreach (var item in version.items.Where(item => item.load))
                {
                    if (item.destination.Equals(version.entryDll))
                        viewsType = Assembly
                            .LoadFrom(versionPath + item.destination, Sha256.StringToBytes(item.sha256),
                                AssemblyHashAlgorithm.SHA256)
                            .GetType(version.entryClass);
                    else
                        Assembly.LoadFrom(versionPath + item.destination, Sha256.StringToBytes(item.sha256),
                            AssemblyHashAlgorithm.SHA256);
                }

                dynamic main = Activator.CreateInstance(viewsType ??
                                                        throw new InvalidOperationException("Unable to run " +
                                                            version.entryClass + " from " + version.entryDll));
                Loaded.Add(version.entryClass);
                return main.StartUp(version.beta, version.build, version.commit, version.extra, exePath, versionPath,
                    dataPath, cliParameters);
            }
        }
    }
}
