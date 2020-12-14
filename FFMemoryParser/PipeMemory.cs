using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMemoryParser {
	public class PipeMemory : Memory {
		NamedPipeServer<PipeData> dataPipe = null;
		public PipeMemory(Process proc) : base(proc) {
			string pipename = string.Format("BmpPipe{0}", proc.Id);
			dataPipe = new NamedPipeServer<PipeData>(pipename);
			dataPipe.ClientConnected += ClientConnected;
			dataPipe.Start();
			Console.WriteLine(string.Format("Pipe name: {0}", pipename));
		}
		private void ClientConnected(NamedPipeConnection<PipeData, PipeData> connection) {
			Console.WriteLine("Client connected");
			this.ResetCache();
			connection.PushMessage(CreatePipeData(notFoundSignatures));
		}
		public PipeData CreatePipeData(object obj) {
			return new PipeData(obj.GetType().ToString(), obj.ToByteArray());
		}

		public override void OnDataUpdate(object data) {
			base.OnDataUpdate(data);
			dataPipe.PushMessage(CreatePipeData(data));
		}
	}
}
