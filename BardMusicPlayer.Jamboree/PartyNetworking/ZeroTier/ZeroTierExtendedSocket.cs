using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using ZeroTier;
using SocketException = ZeroTier.Sockets.SocketException;

namespace BardMusicPlayer.Jamboree.PartyNetworking.ZeroTier;

public class ZeroTierExtendedSocket
{
    /// <summary>No error.</summary>
    public static readonly int ZTS_ERR_OK = 0;
    /// <summary>Socket error, see Socket.ErrNo() for additional context.</summary>
    public static readonly int ZTS_ERR_SOCKET = -1;
    /// <summary>You probably did something at the wrong time.</summary>
    public static readonly int ZTS_ERR_SERVICE = -2;
    /// <summary>Invalid argument.</summary>
    public static readonly int ZTS_ERR_ARG = -3;
    /// <summary>No result. (not necessarily an error.)</summary>
    public static readonly int ZTS_ERR_NO_RESULT = -4;
    /// <summary>Consider filing a bug report.</summary>
    public static readonly int ZTS_ERR_GENERAL = -5;

    private int _fd;
    private bool _isClosed;
    private bool _isListening;
    internal EndPoint _localEndPoint;
    internal EndPoint _remoteEndPoint;

    private void InitializeInternalFlags()
    {
        _isClosed    = false;
        _isListening = false;
    }

    public ZeroTierExtendedSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
    {
        var family = -1;
        var type = -1;
        var protocol = -1;
        // Map .NET socket parameters to ZeroTier equivalents
        family = addressFamily switch
        {
            AddressFamily.InterNetwork   => Constants.AF_INET,
            AddressFamily.InterNetworkV6 => Constants.AF_INET6,
            AddressFamily.Unknown        => Constants.AF_UNSPEC,
            _                            => family
        };
        type = socketType switch
        {
            SocketType.Stream => Constants.SOCK_STREAM,
            SocketType.Dgram  => Constants.SOCK_DGRAM,
            _                 => type
        };
        protocol = protocolType switch
        {
            ProtocolType.Udp         => Constants.IPPROTO_UDP,
            ProtocolType.Tcp         => Constants.IPPROTO_TCP,
            ProtocolType.Unspecified => 0 // ?
            ,
            _ => protocol
        };
        if ((_fd = zts_bsd_socket(family, type, protocol)) < 0)
        {
            throw new SocketException(_fd);
        }
        AddressFamily = addressFamily;
        SocketType    = socketType;
        ProtocolType  = protocolType;
        InitializeInternalFlags();
    }

    private ZeroTierExtendedSocket(
        int fileDescriptor,
        AddressFamily addressFamily,
        SocketType socketType,
        ProtocolType protocolType,
        EndPoint localEndPoint,
        EndPoint remoteEndPoint)
    {
        AddressFamily   = addressFamily;
        SocketType      = socketType;
        ProtocolType    = protocolType;
        _localEndPoint  = localEndPoint;
        _remoteEndPoint = remoteEndPoint;
        _fd             = fileDescriptor;
        InitializeInternalFlags();
    }

    public void Connect(IPEndPoint remoteEndPoint)
    {
        if (_isClosed)
        {
            throw new ObjectDisposedException("Socket has been closed");
        }
        if (_fd < 0)
        {
            // Invalid file descriptor
            throw new SocketException(Constants.ERR_SOCKET);
        }
        if (remoteEndPoint == null)
        {
            throw new ArgumentNullException(nameof(remoteEndPoint));
        }
        var err = zts_connect(_fd, remoteEndPoint.Address.ToString(), (ushort)remoteEndPoint.Port, ConnectTimeout);
        if (err < 0)
        {
            throw new SocketException(err, global::ZeroTier.Core.Node.ErrNo);
        }
        _remoteEndPoint = remoteEndPoint;
        Connected       = true;
    }

    public void Bind(IPEndPoint localEndPoint)
    {
        if (_isClosed)
        {
            throw new ObjectDisposedException("Socket has been closed");
        }
        if (_fd < 0)
        {
            // Invalid file descriptor
            throw new SocketException(Constants.ERR_SOCKET);
        }
        if (localEndPoint == null)
        {
            throw new ArgumentNullException(nameof(localEndPoint));
        }

        var err = localEndPoint.AddressFamily switch
        {
            AddressFamily.InterNetwork   => zts_bind(_fd, "0.0.0.0", (ushort)localEndPoint.Port),
            AddressFamily.InterNetworkV6 => zts_bind(_fd, "::", (ushort)localEndPoint.Port),
            _                            => Constants.ERR_OK
        };
        if (err < 0)
        {
            throw new SocketException(err);
        }
        _localEndPoint = localEndPoint;
        IsBound        = true;
    }

