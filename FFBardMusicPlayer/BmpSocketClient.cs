using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocketSharp;
using FFBardMusicCommon;

namespace FFBardMusicPlayer {
	class BmpSocketClient {

		WebSocket client = null;
		bool connected = false;

		public void Connect(int charId, string charName, string charWorld) {

			if((client is WebSocket) && client.IsAlive) {
				client.Close(2);
			}

			UriBuilder uri = new UriBuilder("ws", "localhost", 8082, "client");
			uri.Query = new Dictionary<string, string> {
				{ "charId", charId.ToString() },
				{ "charName", charName },
				{ "charWorld", charWorld }
			}.ToQuery();

			Console.WriteLine(string.Format("Connecting to [{0}]...", uri.ToString()));
			client = new WebSocket(uri.ToString());
			client.ConnectAsync();

			client.OnOpen += Client_OnOpen;
			client.OnMessage += Client_OnMessage;
			client.OnClose += Client_OnClose;
		}

		private void Client_OnOpen(object sender, EventArgs e) {
			Console.WriteLine("Open connection to sync server.");
		}

		private void Client_OnMessage(object sender, MessageEventArgs e) {

		}

		private void Client_OnClose(object sender, CloseEventArgs e) {
			Console.WriteLine("Close connection to sync server.");
		}

		private void Send(BmpPacket packet) {
			if(!connected) {
				return;
			}
			client.Send(BmpPacket.Serialize(packet));
		}

		// Send funcs

		public void SendConductorPacket(string cn) {
			this.Send(new BmpConductorData(cn));
		}
	}
}