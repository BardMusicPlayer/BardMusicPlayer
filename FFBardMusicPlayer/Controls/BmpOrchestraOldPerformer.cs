using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Timer = System.Threading.Timer;
using Sharlayan;
using Sharlayan.Core;

namespace FFBardMusicPlayer {
	public partial class PerformerControl : UserControl {

		public EventHandler<PerformerControl> OnPerformerClick;

		public EventHandler<int> OnTrackNumChange;
		public EventHandler<float> OnSpeedShiftChange;
		public EventHandler<int> OnOctaveShiftChange;

		private int id = 0;
		public int Id {
			get {
				return id;
			}
			set {
				id = value;
			}
		}
		private string charName = string.Empty;
		public string CharName {
			set {
				charName = value;
				MemberNameLabel.Text = value;
			}
			get {
				return charName;
			}
		}

		private float delay = 0;
		public float Delay {
			get {
				return delay;
			}
			set {
				delay = value;
				this.Invoke(t => t.Refresh());
			}
		}

		private float speedShift = 0.0f;
		public float SpeedShift {
			get { return speedShift; }
			set {
				speedShift = value;
			}
		}
		public void SetSpeedShift(float ss) {
			SpeedShift = ss;
			OnSpeedShiftChange?.Invoke(this, ss);
		}

		private int octaveShift = 0;
		public int OctaveShift {
			get { return octaveShift; }
			set {
				octaveShift = value;
			}
		}
		public void SetOctaveShift(int os) {
			OctaveShift = os;
			OnOctaveShiftChange?.Invoke(this, os);
		}

		private int trackNum = 0;
		public int TrackNum {
			get {
				return trackNum;
			}
			set {
				trackNum = value;
				TrackNumLabel.Invoke(t => t.Text = string.Format("{0}", trackNum));
			}
		}
		public void SetTrackNum(int tn) {
			TrackNum = tn;
			OnTrackNumChange?.Invoke(this, tn);
		}
		public TrackSyncState trackSyncs = new TrackSyncState();


		public string JobName {
			set {
				MemberJobLabel.Text = value;
				MemberNameLabel.ForeColor = (value.Equals("BRD") ? Color.White : Color.Gray);
			}
		}
		public PerformerControl(PerformerControl perf = null) {
			InitializeComponent();
			MemberNameLabel.Click += MemberNameLabel_Click;

			SpeedShift = 1.0f;
			OctaveShift = 0;
			TrackNum = 0;
		}

		private void MemberNameLabel_Click(object sender, EventArgs e) {
			OnPerformerClick?.Invoke(this, this);
		}

		public void Update(ActorItemBase actor) {

			if(actor is ActorItemBase) {
				this.Name = actor.Name;
				this.JobName = actor.Job.ToString();
			}
		}

		private void PerformerControl_Paint(object sender, PaintEventArgs e) {
			float pct = (delay / 500f);

			if(Math.Abs(pct) > 0.01) {
				PointF p1 = new PointF(MemberNameLabel.Location.X + MemberNameLabel.Width / 2, MemberNameLabel.Location.Y + MemberNameLabel.Height);
				PointF p2 = new PointF(p1.X + (MemberNameLabel.Width / 2 * pct), p1.Y);

				Pen pen = new Pen(this.ForeColor);
				e.Graphics.DrawLine(pen, p1, p2);
				e.Graphics.DrawLine(pen, p1, new PointF(p1.X, p1.Y + 4));
			}
		}

		/*
        private void AddMemberButton_Click(object sender, EventArgs e) {
            bool shiftKey = (Control.ModifierKeys & Keys.Shift) != 0;
            if(shiftKey) {
                this.Add(new PerformerControl {
                    Id = 0,
                    CharName = "Nare Katol",
                });
                return;
            }

            if(Reader.CanGetActors()) {

                uint me_id = 0;
                ActorResult res = Reader.GetActors();
                PointF point1 = new PointF();
                if(conductorId != 0) {
                    if(res.CurrentPCs.ContainsKey(conductorId)) {
                        ActorItem actor = res.CurrentPCs[conductorId];
                        me_id = actor.ID;
                        point1 = new PointF((float) actor.X, (float) actor.Z);
                    }
                }
                if(me_id != 0) {
                    NearMeMenu.DropDownItems.Clear();
                    PartyMemberMenu.DropDownItems.Clear();

                    List<uint> partyMembers = new List<uint>();
                    if(Reader.CanGetPartyMembers()) {
                        PartyResult res2 = Reader.GetPartyMembers();
                        partyMembers = res2.PartyMembers.Keys.ToList();
                    }
                    foreach(ActorItem actor in res.CurrentPCs.Values) {
                        if(actor.ID == me_id || actor.ID == conductorId) {
                            continue;
                        }
                        PointF point2 = new PointF((float) actor.X, (float) actor.Z);
                        if(point1.GetDistance(point2) < 20) {
                            NearMeMenu.DropDownItems.Add(ActorToMenuItem(actor));
                        }
                        if(partyMembers.Contains(actor.ID)) {
                            PartyMemberMenu.DropDownItems.Add(ActorToMenuItem(actor));
                        }
                    }
                }
            }
            AddActorMenu.Show(Cursor.Position);
        }

        private void RefreshPartyButton_Click(object sender, EventArgs e) {

        }
        */
	}

	public class TrackSyncStep {

		// The performance key
		public string perfNote = string.Empty;
		public int note = 0;
		// Timestamp
		public long ms = 0;
		// Is performer or not
		public bool performer = false;
	}
	public class TrackSyncState : List<TrackSyncStep> {

	}
}
