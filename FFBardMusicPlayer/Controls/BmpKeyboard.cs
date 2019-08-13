using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;
using System.Drawing.Drawing2D;

namespace FFBardMusicPlayer.Controls {

	public partial class BmpKeyboard : UserControl {

		KeyboardData keyboardData = new KeyboardData();

		private int lowFreq = 0;
		private int highFreq = 0;

		private int maxFreq = 0;
		private SolidBrush fgContrastBrush = new SolidBrush(Color.FromArgb(255, 30, 30, 30));
		private SolidBrush bgContrastBrush = new SolidBrush(Color.FromArgb(100, 200, 200, 200));
		private Font textFont = new Font("Segoe UI", 8);
		private Font textLargeFont = new Font("Segoe UI", 12);
		private StringFormat centerTextFormat = new StringFormat {
			LineAlignment = StringAlignment.Center,
			Alignment = StringAlignment.Center
		};

		[EditorBrowsable(EditorBrowsableState.Always)]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Bindable(true)]
		public override string Text {
			get { return base.Text; }
			set {
				base.Text = value;
				this.Refresh();
			}
		}

		private string overrideText;
		public string OverrideText {
			get { return overrideText; }
			set {
				overrideText = value;
				this.Refresh();
			}
		}

		public BmpKeyboard() {
			InitializeComponent();

			this.DoubleBuffered = true;
			this.ResizeRedraw = true;
		}

		public void UpdateFrequency(List<int> notes) {

			KeyboardLetterList list = keyboardData.letterList;
			int notesAvailable = 0;
			int notesInRange = 0;

			lowFreq = 0;
			highFreq = 0;

			foreach(string key in list.Keys) {
				list[key].frequency = 0;
			}

			foreach(int note in notes) {
				notesAvailable++;
				string perfKey = FFXIVKeybindDat.NoteByteToPerformanceKey((byte) note);
				if(list.ContainsKey(perfKey)) {
					notesInRange++;
					list[perfKey].frequency++;
				} else {
					if(note <= 0) {
						lowFreq++;
					}
					if(note >= 38) {
						highFreq++;
					}
				}
			}

			this.Text = string.Empty;
			if(notesAvailable == 0) {
				this.Text = "No notes playing on this track.";
			} else if(notesInRange == 0) {
				this.Text = "All notes out of range.";
			}

			this.Refresh();
		}

		public void UpdateNoteKeys(FFXIVKeybindDat hotkeys) {
			KeyboardLetterList list = keyboardData.letterList;
			foreach(string key in list.Keys) {
				list[key].key = string.Empty;

				string pk = FFXIVKeybindDat.NoteKeyToPerformanceKey(key);
				if(!string.IsNullOrEmpty(pk)) {
					list[key].key = hotkeys[pk].ToString();
				}
			}
			this.Refresh();
		}

		private void DrawKey(KeyboardUiLetter letter, PaintEventArgs e) {
			string noteKey = string.Empty;
			if(keyboardData.letterList.ContainsValue(letter)) {
				foreach(string k in keyboardData.letterList.Keys) {
					if(keyboardData.letterList[k].Equals(letter)) {
						noteKey = k;
						continue;
					}
				}
			}
			if(string.IsNullOrEmpty(noteKey)) {
				return;
			}

			string drawKey = letter.key; // k = letter.key

			// Determine colors
			SolidBrush keyBrush = new SolidBrush(Color.Transparent);
			if(letter.sup) {
				keyBrush.Color = this.Enabled ? this.ForeColor : Color.FromArgb(50, 50, 50);
			} else {
				keyBrush.Color = this.Enabled ? this.BackColor : Color.FromArgb(180, 180, 180);
			}

			// Determine region
			float ww = this.Width / (float) ((letter.sup ? 26 : 22) - 1);
			SizeF keySize = new SizeF(ww, (letter.y * this.Height));
			PointF keyPosition = new PointF((letter.x * this.Width) - (keySize.Width / 2f), 0f);
			RectangleF keyRect = new RectangleF(keyPosition, keySize);

			// Draw color
			if(letter.sup) {
				e.Graphics.FillRectangle(keyBrush, keyRect);
			}

			// Set clipping and such
			Region clip = e.Graphics.Clip;
			e.Graphics.SetClip(keyRect, System.Drawing.Drawing2D.CombineMode.Replace);

			// Draw key and frequency
			float keyX = keyRect.X + keyRect.Width / 2f;
			float keyY = keyRect.Y + keyRect.Height - 8f;
			if(letter.frequency > 0) {
				int freq = letter.frequency > 0 ? letter.frequency + 60 : 0;
				float size = (freq / 260f);
				if(size > 1f) {
					size = 1f;
				}
				Color freqCol = letter.sup ? Color.Yellow : Color.Red;
				SolidBrush freqBrush = new SolidBrush(Color.FromArgb(50 + (int) (size * 150), freqCol));
				e.Graphics.FillCircle(freqBrush, keyX, (letter.sup ? keyRect.Y : keyRect.Y + keyRect.Height), size * this.Height);
			}

			// Write note
			if(true) {
				SizeF noteKeySize = e.Graphics.MeasureString(drawKey, textFont);
				PointF noteKeyPos = new PointF(keyX - noteKeySize.Width / 2, keyY - noteKeySize.Height / 2);
				e.Graphics.FillRectangle(bgContrastBrush, new RectangleF(noteKeyPos, noteKeySize));
				e.Graphics.DrawString(drawKey, textFont, fgContrastBrush, keyX, keyY, centerTextFormat);
			}

			// Draw line
			if(!letter.sup && !noteKey.Equals("C+2")) {
				PointF p1 = new PointF(keyRect.X + keyRect.Width - 2, keyRect.Y);
				PointF p2 = new PointF(keyRect.X + keyRect.Width - 2, keyRect.Y + keyRect.Height);
				e.Graphics.DrawLine(new Pen(fgContrastBrush), p1, p2);

			}

			e.Graphics.SetClip(clip, System.Drawing.Drawing2D.CombineMode.Replace);

			if(letter.sup) {
				e.Graphics.DrawRectangle(new Pen(fgContrastBrush), Rectangle.Round(keyRect));
			}

		}

