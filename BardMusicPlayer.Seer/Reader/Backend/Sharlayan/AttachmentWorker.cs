/*
 * Copyright(c) 2007-2020 Ryan Wilson syndicated.life@gmail.com (http://syndicated.life/)
 * Licensed under the MIT license. See https://github.com/FFXIVAPP/sharlayan/blob/master/LICENSE.md for full license information.
 */

using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using BardMusicPlayer.Seer.Reader.Backend.Sharlayan.Models;

namespace BardMusicPlayer.Seer.Reader.Backend.Sharlayan
{
	internal class AttachmentWorker : IDisposable
	{

		private readonly Timer _scanTimer;

		private bool _isScanning;

		private ProcessModel _processModel;

		private readonly MemoryHandler _memoryHandler;

		public AttachmentWorker(MemoryHandler memoryHandler)
		{
			_memoryHandler = memoryHandler;
			_scanTimer = new Timer(1000.0);
			_scanTimer.Elapsed += ScanTimerElapsed;
		}

		public void Dispose()
		{
			_scanTimer.Elapsed -= ScanTimerElapsed;
		}

		public void StartScanning(ProcessModel processModel)
		{
			_processModel = processModel;
			_scanTimer.Enabled = true;
		}

		public void StopScanning()
		{
			_scanTimer.Enabled = false;
		}

		private void ScanTimerElapsed(object sender, ElapsedEventArgs e)
		{
			if (_isScanning || !_memoryHandler.IsAttached)
			{
				return;
			}
			_isScanning = true;
			Func<bool> func = delegate
			{
				var processes = Process.GetProcesses();
				if (!processes.Any(process => process.Id == _processModel.ProcessID && process.ProcessName == _processModel.ProcessName))
				{
				}
				_isScanning = false;
				return true;
			};
			func.BeginInvoke(delegate
			{
			}, func);
		}
	}
}
