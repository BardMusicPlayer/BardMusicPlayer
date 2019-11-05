using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFBardMusicPlayer.Components {
	public partial class BmpChatLog : RichTextBox {

		#region API Stuff

		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		private static extern IntPtr SendMessage(IntPtr hWnd, Int32 wMsg, Int32 wParam, ref Point lParam);

		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		public static extern bool ReleaseCapture();

		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		public static extern int GetScrollPos(IntPtr hWnd, int nBar);

		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		private static extern bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);

		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		public static extern int RegisterWindowMessage(string message);

		private const int WM_USER = 0x400;
		private const int SB_HORZ = 0x0;
		private const int SB_VERT = 0x1;
		private const int EM_SETSCROLLPOS = WM_USER + 222;
		private const int EM_GETSCROLLPOS = WM_USER + 221;
		private const int WM_NCLBUTTONDOWN = 0xA1;
		private const int HT_CAPTION = 0x2;
		private const int WM_SETREDRAW = 11;

		public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");

		#endregion


		public BmpChatLog() {
			InitializeComponent();
		}

		public BmpChatLog(IContainer container) {
			container.Add(this);

			InitializeComponent();
		}
		public void AppendRtf(string format) {
			this.AddRtf(this, format);
		}

		delegate void AddRtfCallback(RichTextBox box, string format);
		private void AddRtf(RichTextBox box, string format) {
			if(box.Disposing)
				return;
			if(box.InvokeRequired) {
				this.Invoke(new AddRtfCallback(AddRtf), new object[] { box, format });
				return;
			}

			Point rtfPoint = Point.Empty;
			SendMessage(box.Handle, EM_GETSCROLLPOS, 0, ref rtfPoint);
			GetScrollRange((IntPtr) box.Handle, SB_VERT, out int vmin, out int vmax);

			int pos = rtfPoint.Y + box.ClientSize.Height;
			bool bottom = (pos >= vmax);

			int start = box.SelectionStart;
			int len = box.SelectionLength;

			SendMessage(box.Handle, WM_SETREDRAW, 0, 0);

			box.SelectionStart = box.TextLength;
			box.SelectionLength = 0;
			box.SelectedRtf = format;
			box.AppendText(System.Environment.NewLine);

			if(bottom) {
				//GetScrollRange((IntPtr) box.Handle, SB_VERT, out vmin, out vmax);
				//rtfPoint.Y = vmax - box.ClientSize.Height;
				//SendMessage(box.Handle, EM_SETSCROLLPOS, 0, ref rtfPoint);
				box.ScrollToCaret();

			} else {
				SendMessage(box.Handle, EM_SETSCROLLPOS, 0, ref rtfPoint);
			}

			box.Select(start, len);

			SendMessage(box.Handle, WM_SETREDRAW, 1, 0);
			box.Refresh();

		}
	}
}
