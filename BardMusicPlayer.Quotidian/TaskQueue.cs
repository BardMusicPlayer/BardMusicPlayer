/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayer/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Quotidian;

public class TaskQueue
{
    private Task previous = Task.FromResult(false);
    private readonly object key = new();

    public Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
    {
        lock (key)
        {
            var next = previous.ContinueWith(_ => taskGenerator()).Unwrap();
            previous = next;
            return next;
        }
    }

    public Task Enqueue(Func<Task> taskGenerator)
    {
        lock (key)
        {
            var next = previous.ContinueWith(_ => taskGenerator()).Unwrap();
            previous = next;
            return next;
        }
    }
}