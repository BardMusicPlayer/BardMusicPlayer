using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Timer = System.Timers.Timer;

namespace FFBardMusicPlayer {
	public class FFXIVHook {

		#region WIN32

		private const int WH_KEYBOARD_LL = 13;
		private const int WH_GETMESSAGE = 3;
		private const int WM_KEYDOWN = 0x0100;
		private const int WM_KEYUP = 0x0101;
		public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;
		private const int WM_CHAR = 0x0102;
		private const int WM_LBUTTONDOWN = 0x0201;
		private const int WM_LBUTTONUP = 0x0202;

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

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetClientRect(HandleRef hWnd, out RECT lpRect);

		[DllImport("user32.dll")]
		static extern bool ClientToScreen(HandleRef hWnd, ref POINT lpPoint);


		[StructLayout(LayoutKind.Sequential)]
		public struct RECT {
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;

			public int Width() {
				return Right - Left;
			}
			public int Height() {
				return Bottom - Top;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT {
			public int X;
			public int Y;
			public POINT(int x, int y) {
				X = x;
				Y = y;
			}
		}

		public delegate IntPtr MessageProc(int nCode, IntPtr wParam, IntPtr lParam);

		#endregion

		public List<FFXIVKeybindDat.Keybind> lastPerformanceKeys = new List<FFXIVKeybindDat.Keybind>();

		private IntPtr mainWindowHandle;
		private static MessageProc proc;
		private IntPtr _hookID = IntPtr.Zero;

		public EventHandler<Keys> OnKeyPressed;

		private Process referenceProcess;
		public Process Process {
			get {
				return referenceProcess;
			}
		}

		public FFXIVHook() { }

		public bool Hook(Process process, bool useCallback = true) {
			if(process == null) {
				return false;
			}
			if(_hookID != IntPtr.Zero) {
				Unhook();
			}
			referenceProcess = process;
			mainWindowHandle = process.MainWindowHandle;

			if(useCallback) {
				proc = new MessageProc(HookCallback);
				_hookID = SetWindowsHookEx(WH_KEYBOARD_LL, proc, IntPtr.Zero, 0);
				if(_hookID != IntPtr.Zero) {
					return true;
				} else {
					return false;
				}
			}
			return true;
		}
		public void Unhook() {
			if(_hookID != IntPtr.Zero) {
				UnhookWindowsHookEx(_hookID);
				referenceProcess = null;
				_hookID = IntPtr.Zero;
			}
		}

		public void FocusWindow() {
			SetForegroundWindow(mainWindowHandle);
		}
		public void SendTimedSyncKey(Keys key, bool modifier = true, bool sendDown = true, bool sendUp = true) {
            Task.Run(() =>
			{
				Keys key2 = (key & ~Keys.Control) & (key & ~Keys.Shift) & (key & ~Keys.Alt);
				if (sendDown)
				{
					if (modifier)
					{
						for (int i = 0; i < 5; i++)
						{
							if ((key & Keys.Control) == Keys.Control)
							{
								SendMessage(mainWindowHandle, WM_KEYDOWN, ((IntPtr)Keys.ControlKey), ((IntPtr)0));
							}
							if ((key & Keys.Alt) == Keys.Alt)
							{
								SendMessage(mainWindowHandle, WM_SYSKEYDOWN, ((IntPtr)Keys.Menu), ((IntPtr)0));
							}
							if ((key & Keys.Shift) == Keys.Shift)
							{
								SendMessage(mainWindowHandle, WM_KEYDOWN, ((IntPtr)Keys.ShiftKey), ((IntPtr)0));
							}
							Thread.Sleep(5);
						}
					}
					SendMessage(mainWindowHandle, WM_KEYDOWN, ((IntPtr)key2), ((IntPtr)0));
					Thread.Sleep(50);
				}
				if (sendUp)
				{
					SendMessage(mainWindowHandle, WM_KEYUP, ((IntPtr)key2), ((IntPtr)0));
					if (modifier)
					{
						if ((key & Keys.Shift) == Keys.Shift)
						{
							Thread.Sleep(5);
							SendMessage(mainWindowHandle, WM_KEYUP, ((IntPtr)Keys.ShiftKey), ((IntPtr)0));
						}
						if ((key & Keys.Alt) == Keys.Alt)
						{
							Thread.Sleep(5);
							SendMessage(mainWindowHandle, WM_SYSKEYUP, ((IntPtr)Keys.Menu), ((IntPtr)0));
						}
						if ((key & Keys.Control) == Keys.Control)
						{
							Thread.Sleep(5);
							SendMessage(mainWindowHandle, WM_KEYUP, ((IntPtr)Keys.ControlKey), ((IntPtr)0));
						}
					}
				}
			});
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

		public void SendTimedSyncKeybind(FFXIVKeybindDat.Keybind keybind)
        {
			SendTimedSyncKey(keybind.GetKey(), true, true, true);
        }

		public void SendKeybindDown(FFXIVKeybindDat.Keybind keybind) {
			if(keybind == null) {
				return;
			}
			Keys key = keybind.GetKey();
			if(key == Keys.None) {
				return;
			}

			SendAsyncKey(key, true, true, false);

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

		public bool SendUiMouseClick(FFXIVAddonDat addon, uint uiWidget, int x, int y) {
			if(addon[uiWidget].id == uiWidget) {
				FFXIVHook.RECT clientRect = this.GetClientRect();
				Point point = addon[uiWidget].GetXYPoint(clientRect.Width(), clientRect.Height());
				this.SendMouseClick(point.X + x, point.Y + y);
				return true;
			}
			return false;
		}

		public void SendMouseClick(int x, int y) {
			FFXIVHook.POINT point = new FFXIVHook.POINT(x, y);
			if(this.GetScreenFromClientPoint(ref point)) {
				Cursor.Position = new Point(point.X, point.Y);
				List<INPUT> mouseDown = new List<INPUT> {
					new INPUT {
						type = 0,
						un = new InputUnion {
							mi = new MOUSEINPUT {
								dx = point.X,
								dy = point.Y,
								dwFlags = 0x8000 | 0x0002
							}
						}
					}
				};
				SendInput((uint) mouseDown.Count, mouseDown.ToArray(), Marshal.SizeOf(typeof(INPUT)));
				
				List<INPUT> mouseUp = new List<INPUT> {
					new INPUT {
						type = 0,
						un = new InputUnion {
							mi = new MOUSEINPUT {
								dx = point.X+1,
								dy = point.Y+1,
								dwFlags = 0x8000 | 0x0004
							}
						}
					}
				};
				SendInput((uint) mouseUp.Count, mouseUp.ToArray(), Marshal.SizeOf(typeof(INPUT)));
			}
		//	SendMessage(mainWindowHandle, WM_LBUTTONDOWN, (IntPtr) 0, (IntPtr) ((y << 16) | (x & 0xFFFF)));
		//	SendMessage(mainWindowHandle, WM_LBUTTONUP, (IntPtr) 0, (IntPtr) (((y + 1) << 16) | ((x + 1) & 0xFFFF)));
		}

		public RECT GetClientRect() {
			if(mainWindowHandle != null) {
				if(GetClientRect(new HandleRef(this, mainWindowHandle), out RECT rect)) {
					return rect;
				}
			}
			return new RECT();
		}

		public bool GetScreenFromClientPoint(ref POINT point) {
			if(mainWindowHandle != null) {
				return ClientToScreen(new HandleRef(this, mainWindowHandle), ref point);
			}
			return false;
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
