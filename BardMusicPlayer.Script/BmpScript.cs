/*
 * Copyright(c) 2022 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.Maestro;
using BardMusicPlayer.Maestro.Performance;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Script.BasicSharp;
using BardMusicPlayer.Seer;

#endregion

namespace BardMusicPlayer.Script;

public sealed class BmpScript
{
    private static readonly Lazy<BmpScript> LazyInstance = new(static () => new BmpScript());
    private Interpreter basic;

    private Thread thread;

    private BmpScript()
    {
    }

    /// <summary>
    /// </summary>
    public bool Started { get; private set; }

    public static BmpScript Instance => LazyInstance.Value;

    private string selectedBardName { get; set; } = "";
    private List<string> unselected_bards { get; set; }

    public event EventHandler<bool> OnRunningStateChanged;

    #region accessors

    public void StopExecution()
    {
        if (thread == null)
            return;
        if (basic == null)
            return;

        basic.StopExec();

        if (thread.ThreadState == ThreadState.Running)
            thread.Abort();
    }

    #endregion

    public void LoadAndRun(string basicfile)
    {
        Task.Run(() =>
        {
            thread = Thread.CurrentThread;
            OnRunningStateChanged?.Invoke(this, true);

            unselected_bards = new List<string>();
            basic = new Interpreter(File.ReadAllText(basicfile));
            basic.printHandler += Print;
            basic.cprintHandler += Console.WriteLine;
            basic.tapKeyHandler += TapKey;
            basic.selectedBardHandler += SetSelectedBard;
            basic.selectedBardAsStringHandler += SetSelectedBardName;
            basic.unselectBardHandler += UnSelectBardName;
            try
            {
                basic.Exec();
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
            }

            OnRunningStateChanged?.Invoke(this, false);

            unselected_bards = null;
            basic.printHandler -= Print;
            basic.cprintHandler -= Console.WriteLine;
            basic.tapKeyHandler -= TapKey;
            basic.selectedBardHandler -= SetSelectedBard;
            basic.selectedBardAsStringHandler -= SetSelectedBardName;
            basic.unselectBardHandler -= UnSelectBardName;
            basic = null;
        });
    }

    /// <summary>
    ///     Start Script.
    /// </summary>
    public void Start()
    {
        if (Started) return;

        if (!BmpPigeonhole.Initialized)
            throw new BmpScriptException("Script requires Pigeonhole to be initialized.");
        if (!BmpSeer.Instance.Started) throw new BmpScriptException("Script requires Seer to be running.");

        Started = true;
    }

    /// <summary>
    ///     Stop Script.
    /// </summary>
    public void Stop()
    {
        if (!Started) return;

        Started = false;
    }

    ~BmpScript()
    {
        Dispose();
    }

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }

    #region Routine Handlers

    public void SetSelectedBard(int num)
    {
        if (num == 0)
        {
            selectedBardName = "all";
            return;
        }

        var plist = BmpMaestro.Instance.GetAllPerformers();
        var performers = plist as Performer[] ?? plist.ToArray();
        if (!performers.Any())
        {
            selectedBardName = "";
            return;
        }

        var performer = performers.ElementAt(num - 1);
        selectedBardName = performer != null ? performer.game.PlayerName : "";
    }

    public void SetSelectedBardName(string name)
    {
        selectedBardName = name;
    }

    public void UnSelectBardName(string name)
    {
        if (name.ToLower().Equals(""))
        {
            unselected_bards.Clear();
        }
        else
        {
            if (name.Contains(","))
            {
                var names = name.Split(',');
                Parallel.ForEach(names, n =>
                {
                    var cname = n.Trim();
                    if (cname != "")
                        unselected_bards.Add(cname);
                });
            }
            else
            {
                unselected_bards.Add(name);
            }
        }
    }

    public void Print(ChatMessageChannelType type, string text)
    {
        BmpMaestro.Instance.SendText(selectedBardName, type, text, unselected_bards);
    }

    public void TapKey(string modifier, string character)
    {
        BmpMaestro.Instance.TapKey(selectedBardName, modifier, character, unselected_bards);
    }

    #endregion
}