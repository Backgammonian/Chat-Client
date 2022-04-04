using System;

namespace Shared
{
    public class NetworkDataReceivedEventArgs : EventArgs
    {
        public NetworkDataReceivedEventArgs(byte[] data)
        {
            Data = data;
        }

        public byte[] Data { get; }
    }
}
