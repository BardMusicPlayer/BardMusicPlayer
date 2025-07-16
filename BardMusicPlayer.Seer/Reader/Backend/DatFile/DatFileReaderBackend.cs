/*
 * Copyright(c) 2023 MoogleTroupe, sammhill, 2018-2020 parulina
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.Quotidian.Enums;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Quotidian.UtcMilliTime;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer.Reader.Backend.DatFile
{
    internal sealed class DatFileReaderBackend : IReaderBackend
    {
        private readonly object _lock = new();
        private CommonDatFile _commonDatFile;

        private string _configId = "";
        private FileSystemWatcher _fileSystemWatcher;
        private HotbarDatFile _hotbarDatFile;
        private KeybindDatFile _keybindDatFile;

        public DatFileReaderBackend(int sleepTimeInMs)
        {
            ReaderBackendType = EventSource.DatFile;
            SleepTimeInMs = sleepTimeInMs;
        }

        public EventSource ReaderBackendType { get; }
        public ReaderHandler ReaderHandler { get; set; }
        public int SleepTimeInMs { get; set; }

        public async Task Loop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(SleepTimeInMs, token);

                lock (_lock)
                {
                    if (_keybindDatFile != null && _hotbarDatFile != null && _commonDatFile != null &&
                        (_keybindDatFile.Fresh || _hotbarDatFile.Fresh || _commonDatFile.Fresh))
                    {
                        _keybindDatFile.Fresh = false;
                        _commonDatFile.Fresh = false;
                        _hotbarDatFile.Fresh = false;

                        try
                        {
                            var instrumentKeys = new Dictionary<Instrument, Keys>();

                            foreach (var instrument in Instrument.All)
                            {
                                if (token.IsCancellationRequested)
                                    return;

                                if (instrument.Equals(Instrument.None)) continue;

                                instrumentKeys.Add(instrument, Keys.None);

                                var keyMap = _hotbarDatFile.GetInstrumentKeyMap(instrument);

                                if (string.IsNullOrEmpty(keyMap)) continue;

                                var keyBind = _keybindDatFile[keyMap];
                                if (keyBind.GetKey() != Keys.None) instrumentKeys[instrument] = keyBind.GetKey();
                            }

                            var instrumentToneKeys = new Dictionary<InstrumentTone, Keys>();

                            foreach (var instrumentTone in InstrumentTone.All)
                            {
                                if (token.IsCancellationRequested)
                                    return;

                                if (instrumentTone.Equals(InstrumentTone.None)) continue;

                                instrumentToneKeys.Add(instrumentTone, Keys.None);

                                var keyMap = _hotbarDatFile.GetInstrumentToneKeyMap(instrumentTone);

                                if (string.IsNullOrEmpty(keyMap)) continue;

                                var keyBind = _keybindDatFile[keyMap];
                                if (keyBind.GetKey() != Keys.None)
                                    instrumentToneKeys[instrumentTone] = keyBind.GetKey();
                            }

                            var navigationMenuKeys = Enum.GetValues(typeof(NavigationMenuKey)).Cast<NavigationMenuKey>()
                                .ToDictionary(static navigationMenuKey => navigationMenuKey,
                                    navigationMenuKey =>
                                        _keybindDatFile.GetKeybindFromKeyString(navigationMenuKey.ToString()));

                            var instrumentToneMenuKeys = Enum.GetValues(typeof(InstrumentToneMenuKey))
                                .Cast<InstrumentToneMenuKey>().ToDictionary(
                                    static instrumentToneMenuKey => instrumentToneMenuKey,
                                    instrumentToneMenuKey =>
                                        _keybindDatFile.GetKeybindFromKeyString(instrumentToneMenuKey.ToString()));

                            var noteKeys = Enum.GetValues(typeof(NoteKey)).Cast<NoteKey>()
                                .ToDictionary(static noteKey => noteKey,
                                    noteKey => _keybindDatFile.GetKeybindFromKeyString(noteKey.ToString()));

                            ReaderHandler.Game.PublishEvent(new KeyMapChanged(EventSource.DatFile, instrumentKeys,
                                instrumentToneKeys, navigationMenuKeys, instrumentToneMenuKeys, noteKeys));
                        }
                        catch (Exception ex)
                        {
                            ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.DatFile, ex));
                        }
                    }
                }

                if (ReaderHandler.Game.ConfigId.Equals(_configId)) continue;

                _configId = ReaderHandler.Game.ConfigId;
                CreateWatcher();
            }

            DisposeWatcher();
        }

        public void Dispose()
        {
            DisposeWatcher();
            _keybindDatFile?.Dispose();
            _hotbarDatFile?.Dispose();
            _commonDatFile?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void CreateWatcher()
        {
            DisposeWatcher();

            try
            {
                ParseKeybind(new DirectoryInfo(ReaderHandler.Game.ConfigPath + _configId).GetFiles()
                    .Where(static file => file.Name.ToLower().StartsWith("keybind", StringComparison.Ordinal))
                    .Where(static file => file.Name.ToLower().EndsWith(".dat", StringComparison.Ordinal))
                    .OrderByDescending(static file => file.LastWriteTimeUtc.ToUtcMilliTime()).First().FullName);

                ParseHotbar(new DirectoryInfo(ReaderHandler.Game.ConfigPath + _configId).GetFiles()
                    .Where(static file => file.Name.ToLower().StartsWith("hotbar", StringComparison.Ordinal))
                    .Where(static file => file.Name.ToLower().EndsWith(".dat", StringComparison.Ordinal))
                    .OrderByDescending(static file => file.LastWriteTimeUtc.ToUtcMilliTime()).First().FullName);

                ParseCommon(new DirectoryInfo(ReaderHandler.Game.ConfigPath + _configId).GetFiles()
                    .Where(static file => file.Name.ToLower().StartsWith("common", StringComparison.Ordinal))
                    .Where(static file => file.Name.ToLower().EndsWith(".dat", StringComparison.Ordinal))
                    .OrderByDescending(static file => file.LastWriteTimeUtc.ToUtcMilliTime()).First().FullName);
            }
            catch (Exception ex)
            {
                ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.DatFile, ex));
            }

            _fileSystemWatcher = new FileSystemWatcher(ReaderHandler.Game.ConfigPath + _configId, "*.dat")
            {
                NotifyFilter = NotifyFilters.LastWrite,
                IncludeSubdirectories = false,
                EnableRaisingEvents = true
            };

            _fileSystemWatcher.Error += OnError;
            _fileSystemWatcher.Changed += OnChanged;
        }

        private void DisposeWatcher()
        {
            if (_fileSystemWatcher is null) return;

            _fileSystemWatcher.Changed -= OnChanged;
            _fileSystemWatcher.Error -= OnError;
            _fileSystemWatcher.Dispose();
        }

        private void OnChanged(object sender, FileSystemEventArgs eventArgs)
        {
            if (eventArgs.ChangeType != WatcherChangeTypes.Changed) return;

            if (eventArgs.Name.ToLower().StartsWith("hotbar", StringComparison.Ordinal) &&
                eventArgs.Name.ToLower().EndsWith(".dat", StringComparison.Ordinal))
                ParseHotbar(eventArgs.FullPath);
            else if (eventArgs.Name.ToLower().StartsWith("keybind", StringComparison.Ordinal) &&
                     eventArgs.Name.ToLower().EndsWith(".dat", StringComparison.Ordinal))
                ParseKeybind(eventArgs.FullPath);
        }

        private void OnError(object sender, ErrorEventArgs ex)
        {
            ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.DatFile, ex.GetException()));
        }

        private void ParseKeybind(string filePath)
        {
            lock (_lock)
            {
                try
                {
                    var newDat = new KeybindDatFile(filePath);
                    if (newDat.Load())
                    {
                        _keybindDatFile?.Dispose();
                        _keybindDatFile = newDat;
                    }
                    else
                    {
                        newDat?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.DatFile, ex));
                }
            }
        }

        private void ParseHotbar(string filePath)
        {
            lock (_lock)
            {
                try
                {
                    var newDat = new HotbarDatFile(filePath);
                    if (newDat.Load())
                    {
                        _hotbarDatFile?.Dispose();
                        _hotbarDatFile = newDat;
                    }
                    else
                    {
                        newDat?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.DatFile, ex));
                }
            }
        }

        private void ParseCommon(string filePath)
        {
            lock (_lock)
            {
                try
                {
                    var newDat = new CommonDatFile(filePath);
                    if (newDat.Load())
                    {
                        _commonDatFile?.Dispose();
                        _commonDatFile = newDat;
                    }
                    else
                    {
                        newDat?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.DatFile, ex));
                }
            }
        }

        ~DatFileReaderBackend()
        {
            Dispose();
        }
    }
}