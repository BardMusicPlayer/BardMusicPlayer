using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WebSocketSharp;
using WebSocketSharp.Server;

namespace FFBardMusicServer {


	class BmpCharacter {
		public int characterId = 0;
		public string characterName = string.Empty;

		public int conductorId = 0;
		public string conductorName = string.Empty;

		public BmpWorld characterWorld;
		public WebSocket socket;
	}

	class BmpWorld {

		public List<BmpCharacter> characterList = new List<BmpCharacter>();
		public string worldName;
		public string dataCenter;

		public BmpWorld(string w, string dc) {
			worldName = w;
			dataCenter = dc;
		}

		public void Add(BmpCharacter c) {
			c.characterWorld = this;
			characterList.Add(c);
		}
		public void Remove(BmpCharacter c) {
			characterList.Remove(c);
		}
	}

	class BmpWorlds {
		Dictionary<string, BmpWorld> worlds = new Dictionary<string, BmpWorld>();

		public BmpWorld this[string world] {
			get {
				if(worlds.ContainsKey(world)) {
					return worlds[world];
				}
				return null;
			}
		}

		public List<BmpWorld> GetWorlds() {
			return worlds.Values.ToList();
		}

		public BmpWorlds() {
			List<string> aetherServers = new List<string> {
				"Adamantoise", "Balmung", "Cactuar", "Coeurl", "Faerie",
				"Gilgamesh", "Goblin", "Jenova", "Mateus", "Midgardsormr",
				"Sargatanas", "Siren", "Zalera"
			};
			List<string> primalServers = new List<string> {
				"Behemoth", "Brynhildr", "Diabolos", "Excalibur", "Exodus",
				"Famfrit", "Hyperion", "Lamia", "Leviathan", "Malboro", "Ultros"
			};
			List<string> chaosServers = new List<string> {
				"Cerberus", "Lich", "Louisoix", "Moogle", "Odin",
				"Omega", "Phoenix", "Ragnarok", "Shiva", "Zodiark"
			};

			foreach(string world in aetherServers) {
				worlds[world] = new BmpWorld(world, "Aether");
			}
			foreach(string world in primalServers) {
				worlds[world] = new BmpWorld(world, "Primal");
			}
			foreach(string world in chaosServers) {
				worlds[world] = new BmpWorld(world, "Chaos");
			}
		}
	}

	class Program {

		public static BmpWorlds worlds = new BmpWorlds();

		static void Main(string[] args) {
			WebSocketServer ws = new WebSocketServer(8082);
			ws.AddWebSocketService<BmpSocketServer>("/client");
			ws.Start();
			Console.WriteLine(string.Format("Starting server on [{0}]", ws.Address.ToString()));
			
			while(ws.IsListening) {
				Console.Write("?> ");

				string input = Console.ReadLine();
				if(string.IsNullOrEmpty(input)) continue;

				switch(input) {
					case "q":
					case "quit":
					case "stop": {
						ws.Stop();
						break;
					}
					case "players": {
						foreach(BmpWorld world in worlds.GetWorlds()) {
							if(world.characterList.Count == 0) {
								continue;
							}
							Console.WriteLine(string.Format("{0} ({1})", world.worldName, world.characterList.Count));
							foreach(BmpCharacter character in world.characterList) {
								string name = character.characterName;
								string ip = character.socket.Url.Host;
								Console.WriteLine(string.Format(" - {0} ({1})", name, ip));
							}
						}
						break;
					}
				}
			}
		}

	}


	public class BmpSocketServer : WebSocketBehavior {

		private class SocketCharacterDictionary : Dictionary<WebSocket, BmpCharacter> { }
		private SocketCharacterDictionary socketToCharacter = new SocketCharacterDictionary();

		protected override void OnOpen() {

			int charId = int.Parse(Context.QueryString["charId"]);
			string charName = Context.QueryString["charName"];
			string charWorld = Context.QueryString["charWorld"];
			
			if(string.IsNullOrEmpty(charName) || string.IsNullOrEmpty(charWorld) || charId == 0) {
				return;
			}
			
			if(Program.worlds[charWorld] is BmpWorld world) {
				var socket = this.Context.WebSocket;
				socketToCharacter[socket] = new BmpCharacter {
					characterId = charId,
					characterName = charName,
					socket = socket,
				};
				world.Add(socketToCharacter[socket]);
				Console.WriteLine(string.Format("\n[{0} ({1})] connected to sync server.", charName, charId));
			}
		}

		protected override void OnClose(CloseEventArgs e) {
			
			if(socketToCharacter.TryGetValue(this.Context.WebSocket, out BmpCharacter character)) {
				socketToCharacter.Remove(character.socket);
				character.characterWorld.Remove(character);
				Console.WriteLine(string.Format("\n[{0}] disconnected.", character.characterName));
			}
		}

		protected override void OnMessage(MessageEventArgs e) {
			if(socketToCharacter.TryGetValue(this.Context.WebSocket, out BmpCharacter character)) {
				Console.WriteLine(string.Format("[{0}]: {1}", character.characterName, e.Data));
			}
		}
	}
}