    /// <summary>
    /// The bsd_bind used for broadcasting
    /// </summary>
    /// <param name="localEndPoint"></param>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="SocketException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public void BSD_Bind(IPEndPoint localEndPoint)
    {
        if (_isClosed)
        {
            throw new ObjectDisposedException("Socket has been closed");
        }
        if (_fd < 0)
        {
            // Invalid file descriptor
            throw new SocketException(Constants.ERR_SOCKET);
        }
        if (localEndPoint == null)
        {
            throw new ArgumentNullException(nameof(localEndPoint));
        }
        int err = Constants.ERR_OK;
        var broadcast = new zts_sockaddr_in
        {
            sin_family = (byte)Constants.AF_INET,
            sin_addr   = BitConverter.GetBytes(BitConverter.ToInt32(localEndPoint.Address.GetAddressBytes(), 0)),
            sin_port   = (short)localEndPoint.Port
        };

        var bcPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(zts_sockaddr)));
        Marshal.StructureToPtr(broadcast, bcPtr, false);

        err = zts_bsd_bind(_fd, bcPtr, (ushort)Marshal.SizeOf(typeof(zts_sockaddr)));
        if (err < 0)
        {
            var t = ErrNo;
            Console.WriteLine(t);
            throw new SocketException(err);
        }
        _localEndPoint = localEndPoint;
        IsBound        = true;
        Marshal.FreeHGlobal(bcPtr);
    }

    public void Listen(int backlog)
    {
        if (_isClosed)
        {
            throw new ObjectDisposedException("Socket has been closed");
        }
        if (_fd < 0)
        {
            // Invalid file descriptor
            throw new SocketException(Constants.ERR_SOCKET);
        }
        int err = Constants.ERR_OK;
        if ((err = zts_bsd_listen(_fd, backlog)) < 0)
        {
            // Invalid backlog value perhaps?
            throw new SocketException(Constants.ERR_SOCKET);
        }
        _isListening = true;
    }

    public ZeroTierExtendedSocket Accept()
    {
        if (_isClosed)
        {
            throw new ObjectDisposedException("Socket has been closed");
        }
        if (_fd < 0)
        {
            // Invalid file descriptor
            throw new SocketException(Constants.ERR_SOCKET);
        }
        if (_isListening == false)
        {
            throw new InvalidOperationException("Socket is not in a listening state. Call Listen() first");
        }
        var lpBuffer = Marshal.AllocHGlobal(Constants.INET6_ADDRSTRLEN);
        var port = 0;
        var accepted_fd = zts_accept(_fd, lpBuffer, Constants.INET6_ADDRSTRLEN, ref port);
        // Convert buffer to managed string
        var str = Marshal.PtrToStringAnsi(lpBuffer);
        Marshal.FreeHGlobal(lpBuffer);
        lpBuffer = IntPtr.Zero;
        var clientEndPoint = new IPEndPoint(IPAddress.Parse(str ?? string.Empty), port);
        // Create new socket by providing file descriptor returned from zts_bsd_accept call.
        var clientSocket =
            new ZeroTierExtendedSocket(accepted_fd, AddressFamily, SocketType, ProtocolType, _localEndPoint, clientEndPoint);
        return clientSocket;
    }

    public void Shutdown(SocketShutdown how)
    {
        if (_isClosed)
        {
            throw new ObjectDisposedException("Socket has been closed");
        }

        var ztHow = how switch
        {
            SocketShutdown.Receive => Constants.O_RDONLY,
            SocketShutdown.Send    => Constants.O_WRONLY,
            SocketShutdown.Both    => Constants.O_RDWR,
            _                      => 0
        };
        zts_bsd_shutdown(_fd, ztHow);
    }

    public void Close(int timeout = 0)
    {
        // TODO: Timeout needs to be implemented
        if (_isClosed)
        {
            throw new ObjectDisposedException("Socket has already been closed");
        }
        zts_bsd_close(_fd);
        _isClosed = true;
    }

    public bool Blocking
    {
        get => Convert.ToBoolean(zts_get_blocking(_fd));
        set => zts_set_blocking(_fd, Convert.ToInt32(value));
    }

    public bool reuse_addr
    {
        get => Convert.ToBoolean(zts_get_reuse_addr(_fd));
        set => zts_set_reuse_addr(_fd, Convert.ToInt32(value));
    }

    public bool Poll(int microSeconds, SelectMode mode)
    {
        if (_isClosed)
        {
            throw new ObjectDisposedException("Socket has been closed");
        }
        var poll_set = new zts_pollfd
        {
            fd = _fd
        };
        poll_set.events = mode switch
        {
            SelectMode.SelectRead  => Constants.POLLIN,
            SelectMode.SelectWrite => Constants.POLLOUT,
            SelectMode.SelectError => (short)((byte)Constants.POLLERR | (byte)Constants.POLLNVAL),
            _                      => poll_set.events
        };
        var poll_fd_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(zts_pollfd)));
        Marshal.StructureToPtr(poll_set, poll_fd_ptr, false);
        var result = 0;
        var timeout_ms = microSeconds / 1000;
        uint numfds = 1;
        if ((result = zts_bsd_poll(poll_fd_ptr, numfds, timeout_ms)) < 0)
        {
            throw new SocketException(result, global::ZeroTier.Core.Node.ErrNo);
        }
        poll_set = (zts_pollfd)Marshal.PtrToStructure(poll_fd_ptr, typeof(zts_pollfd));
        if (result != 0)
        {
            result = mode switch
            {
                SelectMode.SelectRead  => Convert.ToInt32(((byte)poll_set.revents & (byte)Constants.POLLIN) != 0),
                SelectMode.SelectWrite => Convert.ToInt32(((byte)poll_set.revents & (byte)Constants.POLLOUT) != 0),
                SelectMode.SelectError => Convert.ToInt32((poll_set.revents & (byte)Constants.POLLERR) != 0 ||
                                                          (poll_set.revents & (byte)Constants.POLLNVAL) != 0),
                _ => result
            };
        }
        Marshal.FreeHGlobal(poll_fd_ptr);
        return result > 0;
    }

    public int Send(byte[] buffer)
    {
        return Send(buffer, 0, buffer?.Length ?? 0, SocketFlags.None);
    }

    public int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags)
    {
        if (_isClosed)
        {
            throw new ObjectDisposedException("Socket has been closed");
        }
        if (_fd < 0)
        {
            throw new SocketException(Constants.ERR_SOCKET);
        }
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }
        if (size < 0 || size > buffer.Length - offset)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }
        if (offset < 0 || offset > buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }
        const int flags = 0;
        var bufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
        return zts_bsd_send(_fd, bufferPtr + offset, (uint)Buffer.ByteLength(buffer), flags);
    }

    /// <summary>
    /// Used for UDP broadcast
    /// </summary>
    /// <param name="localEndPoint"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public int SendTo(IPEndPoint localEndPoint, byte[] buffer)
    {
        return SendTo(localEndPoint, buffer, 0, buffer?.Length ?? 0, SocketFlags.None);
    }

    /// <summary>
    /// Used for UDP broadcast
    /// </summary>
    /// <param name="localEndPoint"></param>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    /// <param name="socketFlags"></param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="SocketException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public int SendTo(IPEndPoint localEndPoint, byte[] buffer, int offset, int size, SocketFlags socketFlags)
    {
        if (_isClosed)
        {
            throw new ObjectDisposedException("Socket has been closed");
        }
        if (_fd < 0)
        {
            throw new SocketException(Constants.ERR_SOCKET);
        }
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }
        if (size < 0 || size > buffer.Length - offset)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }
        if (offset < 0 || offset > buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }
        var broadcast = new zts_sockaddr_in
        {
            sin_family = (byte)Constants.AF_INET,
            sin_addr   = BitConverter.GetBytes(BitConverter.ToInt32(localEndPoint.Address.GetAddressBytes(), 0)),
            sin_port   = (short)localEndPoint.Port
        };

        var bcPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(zts_sockaddr)));
        Marshal.StructureToPtr(broadcast, bcPtr, false);

        const int flags = 0;
        var bufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
        var er = zts_bsd_sendto(_fd, bufferPtr + offset, (uint)Buffer.ByteLength(buffer), flags, bcPtr, (ushort)Marshal.SizeOf(typeof(zts_sockaddr)));
        Marshal.FreeHGlobal(bcPtr);
        return er;
    }

    public int Available => zts_get_data_available(_fd);

    public int Receive(byte[] buffer)
    {
        return Receive(buffer, 0, buffer?.Length ?? 0, SocketFlags.None);
    }

    public int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags)
    {
        if (_isClosed)
        {
            throw new ObjectDisposedException("Socket has been closed");
        }
        if (_fd < 0)
        {
            throw new SocketException(Constants.ERR_SOCKET);
        }
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }
        if (size < 0 || size > buffer.Length - offset)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }
        if (offset < 0 || offset > buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }
        const int flags = 0;
        var bufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
        var er = zts_bsd_recv(_fd, bufferPtr + offset, (uint)Buffer.ByteLength(buffer), flags);
        return er;
    }

    public int ReceiveFrom(byte[] buffer)
    {
        return ReceiveFrom(buffer, buffer?.Length ?? 0);
    }

    public int ReceiveFrom(byte[] buffer, int size)
    {
        if (_isClosed)
        {
            throw new ObjectDisposedException("Socket has been closed");
        }
        if (_fd < 0)
        {
            throw new SocketException(Constants.ERR_SOCKET);
        }
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }
        if (size < 0 || size > buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }
        const int broadcast = 0;
        var lpBuffer = Marshal.AllocHGlobal(broadcast);
        var bufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
        var er = zts_bsd_recvfrom(_fd, bufferPtr, (uint)Buffer.ByteLength(buffer)-1, 0, IntPtr.Zero, lpBuffer);
        Marshal.FreeHGlobal(lpBuffer);
        return er;
    }

    public int ReceiveTimeout
    {
        get => zts_get_recv_timeout(_fd);
        // TODO: microseconds
        set => zts_set_recv_timeout(_fd, value, 0);
    }

    public int SendTimeout
    {
        get => zts_get_send_timeout(_fd);
        // TODO: microseconds
        set => zts_set_send_timeout(_fd, value, 0);
    }

    public int ConnectTimeout { get; set; } = 30000;

    public int ReceiveBufferSize
    {
        get => zts_get_recv_buf_size(_fd);
        set => zts_set_recv_buf_size(_fd, value);
    }

    public int SendBufferSize
    {
        get => zts_get_send_buf_size(_fd);
        set => zts_set_send_buf_size(_fd, value);
    }

    public int SetBroadcast()
    {
        const int broadcast = 1;
        var lpBuffer = Marshal.AllocHGlobal(broadcast);

        return zts_bsd_setsockopt(_fd, Constants.SOL_SOCKET, Constants.SO_BROADCAST, lpBuffer, sizeof(int));
    }

    public short Ttl
    {
        get => Convert.ToInt16(zts_get_ttl(_fd));
        set => zts_set_ttl(_fd, value);
    }

    public LingerOption LingerState
    {
        get
        {
            var lo =
                new LingerOption(Convert.ToBoolean(zts_get_linger_enabled(_fd)), zts_get_linger_value(_fd));
            return lo;
        }
        set => zts_set_linger(_fd, Convert.ToInt32(value.Enabled), value.LingerTime);
    }

    public bool NoDelay
    {
        get => Convert.ToBoolean(zts_get_no_delay(_fd));
        set => zts_set_no_delay(_fd, Convert.ToInt32(value));
    }

    public bool KeepAlive
    {
        get => Convert.ToBoolean(zts_get_keepalive(_fd));
        set => zts_set_keepalive(_fd, Convert.ToInt32(value));
    }

    public bool Connected { get; private set; }

    public bool IsBound { get; private set; }

    public AddressFamily AddressFamily { get; }

    public SocketType SocketType { get; }

    public ProtocolType ProtocolType { get; }

    /* .NET has moved to OSSupportsIPv* but libzt isn't an OS so we keep this old convention */
    public static bool SupportsIPv4 => true;

    /* .NET has moved to OSSupportsIPv* but libzt isn't an OS so we keep this old convention */
    public static bool SupportsIPv6 => true;

    public EndPoint RemoteEndPoint => _remoteEndPoint;

    public EndPoint LocalEndPoint => _localEndPoint;

    /* Structures and functions used internally to communicate with
    lower-level C API defined in include/ZeroTierSockets.h */

    [DllImport(
        "libzt",
        CharSet = CharSet.Ansi,
        EntryPoint = "CSharp_zts_bsd_gethostbyname")]
    public static extern IntPtr
        zts_bsd_gethostbyname(string jarg1);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_select")]
    private static extern int zts_bsd_select(int jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, IntPtr jarg5);

    [DllImport("libzt", EntryPoint = "CSharp_zts_get_all_stats")]
    private static extern int zts_get_all_stats(IntPtr arg1);

    [DllImport("libzt", EntryPoint = "CSharp_zts_get_protocol_stats")]
    private static extern int zts_get_protocol_stats(int arg1, IntPtr arg2);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_socket")]
    private static extern int zts_bsd_socket(int arg1, int arg2, int arg3);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_connect")]
    private static extern int zts_bsd_connect(int arg1, IntPtr arg2, ushort arg3);

    [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_bsd_connect_easy")]
    private static extern int zts_bsd_connect_easy(int arg1, int arg2, string arg3, ushort arg4, int arg5);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_bind")]
    private static extern int zts_bsd_bind(int arg1, IntPtr arg2, ushort arg3);

    [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_bsd_bind_easy")]
    private static extern int zts_bsd_bind_easy(int arg1, int arg2, string arg3, ushort arg4);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_listen")]
    private static extern int zts_bsd_listen(int arg1, int arg2);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_accept")]
    private static extern int zts_bsd_accept(int arg1, IntPtr arg2, IntPtr arg3);

    [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_bsd_accept_easy")]
    private static extern int zts_bsd_accept_easy(int arg1, IntPtr remoteAddrStr, int arg2, ref int arg3);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_setsockopt")]
    private static extern int zts_bsd_setsockopt(int arg1, int arg2, int arg3, IntPtr arg4, ushort arg5);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_getsockopt")]
    private static extern int zts_bsd_getsockopt(int arg1, int arg2, int arg3, IntPtr arg4, IntPtr arg5);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_getsockname")]
    private static extern int zts_bsd_getsockname(int arg1, IntPtr arg2, IntPtr arg3);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_getpeername")]
    private static extern int zts_bsd_getpeername(int arg1, IntPtr arg2, IntPtr arg3);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_close")]
    private static extern int zts_bsd_close(int arg1);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_fcntl")]
    private static extern int zts_bsd_fcntl(int arg1, int arg2, int arg3);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_poll")]
    private static extern int zts_bsd_poll(IntPtr arg1, uint arg2, int arg3);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_ioctl")]
    private static extern int zts_bsd_ioctl(int arg1, uint arg2, IntPtr arg3);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_send")]
    private static extern int zts_bsd_send(int arg1, IntPtr arg2, uint arg3, int arg4);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_sendto")]
    private static extern int zts_bsd_sendto(int arg1, IntPtr arg2, uint arg3, int arg4, IntPtr arg5, ushort arg6);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_sendmsg")]
    private static extern int zts_bsd_sendmsg(int arg1, IntPtr arg2, int arg3);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_recv")]
    private static extern int zts_bsd_recv(int arg1, IntPtr arg2, uint arg3, int arg4);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_recvfrom")]
    private static extern int zts_bsd_recvfrom(int arg1, IntPtr arg2, uint arg3, int arg4, IntPtr arg5, IntPtr arg6);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_recvmsg")]
    private static extern int zts_bsd_recvmsg(int arg1, IntPtr arg2, int arg3);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_read")]
    private static extern int zts_bsd_read(int arg1, IntPtr arg2, uint arg3);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_readv")]
    private static extern int zts_bsd_readv(int arg1, IntPtr arg2, int arg3);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_write")]
    private static extern int zts_bsd_write(int arg1, IntPtr arg2, uint arg3);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_writev")]
    private static extern int zts_bsd_writev(int arg1, IntPtr arg2, int arg3);

    [DllImport("libzt", EntryPoint = "CSharp_zts_bsd_shutdown")]
    private static extern int zts_bsd_shutdown(int arg1, int arg2);

    [DllImport("libzt", EntryPoint = "CSharp_zts_get_data_available")]
    private static extern int zts_get_data_available(int fd);

    [DllImport("libzt", EntryPoint = "CSharp_zts_set_no_delay")]
    private static extern int zts_set_no_delay(int fd, int enabled);

    [DllImport("libzt", EntryPoint = "CSharp_zts_get_no_delay")]
    private static extern int zts_get_no_delay(int fd);

    [DllImport("libzt", EntryPoint = "CSharp_zts_set_linger")]
    private static extern int zts_set_linger(int fd, int enabled, int value);

    [DllImport("libzt", EntryPoint = "CSharp_zts_get_linger_enabled")]
    private static extern int zts_get_linger_enabled(int fd);

    [DllImport("libzt", EntryPoint = "CSharp_zts_get_linger_value")]
    private static extern int zts_get_linger_value(int fd);

    [DllImport("libzt", EntryPoint = "CSharp_zts_set_reuse_addr")]
    private static extern int zts_set_reuse_addr(int fd, int enabled);

    [DllImport("libzt", EntryPoint = "CSharp_zts_get_reuse_addr")]
    private static extern int zts_get_reuse_addr(int fd);

    [DllImport("libzt", EntryPoint = "CSharp_zts_set_recv_timeout")]
    private static extern int zts_set_recv_timeout(int fd, int seconds, int microseconds);

    [DllImport("libzt", EntryPoint = "CSharp_zts_get_recv_timeout")]
    private static extern int zts_get_recv_timeout(int fd);

    [DllImport("libzt", EntryPoint = "CSharp_zts_set_send_timeout")]
    private static extern int zts_set_send_timeout(int fd, int seconds, int microseconds);

    [DllImport("libzt", EntryPoint = "CSharp_zts_get_send_timeout")]
    private static extern int zts_get_send_timeout(int fd);

    [DllImport("libzt", EntryPoint = "CSharp_zts_set_send_buf_size")]
    private static extern int zts_set_send_buf_size(int fd, int size);

    [DllImport("libzt", EntryPoint = "CSharp_zts_get_send_buf_size")]
    private static extern int zts_get_send_buf_size(int fd);

    [DllImport("libzt", EntryPoint = "CSharp_zts_set_recv_buf_size")]
    private static extern int zts_set_recv_buf_size(int fd, int size);

    [DllImport("libzt", EntryPoint = "CSharp_zts_get_recv_buf_size")]
    private static extern int zts_get_recv_buf_size(int fd);

    [DllImport("libzt", EntryPoint = "CSharp_zts_set_ttl")]
    private static extern int zts_set_ttl(int fd, int ttl);

    [DllImport("libzt", EntryPoint = "CSharp_zts_get_ttl")]
    private static extern int zts_get_ttl(int fd);

    [DllImport("libzt", EntryPoint = "CSharp_zts_set_blocking")]
    private static extern int zts_set_blocking(int fd, int enabled);

    [DllImport("libzt", EntryPoint = "CSharp_zts_get_blocking")]
    private static extern int zts_get_blocking(int fd);

    [DllImport("libzt", EntryPoint = "CSharp_zts_set_keepalive")]
    private static extern int zts_set_keepalive(int fd, int enabled);

    [DllImport("libzt", EntryPoint = "CSharp_zts_get_keepalive")]
    private static extern int zts_get_keepalive(int fd);

    [DllImport("libzt", EntryPoint = "CSharp_zts_add_dns_nameserver")]
    private static extern int zts_add_dns_nameserver(IntPtr arg1);

    [DllImport("libzt", EntryPoint = "CSharp_zts_del_dns_nameserver")]
    private static extern int zts_del_dns_nameserver(IntPtr arg1);

    [DllImport("libzt", EntryPoint = "CSharp_zts_errno_get")]
    private static extern int zts_errno_get();

    [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_accept")]
    private static extern int zts_accept(int jarg1, IntPtr jarg2, int jarg3, ref int jarg4);

    [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_tcp_client")]
    private static extern int zts_tcp_client(string jarg1, int jarg2);

    [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_tcp_server")]
    private static extern int zts_tcp_server(string jarg1, int jarg2, string jarg3, int jarg4, IntPtr jarg5);

    [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_udp_server")]
    private static extern int zts_udp_server(string jarg1, int jarg2);

    [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_udp_client")]
    private static extern int zts_udp_client(string jarg1);

    [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_bind")]
    private static extern int zts_bind(int jarg1, string jarg2, int jarg3);

    [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_connect")]
    private static extern int zts_connect(int jarg1, string jarg2, int jarg3, int jarg4);

    [DllImport("libzt", EntryPoint = "CSharp_zts_stats_get_all")]
    private static extern int zts_stats_get_all(IntPtr jarg1);
    /*
            [DllImport("libzt", EntryPoint = "CSharp_zts_set_no_delay")]
            static extern int zts_set_no_delay(int jarg1, int jarg2);

            [DllImport("libzt", EntryPoint = "CSharp_zts_get_no_delay")]
            static extern int zts_get_no_delay(int jarg1);

            [DllImport("libzt", EntryPoint = "CSharp_zts_set_linger")]
            static extern int zts_set_linger(int jarg1, int jarg2, int jarg3);

            [DllImport("libzt", EntryPoint = "CSharp_zts_get_linger_enabled")]
            static extern int zts_get_linger_enabled(int jarg1);

            [DllImport("libzt", EntryPoint = "CSharp_zts_get_linger_value")]
            static extern int zts_get_linger_value(int jarg1);

            [DllImport("libzt", EntryPoint = "CSharp_zts_set_reuse_addr")]
            static extern int zts_set_reuse_addr(int jarg1, int jarg2);

            [DllImport("libzt", EntryPoint = "CSharp_zts_get_reuse_addr")]
            static extern int zts_get_reuse_addr(int jarg1);

            [DllImport("libzt", EntryPoint = "CSharp_zts_set_recv_timeout")]
            static extern int zts_set_recv_timeout(int jarg1, int jarg2, int jarg3);

            [DllImport("libzt", EntryPoint = "CSharp_zts_get_recv_timeout")]
            static extern int zts_get_recv_timeout(int jarg1);

            [DllImport("libzt", EntryPoint = "CSharp_zts_set_send_timeout")]
            static extern int zts_set_send_timeout(int jarg1, int jarg2, int jarg3);

            [DllImport("libzt", EntryPoint = "CSharp_zts_get_send_timeout")]
            static extern int zts_get_send_timeout(int jarg1);

            [DllImport("libzt", EntryPoint = "CSharp_zts_set_send_buf_size")]
            static extern int zts_set_send_buf_size(int jarg1, int jarg2);

            [DllImport("libzt", EntryPoint = "CSharp_zts_get_send_buf_size")]
            static extern int zts_get_send_buf_size(int jarg1);

            [DllImport("libzt", EntryPoint = "CSharp_zts_set_recv_buf_size")]
            static extern int zts_set_recv_buf_size(int jarg1, int jarg2);

            [DllImport("libzt", EntryPoint = "CSharp_zts_get_recv_buf_size")]
            static extern int zts_get_recv_buf_size(int jarg1);

            [DllImport("libzt", EntryPoint = "CSharp_zts_set_ttl")]
            static extern int zts_set_ttl(int jarg1, int jarg2);

            [DllImport("libzt", EntryPoint = "CSharp_zts_get_ttl")]
            static extern int zts_get_ttl(int jarg1);

            [DllImport("libzt", EntryPoint = "CSharp_zts_set_blocking")]
            static extern int zts_set_blocking(int jarg1, int jarg2);

            [DllImport("libzt", EntryPoint = "CSharp_zts_get_blocking")]
            static extern int zts_get_blocking(int jarg1);

            [DllImport("libzt", EntryPoint = "CSharp_zts_set_keepalive")]
            static extern int zts_set_keepalive(int jarg1, int jarg2);

            [DllImport("libzt", EntryPoint = "CSharp_zts_get_keepalive")]
            static extern int zts_get_keepalive(int jarg1);



    */

    [DllImport("libzt", EntryPoint = "CSharp_zts_util_delay")]
    public static extern void zts_util_delay(int jarg1);

    [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_util_get_ip_family")]
    private static extern int zts_util_get_ip_family(string jarg1);

    [DllImport("libzt", CharSet = CharSet.Ansi, EntryPoint = "CSharp_zts_util_ipstr_to_saddr")]
    private static extern int zts_util_ipstr_to_saddr(string jarg1, int jarg2, IntPtr jarg3, IntPtr jarg4);

    /// <value>The value of errno for the low-level socket layer</value>
    public static int ErrNo => zts_errno_get();

    [StructLayout(LayoutKind.Sequential)]
    private struct zts_sockaddr
    {
        public byte sa_len;
        public byte sa_family;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
        public byte[] sa_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct zts_in_addr
    {
        public uint s_addr;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct zts_sockaddr_in
    {
        public byte sin_len;
        public byte sin_family;
        public short sin_port;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] sin_addr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] sin_zero; // SIN_ZERO_LEN
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct zts_pollfd
    {
        public int fd;
        public short events;
        public short revents;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct zts_timeval
    {
        public long tv_sec;
        public long tv_usec;
    }
}