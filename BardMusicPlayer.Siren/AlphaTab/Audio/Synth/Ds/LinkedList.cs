/*
 * Copyright(c) 2021 Daniel Kuschny
 * Licensed under the MPL-2.0 license. See https://github.com/CoderLine/alphaTab/blob/develop/LICENSE for full license information.
 */

namespace BardMusicPlayer.Siren.AlphaTab.Audio.Synth.Ds;

internal class LinkedList<T> where T : class
{
    public LinkedListNode<T> First { get; set; }

    public int Length { get; private set; }

    public LinkedList()
    {
        Length = 0;
    }

    public void AddFirst(T value)
    {
        var node = new LinkedListNode<T>
        {
            Value = value
        };
        if (First == null)
        {
            InsertNodeToEmptyList(node);
        }
        else
        {
            InsertNodeBefore(First, node);
            First = node;
        }
    }

    public void AddLast(T value)
    {
        var node = new LinkedListNode<T>
        {
            Value = value
        };
        if (First == null)
        {
            InsertNodeToEmptyList(node);
        }
        else
        {
            InsertNodeBefore(First, node);
        }
    }

    public T RemoveFirst()
    {
        if (First == null)
        {
            return null;
        }

        var v = First.Value;
        Remove(First);
        return v;
    }

    public T RemoveLast()
    {
        if (First == null)
        {
            return null;
        }

        var v = First.PrevInternal?.Value;
        Remove(First.PrevInternal);
        return v;
    }

    public void Remove(LinkedListNode<T> n)
    {
        if (n.NextInternal == n)
        {
            First = null;
        }
        else
        {
            n.NextInternal.PrevInternal = n.PrevInternal;
            n.PrevInternal.NextInternal = n.NextInternal;
            if (First == n)
            {
                First = n.NextInternal;
            }
        }

        n.Invalidate();
        Length--;
    }

    private void InsertNodeBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
    {
        newNode.NextInternal           = node;
        newNode.PrevInternal           = node.PrevInternal;
        node.PrevInternal.NextInternal = newNode;
        node.PrevInternal              = newNode;
        newNode.List                   = this;
        Length++;
    }

    private void InsertNodeToEmptyList(LinkedListNode<T> node)
    {
        node.NextInternal = node;
        node.PrevInternal = node;
        node.List         = this;
        First             = node;
        Length++;
    }
    
    private async Task ProcessDataLoop(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                _connectionManager.Refresh();
                ProcessNetworkData();
                await Task.Delay(30, token);
            }
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation
        }
        catch (Exception ex)
        {
            Trace.WriteLine("TCPNetworkMonitor Error in ProcessDataLoop: " + ex.ToString(), "DEBUG-MACHINA");
        }
    }

    private void ProcessNetworkData()
    {
        foreach (var connection in _connectionManager.Connections)
        {
            using var socket = connection.Socket;
            CapturedData data;
            while ((data = socket.Receive()).Size > 0)
            {
                connection.IPDecoderSend.FilterAndStoreData(data.Buffer, data.Size);
                byte[] tcpbuffer;
                while ((tcpbuffer = connection.IPDecoderSend.GetNextIPPayload()) != null)
                {
                    connection.TCPDecoderSend.FilterAndStoreData(tcpbuffer);
                    while (connection.TCPDecoderSend.GetNextTCPDatagram() is { } payloadBuffer)
                        OnDataSent(connection, payloadBuffer);
                }

                connection.IPDecoderReceive.FilterAndStoreData(data.Buffer, data.Size);
                while ((tcpbuffer = connection.IPDecoderReceive.GetNextIPPayload()) != null)
                {
                    connection.TCPDecoderReceive.FilterAndStoreData(tcpbuffer);
                    while (connection.TCPDecoderReceive.GetNextTCPDatagram() is { } payloadBuffer)
                        OnDataReceived(connection, payloadBuffer);
                }
            }
        }
    }
}

internal class LinkedListNode<T> where T : class
{
    internal LinkedList<T> List;
    internal LinkedListNode<T> NextInternal;
    internal LinkedListNode<T> PrevInternal;

    public T Value { get; set; }

    public LinkedListNode<T> Next => NextInternal == null || List.First == NextInternal ? null : NextInternal;

    public LinkedListNode<T> Prev => PrevInternal == null || this == List.First ? null : PrevInternal;

    public void Invalidate()
    {
        List         = null;
        NextInternal = null;
        PrevInternal = null;
    }
}