		private void KeyboardControl_Paint(object sender, PaintEventArgs e) {

			KeyboardLetterList list = keyboardData.letterList;

			// Get max frequency
			foreach(string key in list.Keys) {
				if(list.ContainsKey(key)) {
					KeyboardUiLetter letter = keyboardData.letterList[key];
					if(letter.frequency > maxFreq) {
						maxFreq = letter.frequency;
					}
				}
			}

			foreach(string key in list.Keys) {
				if(list.ContainsKey(key)) {
					KeyboardUiLetter letter = keyboardData.letterList[key];
					if(!letter.sup) {
						DrawKey(letter, e);
					}
				}
			}
			foreach(string key in list.Keys) {
				if(list.ContainsKey(key)) {
					KeyboardUiLetter letter = keyboardData.letterList[key];
					if(letter.sup) {
						DrawKey(letter, e);
					}
				}
			}


			//SolidBrush freqBrush = new SolidBrush(Color.FromArgb(120, Color.Red));
			if(lowFreq > 0) {
				float f = (lowFreq / 1024f).Clamp(0.01f, 1.0f);
				SizeF freqSize = new SizeF(f * (this.Width / 4), this.Height);
				RectangleF freqRect = new RectangleF(new PointF(0, 0), freqSize);
				LinearGradientBrush lgrad = new LinearGradientBrush(freqRect, Color.Red, Color.Transparent, LinearGradientMode.Horizontal);
				e.Graphics.FillRectangle(lgrad, freqRect);
			}
			if(highFreq > 0) {
				float f = (highFreq / 1024f).Clamp(0.01f, 1.0f);
				SizeF freqSize = new SizeF(f * (this.Width / 4), this.Height);
				RectangleF freqRect = new RectangleF(new PointF(this.Width - freqSize.Width, 0), freqSize);
				LinearGradientBrush lgrad = new LinearGradientBrush(freqRect, Color.Transparent, Color.Red, LinearGradientMode.Horizontal);
				e.Graphics.FillRectangle(lgrad, freqRect);

			}

			RectangleF rect = new RectangleF(0, 0, this.Width, this.Height);
			if(this.Enabled) {
				Pen rectPen = new Pen(Color.Black);
				e.Graphics.DrawRectangle(rectPen, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
			}
			if(true) {
				// Fade out if it's disabled or text is showing
				if(!string.IsNullOrEmpty(this.Text) || !this.Enabled) {
					SolidBrush disabledBrush = new SolidBrush(Color.FromArgb(120, Color.White));
					e.Graphics.FillRectangle(disabledBrush, rect);
				}
				if(!string.IsNullOrEmpty(overrideText)) {
					PointF center = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
					SizeF textSize = e.Graphics.MeasureString(this.overrideText, textLargeFont);
					PointF textPos = new PointF(center.X - textSize.Width / 2, center.Y - textSize.Height / 2);

					e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(80, 30, 30)), new RectangleF(textPos, textSize));
					e.Graphics.DrawString(this.overrideText, textLargeFont, new SolidBrush(Color.White), this.Width / 2, this.Height / 2, centerTextFormat);
				}
				// Show custom message
				else if(!string.IsNullOrEmpty(this.Text)) {
					PointF center = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
					SizeF textSize = e.Graphics.MeasureString(this.Text, textLargeFont);
					PointF textPos = new PointF(center.X - textSize.Width / 2, center.Y - textSize.Height / 2);

					e.Graphics.FillRectangle(new SolidBrush(Color.Black), new RectangleF(textPos, textSize));
					e.Graphics.DrawString(this.Text, textLargeFont, new SolidBrush(Color.White), this.Width / 2, this.Height / 2, centerTextFormat);
				}
			}
		}
	}

	public class KeyboardUiLetter {
		public int frequency = 0;
		public string key = string.Empty;
		public float x;
		public float y;
		public bool sup;
		public KeyboardUiLetter(float xx, float yy, bool s = false) {
			x = xx;
			y = yy;
			sup = s;
		}
	}
	public class KeyboardLetterList : Dictionary<string, KeyboardUiLetter> { }

	public class KeyboardData {
		public KeyboardLetterList letterList = new KeyboardLetterList();

		public KeyboardData() {
			string[] mainKeys = {
				"C-1", "D-1", "E-1", "F-1", "G-1", "A-1", "B-1",
				"C",   "D",   "E",   "F",   "G",   "A",   "B",
				"C+1", "D+1", "E+1", "F+1", "G+1", "A+1", "B+1", "C+2",
			};
			string[] supKeys = {
				"C#-1", "Eb-1", "F#-1", "G#-1", "Bb-1",
				"C#",   "Eb",   "F#",   "G#",   "Bb",
				"C#+1", "Eb+1", "F#+1", "G#+1", "Bb+1",
			};
			int nk = 22;
			float ww = (1f / (nk));
			for(int i = 0; i < nk; i++) {
				letterList.Add(mainKeys[i], new KeyboardUiLetter((i * (ww)) + (ww / 2f), 1.0f, false));
			}
			nk = 15;
			float add = 0;
			for(int i = 0; i < nk; i++) {
				if((i % 5 == 2) || (i % 5) == 0) {
					add += ww;
				}
				letterList.Add(supKeys[i], new KeyboardUiLetter((i * (ww)) + (add), 0.7f, true));
			}
		}
	}
}
