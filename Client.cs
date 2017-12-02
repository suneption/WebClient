using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class Client : IDisposable
{
    private readonly ClientCfg _cfg;

    private Socket _socket;

    public Client(ClientCfg cfg) 
    {
        _cfg = cfg;
        _socket = Initialize();
    }

    public async Task TestSending() 
    {
        var request = "hello world";
        var requestBytes = Encoding.UTF8.GetBytes(request);
        
        var watcher = new Stopwatch();
        watcher.Start();
        await SendAsync(requestBytes);

        var responseBytes = new byte[1024];
        //var responseSize = await ReceiveAsync(_socket, responseBytes);
        var responseSize = _socket.Receive(responseBytes, 0, responseBytes.Length, SocketFlags.None);
        watcher.Stop();
        Console.WriteLine($"Elapsed time {watcher.ElapsedMilliseconds}");
        var response = Encoding.UTF8.GetString(responseBytes.Take(responseSize).ToArray());
        Console.WriteLine(response);
    }

    public async Task EndlessTestSending()
    {
        while (true)
        {
            await TestSending();
        }
    }

    private Task<int> SendAsync(byte[] request) 
    {
        var result = Task.Factory
            .FromAsync(
                _socket.BeginSend(request, 0, request.Length, SocketFlags.None, null, _socket),
                _socket.EndSend);
        return result;
    }

    private Task<int> ReceiveAsync(Socket socket, byte[] response) 
    {
        return Task.Run(() => { return socket.Receive(response); });
        //var result = Task.Factory
        //    .FromAsync(
        //        socket.BeginReceive(response, 0, response.Length, SocketFlags.None, null, socket),
        //        socket.EndReceive);
        //return result;
    }

    private Socket Initialize() 
    {
        Socket socket = null;
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var endPoint = new IPEndPoint(_cfg.IpAddress, _cfg.Port);
            socket.Connect(endPoint);
        }
        catch 
        {
            DisposeSocket(socket);
            throw;
        }
        return socket;
    }

    private void DisposeSocket(Socket socket) 
    {
        if (socket != null) 
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            socket.Close();
        }
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
                DisposeSocket(_socket);
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            disposedValue = true;
        }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~Client() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
    }
    #endregion
}