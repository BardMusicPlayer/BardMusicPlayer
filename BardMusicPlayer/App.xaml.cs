/*
 * Copyright(c) 2021 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace BardMusicPlayer
{
    public partial class App : Application
    {
        private static readonly string ExePath = Assembly.GetExecutingAssembly().Location;

#if DEBUG
        private static readonly bool Debug = true;
        private static readonly string DataPath = Directory.GetCurrentDirectory() + @"\Data\";
        private static readonly string ResourcePath = Directory.GetCurrentDirectory() + @"\";

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();
#else
        private static readonly bool Debug = false;
        private static readonly string DataPath = @Environment.GetFolderPath(@Environment.SpecialFolder.LocalApplicationData) + @"\BardMusicPlayer\";
        private static readonly string ResourcePath = DataPath + @"Resources\";

#endif

#pragma warning disable CS1998
        protected override async void OnStartup(StartupEventArgs eventArgs)
#pragma warning restore CS1998
        {
            try
            {
                Directory.CreateDirectory(DataPath);
                Directory.CreateDirectory(ResourcePath);

#if DEBUG
                AllocConsole();
#endif

                Ui.Bootstrapper.Instance.StartUp(Debug, 2, "2", ExePath, ResourcePath, DataPath, eventArgs.Args);

            }
            catch (Exception exception)
            {
                MessageBox.Show("Uh oh, something went wrong and BardMusicPlayer is shutting down.\nPlease ask for support in the Discord Server and provide a picture of this error message:\n\n" + exception.Message,
                    "BardMusicPlayer Launcher Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Environment.Exit(0);
            }

            base.OnStartup(eventArgs);
        }
    }
}
