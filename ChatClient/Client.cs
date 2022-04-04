using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Shared;

namespace ChatClient
{
    //create one Client object for each connection to server
    public class Client
    {
        private readonly Socket _client;
        private readonly Task _listenTask;
        private readonly CancellationTokenSource _tokenSource;

        public Client()
        {
            var size = Marshal.SizeOf((uint)0);
            var keepAlive = new byte[size * 3];
            Buffer.BlockCopy(BitConverter.GetBytes(TcpKeepAliveConstants.TurnKeepAliveOn), 0, keepAlive, 0, size);
            Buffer.BlockCopy(BitConverter.GetBytes(TcpKeepAliveConstants.TimeWithoutActivity), 0, keepAlive, size, size);
            Buffer.BlockCopy(BitConverter.GetBytes(TcpKeepAliveConstants.KeepAliveInterval), 0, keepAlive, size * 2, size);

            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _client.IOControl(IOControlCode.KeepAliveValues, keepAlive, null);

            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;
            _listenTask = new Task(async () => await Listen(token));
        }

        public event EventHandler<NetworkDataReceivedEventArgs> DataReceived;

        public IPEndPoint ServerAddress { get; private set; }

        public async Task<bool> Connect(IPEndPoint serverEndPoint)
        {
            try
            {
                await _client.ConnectAsync(serverEndPoint);
                ServerAddress = serverEndPoint;

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);

                return false;
            }
        }

        public async Task Send(byte[] message)
        {
            try
            {
                var messageLengthArray = BitConverter.GetBytes(message.Length);

                await _client.SendAsync(messageLengthArray, SocketFlags.None);
                await _client.SendAsync(message, SocketFlags.None);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private async Task<byte[]> Receive()
        {
            try
            {
                var messageLengthArray = new byte[4];
                await _client.ReceiveAsync(messageLengthArray, SocketFlags.None);

                var message = new byte[BitConverter.ToInt32(messageLengthArray)];
                await _client.ReceiveAsync(message, SocketFlags.None);

                return message;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);

                return Array.Empty<byte>();
            }
        }

        private async Task Listen(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var data = await Receive();

                DataReceived?.Invoke(this, new NetworkDataReceivedEventArgs(data));
            }
        }

        public void StartListen()
        {
            _listenTask.Start();
        }

        public void Stop()
        {
            _tokenSource.Cancel();

            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
        }
    }
}
