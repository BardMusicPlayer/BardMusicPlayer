using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace BardMusicPlayer
{
    public partial class App
    {
#pragma warning disable CS1998
        protected override async void OnStartup(StartupEventArgs eventArgs)
#pragma warning restore CS1998
        {

#if LOCAL
            
            foreach (string dll in Directory.EnumerateFiles(@".\","*.dll")) File.Delete(dll);
#if DEBUG
            var dlls = Directory.GetFiles(@"..\..\..\..\BardMusicPlayer.Ui\bin\Debug\net48", "*.dll");
#else
            var dlls = Directory.GetFiles(@"..\..\..\..\BardMusicPlayer.Ui\bin\Release\net48", "*.dll");
#endif
            Type viewsType = null;
            foreach (var dll in dlls)
            {
                if (Path.GetFileName(dll).Equals("BardMusicPlayer.Ui.dll")) viewsType = Assembly.LoadFrom(dll).GetType("BardMusicPlayer.Ui.Main");
                else Assembly.LoadFrom(dll);
            }
            dynamic main = Activator.CreateInstance(viewsType ?? throw new InvalidOperationException("BardMusicPlayer.Ui.dll"));
            main.ShowDialog();
            base.OnStartup(eventArgs);

#else




#endif
        }
    }
}
