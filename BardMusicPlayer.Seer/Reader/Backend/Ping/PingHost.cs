using System;
using System.Text;
using System.Net.NetworkInformation;
using System.Threading;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Seer.Reader.Backend.Ping;

public class PingHost
{
    public static Game _game { get; private set; }

    public PingHost(string args, Game game)
    {
        if (args.Length == 0)
            throw new ArgumentException("Ping needs a host or IP Address.");
        _game = game;
        var waiter = new AutoResetEvent(false);

        var pingSender = new System.Net.NetworkInformation.Ping();

        // When the PingCompleted event is raised,
        // the PingCompletedCallback method is called.
        pingSender.PingCompleted += PingCompletedCallback;

        // Create a buffer of 32 bytes of data to be transmitted.
        const string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        var buffer = Encoding.ASCII.GetBytes(data);

        const int timeout = 12000;
        var options = new PingOptions(64, true);

        Console.WriteLine(@"Time to live: {0}", options.Ttl);
        Console.WriteLine(@"Don't fragment: {0}", options.DontFragment);

        // Send the ping asynchronously.
        // Use the waiter as the user token.
        // When the callback completes, it can wake up this thread.
        pingSender.SendAsync(args, timeout, buffer, options, waiter);

        // Prevent this example application from ending.
        // A real application should do something useful
        // when possible.
        waiter.WaitOne();
    }

    private static void PingCompletedCallback(object sender, PingCompletedEventArgs e)
    {
        // If the operation was canceled, display a message to the user.
        if (e.Cancelled)
            ((AutoResetEvent)e.UserState).Set();

        // If an error occurred, display the exception to the user.
        if (e.Error != null)
            ((AutoResetEvent)e.UserState).Set();

        var reply = e.Reply;

        DisplayReply(reply);

        // Let the main thread resume.
        ((AutoResetEvent)e.UserState).Set();
    }

    public static void DisplayReply(PingReply reply)
    {
        if (reply == null)
            return;

        Console.WriteLine(@"ping status: {0}", reply.Status);
        if (reply.Status == IPStatus.Success)
        {
            Console.WriteLine(@"Address: {0}", reply.Address);
            Console.WriteLine(@"RoundTrip time: {0}", reply.RoundtripTime);
            Console.WriteLine(@"Time to live: {0}", reply.Options.Ttl);
            Console.WriteLine(@"Don't fragment: {0}", reply.Options.DontFragment);
            Console.WriteLine(@"Buffer size: {0}", reply.Buffer.Length);
            _game.PublishEvent(new LatencyUpdate(EventSource.Machina, reply.RoundtripTime));
        }
    }
}