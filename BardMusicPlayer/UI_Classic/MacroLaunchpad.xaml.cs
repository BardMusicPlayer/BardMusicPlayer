#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BardMusicPlayer.Script;
using BardMusicPlayer.Ui.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;

#endregion

namespace BardMusicPlayer.Ui.Classic;

public sealed class Macro
{
    public string DisplayedText { get; set; } = "";
    public string File { get; set; } = "";
}

public sealed partial class MacroLaunchpad
{
    public MacroLaunchpad()
    {
        InitializeComponent();
        BmpScript.Instance.OnRunningStateChanged += Instance_OnRunningStateChanged;


        DataContext = this;
        _Macros = new List<Macro>();
        MacroList.ItemsSource = _Macros;
    }

    public List<Macro> _Macros { get; }
    public Macro SelectedMacro { get; set; }

    private void Instance_OnRunningStateChanged(object sender, bool e)
    {
        Dispatcher.BeginInvoke(e
            ? new Action(() => { StopIndicator.Content = "Stop"; })
            : () => StopIndicator.Content = "Idle");
    }

    private void Macros_CollectionChanged()
    {
        MacroList.ItemsSource = _Macros;
        MacroList.Items.Refresh();
    }

    private void MacroList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedMacro = MacroList.SelectedItem as Macro;
    }

    private void MacroList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedMacro == null)
            return;

        SelectedMacro = MacroList.SelectedItem as Macro;
        if (SelectedMacro != null && !File.Exists(SelectedMacro.File))
            return;

        if (SelectedMacro != null) BmpScript.Instance.LoadAndRun(SelectedMacro.File);
    }

    private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (SelectedMacro == null)
            return;

        var macroEdit = new MacroEditWindow(SelectedMacro)
        {
            Visibility = Visibility.Visible
        };
        macroEdit.Closed += MacroEdit_Closed;
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        var newMacro = new Macro();

        var macroEdit = new MacroEditWindow(newMacro)
        {
            Visibility = Visibility.Visible
        };
        macroEdit.Closed += MacroEdit_Closed;
        _Macros.Add(newMacro);
        Macros_CollectionChanged();
    }

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedMacro == null)
            return;

        _Macros.Remove(SelectedMacro);
        SelectedMacro = null;
        Macros_CollectionChanged();
    }

    private void Load_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Macro List | *.cfg",
            Multiselect = true
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        var memoryStream = new MemoryStream();
        var fileStream = File.Open(openFileDialog.FileName, FileMode.Open);
        fileStream.CopyTo(memoryStream);
        fileStream.Close();

        var data = memoryStream.ToArray();
        _Macros.Clear();
        var x = JsonConvert.DeserializeObject<List<Macro>>(new UTF8Encoding(true).GetString(data));
        foreach (var m in x)
            _Macros.Add(m);

        Macros_CollectionChanged();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (_Macros.Count <= 0)
            return;

        var openFileDialog = new SaveFileDialog
        {
            Filter = "Macro List | *.cfg"
        };

        if (openFileDialog.ShowDialog() != true)
            return;

        var t = JsonConvert.SerializeObject(_Macros);
        var content = new UTF8Encoding(true).GetBytes(t);

        var fileStream = File.Create(openFileDialog.FileName);
        fileStream.Write(content, 0, content.Length);
        fileStream.Close();

        Macros_CollectionChanged();
    }

    private void MacroEdit_Closed(object sender, EventArgs e)
    {
        MacroList.Items.Refresh();
    }

    private void StopIndicator_Click(object sender, RoutedEventArgs e)
    {
        BmpScript.Instance.StopExecution();
    }
}