using System;
using System.Text;

namespace Shared
{
    public class SimpleReader
    {
        private readonly byte[] _data;
        private int _position;

        public SimpleReader(byte[] data)
        {
            _data = data;
            _position = 0;
        }

        public byte GetByte()
        {
            var result = _data[_position];
            _position += 1;
            return result;
        }

        public int GetInt()
        {
            var result = BitConverter.ToInt32(_data, _position);
            _position += 4;
            return result;
        }

        public uint GetUInt()
        {
            var result = BitConverter.ToUInt32(_data, _position);
            _position += 4;
            return result;
        }

        public long GetLong()
        {
            var result = BitConverter.ToInt64(_data, _position);
            _position += 8;
            return result;
        }

        public string GetString()
        {
            var length = GetInt();
            if (length <= 0)
            {
                return string.Empty;
            }

            var result = Encoding.UTF8.GetString(_data, _position, length);
            _position += length;
            return result;
        }

        public void GetBytes(byte[] destination, int count)
        {
            Buffer.BlockCopy(_data, _position, destination, 0, count);
            _position += count;
        }
    }
}
