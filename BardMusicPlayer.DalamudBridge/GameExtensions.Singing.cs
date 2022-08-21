using System.Threading;
using System.Threading.Tasks;
using BardMusicPlayer.DalamudBridge.Helper.Dalamud;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Seer;

namespace BardMusicPlayer.DalamudBridge
{
    public static partial class GameExtensions
    {
        private static readonly SemaphoreSlim LyricSemaphoreSlim = new (1,1);

        /// <summary>
        /// Sands a lyric line via say
        /// </summary>
        /// <param name="game"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Task<bool> SendLyricLine(this Game game, string text)
        {
            if (!DalamudBridge.Instance.Started) throw new DalamudBridgeException("DalamudBridge not started.");

            if (DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid))
                return Task.FromResult(DalamudBridge.Instance.DalamudServer.SendChat(game.Pid, ChatMessageChannelType.Say, text));
            return Task.FromResult(false);
        }

        public static Task<bool> SendText(this Game game, ChatMessageChannelType type, string text)
        {
            if (!DalamudBridge.Instance.Started) throw new DalamudBridgeException("DalamudBridge not started.");

            if (DalamudBridge.Instance.DalamudServer.IsConnected(game.Pid))
                return Task.FromResult(DalamudBridge.Instance.DalamudServer.SendChat(game.Pid, type, text));
            return Task.FromResult(false);
        }
    }
}
