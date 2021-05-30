/*
 * Copyright(c) 2021 MoogleTroupe, 2018-2020 parulina
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
    internal class DatFileReaderBackend : IReaderBackend
    {
        public EventSource ReaderBackendType { get; }
        public ReaderHandler ReaderHandler { get; set; }
        public int SleepTimeInMs { get; set; }
        public DatFileReaderBackend(int sleepTimeInMs)
        {
            ReaderBackendType = EventSource.DatFile;
            SleepTimeInMs = sleepTimeInMs;
        }

        private string _configId = "";
        private FileSystemWatcher _fileSystemWatcher;
        private KeybindDatFile _keybindDatFile;
        private HotbarDatFile _hotbarDatFile;
        private readonly object _lock = new();

        public async Task Loop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(SleepTimeInMs, token);

                lock (_lock)
                {
                    if (_keybindDatFile != null && _hotbarDatFile != null && (_keybindDatFile.Fresh || _hotbarDatFile.Fresh))
                    {
                        _keybindDatFile.Fresh = false;
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
                                if (keyBind.GetKey() != Keys.None) instrumentToneKeys[instrumentTone] = keyBind.GetKey();
                            }

                            var navigationMenuKeys = Enum.GetValues(typeof(NavigationMenuKey)).Cast<NavigationMenuKey>().ToDictionary(navigationMenuKey => navigationMenuKey, navigationMenuKey => _keybindDatFile.GetKeybindFromKeyString(navigationMenuKey.ToString()));

                            var instrumentToneMenuKeys = Enum.GetValues(typeof(InstrumentToneMenuKey)).Cast<InstrumentToneMenuKey>().ToDictionary(instrumentToneMenuKey => instrumentToneMenuKey, instrumentToneMenuKey => _keybindDatFile.GetKeybindFromKeyString(instrumentToneMenuKey.ToString()));

                            var noteKeys = Enum.GetValues(typeof(NoteKey)).Cast<NoteKey>().ToDictionary(noteKey => noteKey, noteKey => _keybindDatFile.GetKeybindFromKeyString(noteKey.ToString()));

                            ReaderHandler.Game.PublishEvent(new KeyMapChanged(EventSource.DatFile, instrumentKeys, instrumentToneKeys, navigationMenuKeys, instrumentToneMenuKeys, noteKeys));
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

        private void CreateWatcher()
        {
            DisposeWatcher();

            try
            {
                ParseKeybind(new DirectoryInfo(ReaderHandler.Game.ConfigPath + _configId).GetFiles()
                    .Where(file => file.Name.ToLower().StartsWith("keybind")).Where(file => file.Name.ToLower().EndsWith(".dat"))
                    .OrderByDescending(file => file.LastWriteTimeUtc.ToUtcMilliTime()).First().FullName);

                ParseHotbar(new DirectoryInfo(ReaderHandler.Game.ConfigPath + _configId).GetFiles()
                    .Where(file => file.Name.ToLower().StartsWith("hotbar")).Where(file => file.Name.ToLower().EndsWith(".dat"))
                    .OrderByDescending(file => file.LastWriteTimeUtc.ToUtcMilliTime()).First().FullName);
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
            
            if (eventArgs.Name.ToLower().StartsWith("hotbar") && eventArgs.Name.ToLower().EndsWith(".dat")) ParseHotbar(eventArgs.FullPath);
            else if (eventArgs.Name.ToLower().StartsWith("keybind") && eventArgs.Name.ToLower().EndsWith(".dat")) ParseKeybind(eventArgs.FullPath);
        }

        private void OnError(object sender, ErrorEventArgs ex) => ReaderHandler.Game.PublishEvent(new BackendExceptionEvent(EventSource.DatFile, ex.GetException()));

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

        ~DatFileReaderBackend() => Dispose();
        public void Dispose()
        {
            DisposeWatcher();
            _keybindDatFile?.Dispose();
            _hotbarDatFile?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
