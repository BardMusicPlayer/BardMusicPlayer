using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Timer = System.Timers.Timer;

namespace FFBardMusicPlayer {
	public class FFXIVHook {

		#region WIN32

		private const int WH_KEYBOARD_LL = 13;
		private const int WH_GETMESSAGE = 3;
		private const int WM_KEYDOWN = 0x0100;
		private const int WM_KEYUP = 0x0101;
		private const int WM_CHAR = 0x0102;

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, MessageProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("user32.dll", SetLastError = true)]
		static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		static extern IntPtr GetMessageExtraInfo();

		[DllImport("user32.dll", SetLastError = true)]
		static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern uint MapVirtualKey(uint uCode, uint uMapType);

		struct INPUT {
			public int type;
			public InputUnion un;
		}

		[StructLayout(LayoutKind.Explicit)]
		struct InputUnion {
			[FieldOffset(0)]
			public MOUSEINPUT mi;
			[FieldOffset(0)]
			public KEYBDINPUT ki;
			[FieldOffset(0)]
			public HARDWAREINPUT hi;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MOUSEINPUT {
			public int dx;
			public int dy;
			public uint mouseData;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct KEYBDINPUT {
			public ushort wVk;
			public ushort wScan;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct HARDWAREINPUT {
			public uint uMsg;
			public ushort wParamL;
			public ushort wParamH;
		}

		public delegate IntPtr MessageProc(int nCode, IntPtr wParam, IntPtr lParam);

		#endregion

		public NoteDoubleDetection<FFXIVKeybindDat.Keybind> keyTimers = new NoteDoubleDetection<FFXIVKeybindDat.Keybind>();
		public List<FFXIVKeybindDat.Keybind> lastPerformanceKeys = new List<FFXIVKeybindDat.Keybind>();

		private IntPtr mainWindowHandle;
		private static MessageProc proc;
		private IntPtr _hookID = IntPtr.Zero;

		public EventHandler<Keys> OnKeyPressed;

		public FFXIVHook() {
			keyTimers.NoteEvent += delegate (Object o, FFXIVKeybindDat.Keybind keybind) {
				SendSyncKey(keybind.GetKey(), true, true, false);
				// Send synced key down
			};
		}

		public bool Hook(Process process) {
			if(process == null) {
				return false;
			}
			if(_hookID != IntPtr.Zero) {
				Unhook();
			}

			proc = new MessageProc(HookCallback);

			mainWindowHandle = process.MainWindowHandle;
			_hookID = SetWindowsHookEx(WH_KEYBOARD_LL, proc, IntPtr.Zero, 0);
			if(_hookID != IntPtr.Zero) {
				return true;
			} else {
				return false;
			}
		}
		public void Unhook() {
			UnhookWindowsHookEx(_hookID);
			_hookID = IntPtr.Zero;
		}

		public void FocusWindow() {
			SetForegroundWindow(mainWindowHandle);
		}

		public void SendSyncKey(Keys key, bool modifier = true, bool sendDown = true, bool sendUp = true) {

			Keys key2 = (key & ~Keys.Control) & (key & ~Keys.Shift);
			if(sendDown) {
				if(modifier) {
					if((key & Keys.Control) == Keys.Control) {
						SendMessage(mainWindowHandle, WM_KEYDOWN, ((IntPtr) Keys.ControlKey), ((IntPtr) 0));
					}
					if((key & Keys.Shift) == Keys.Shift) {
						SendMessage(mainWindowHandle, WM_KEYDOWN, ((IntPtr) Keys.ShiftKey), ((IntPtr) 0));
					}
				}
				SendMessage(mainWindowHandle, WM_KEYDOWN, ((IntPtr) key2), ((IntPtr) 0));
			}
			if(sendUp) {
				SendMessage(mainWindowHandle, WM_KEYUP, ((IntPtr) key2), ((IntPtr) 0));
				if(modifier) {
					if((key & Keys.Control) == Keys.Control) {
						SendMessage(mainWindowHandle, WM_KEYUP, ((IntPtr) Keys.ControlKey), ((IntPtr) 0));
					}
					if((key & Keys.Shift) == Keys.Shift) {
						SendMessage(mainWindowHandle, WM_KEYUP, ((IntPtr) Keys.ShiftKey), ((IntPtr) 0));
					}
				}
			}
		}
		public void SendAsyncKey(Keys key, bool modifier = true, bool sendDown = true, bool sendUp = true) {

			Keys key2 = (key & ~Keys.Control) & (key & ~Keys.Shift);
			if(sendDown) {
				if(modifier) {
					if((key & Keys.Control) == Keys.Control) {
						PostMessage(mainWindowHandle, WM_KEYDOWN, ((IntPtr) Keys.ControlKey), ((IntPtr) 0));
					}
					if((key & Keys.Shift) == Keys.Shift) {
						PostMessage(mainWindowHandle, WM_KEYDOWN, ((IntPtr) Keys.ShiftKey), ((IntPtr) 0));
					}
				}
				PostMessage(mainWindowHandle, WM_KEYDOWN, ((IntPtr) key2), ((IntPtr) 0));
			}
			if(sendUp) {
				PostMessage(mainWindowHandle, WM_KEYUP, ((IntPtr) key2), ((IntPtr) 0));
				if(modifier) {
					if((key & Keys.Control) == Keys.Control) {
						PostMessage(mainWindowHandle, WM_KEYUP, ((IntPtr) Keys.ControlKey), ((IntPtr) 0));
					}
					if((key & Keys.Shift) == Keys.Shift) {
						PostMessage(mainWindowHandle, WM_KEYUP, ((IntPtr) Keys.ShiftKey), ((IntPtr) 0));
					}
				}
			}
		}

		public void SendKeyStroke(Keys keyDown = Keys.None, Keys modDown = Keys.None, Keys keyUp = Keys.None, Keys modUp = Keys.None) {
			if(keyDown != Keys.None) {
				SendAsyncKey(keyDown | modDown, true, true, false);
			}
			if(keyUp != Keys.None) {
				SendAsyncKey(keyDown | modDown, true, false, true);
			}
		}

		public void SendKeyInput(List<KEYBDINPUT> KeybdInput) {
			List<INPUT> keyList = new List<INPUT>();
			foreach(KEYBDINPUT input in KeybdInput) {
				keyList.Add(new INPUT {
					type = 1,
					un = new InputUnion {
						ki = input,
					},
				});
			}
			SendInput((uint) keyList.Count, keyList.ToArray(), Marshal.SizeOf(typeof(INPUT)));
		}

		public void SendAsyncChar(char charInput) {
			SendMessage(mainWindowHandle, WM_CHAR, ((IntPtr) charInput), ((IntPtr) 0));
		}

		public void SendString(string input) {
			if(GetForegroundWindow() != mainWindowHandle) {
				SetForegroundWindow(mainWindowHandle);
			}
			List<INPUT> keyList = new List<INPUT>();
			foreach(short c in input) {
				if(c > 10) {
					INPUT keyDown = new INPUT {
						type = 1,
						un = new InputUnion {
							ki = new KEYBDINPUT {
								wVk = 0,
								wScan = (ushort) c,
								dwFlags = 0x0004,
							}
						}
					};
					keyList.Add(keyDown);

					INPUT keyUp = new INPUT {
						type = 1,
						un = new InputUnion {
							ki = new KEYBDINPUT {
								wVk = 0,
								wScan = (ushort) c,
								dwFlags = 0x0004 | 0x0002,
							}
						}
					};
					keyList.Add(keyUp);
				}
			}
			SendInput((uint) keyList.Count, keyList.ToArray(), Marshal.SizeOf(typeof(INPUT)));
		}

		public void SendAsyncKeybind(FFXIVKeybindDat.Keybind keybind) {

			SendAsyncKey(keybind.GetKey(), true, true, true);
		}
		public void SendSyncKeybind(FFXIVKeybindDat.Keybind keybind) {
			SendSyncKey(keybind.GetKey(), true, true, true);
		}

		public void SendKeybindDown(FFXIVKeybindDat.Keybind keybind) {
			if(keybind == null) {
				return;
			}
			Keys key = keybind.GetKey();
			if(key == Keys.None) {
				return;
			}

			if(keyTimers.OnKey(keybind)) {
				// This is a Synced key so as to somewhat have control
				// over the delay 
				SendSyncKey(key, true, false, true);
			}
			if(!keyTimers.HasTimer(keybind)) {
				SendAsyncKey(key, true, true, false);
			}

			if(!lastPerformanceKeys.Contains(keybind)) {
				lastPerformanceKeys.Add(keybind);
			}
		}
		public void SendKeybindUp(FFXIVKeybindDat.Keybind keybind) {
			if(keybind == null) {
				return;
			}
			Keys key = keybind.GetKey();
			if(key == Keys.None) {
				return;
			}
			keyTimers.OffKey(keybind);
			SendAsyncKey(key, true, false, true);

			if(lastPerformanceKeys.Contains(keybind)) {
				lastPerformanceKeys.Remove(keybind);
			}
		}

		public void ClearLastPerformanceKeybinds() {
			foreach(FFXIVKeybindDat.Keybind keybind in lastPerformanceKeys.ToArray()) {
				SendSyncKey(keybind.GetKey(), true, false, true);
			}
			lastPerformanceKeys.Clear();
		}

		public IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {

			if(GetForegroundWindow() == mainWindowHandle) {
				if(nCode >= 0 && wParam == (IntPtr) WM_KEYDOWN) {
					int vkCode = Marshal.ReadInt32(lParam);
					OnKeyPressed?.Invoke(this, ((Keys) vkCode));
				}
			}
			return CallNextHookEx(_hookID, nCode, wParam, lParam);
		}
	}
}
