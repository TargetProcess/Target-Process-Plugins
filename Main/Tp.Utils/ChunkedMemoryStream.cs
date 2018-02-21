using System.Collections.Generic;
using System.Linq;

namespace System.IO
{
    public class ChunkedMemoryStream : Stream
    {
        private readonly List<byte[]> _chunks = new List<byte[]>();
        private int _positionChunk;
        private int _positionOffset;
        private long _position;
        private const int MAX_CHUNK_SIZE = 64 * 1024;

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
        }

        public override long Length
        {
            get { return _chunks.Sum(c => c.Length); }
        }

        public override long Position
        {
            get { return _position; }
            set
            {
                _position = value;

                _positionChunk = 0;

                while (_positionOffset != 0)
                {
                    if (_positionChunk >= _chunks.Count)
                        throw new OverflowException();

                    if (_positionOffset < _chunks[_positionChunk].Length)
                        return;

                    _positionOffset -= _chunks[_positionChunk].Length;
                    _positionChunk++;
                }
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int result = 0;
            while ((count != 0) && (_positionChunk != _chunks.Count) && Position < Length)
            {
                int fromChunk = Math.Min(count, _chunks[_positionChunk].Length - _positionOffset);
                if (fromChunk != 0)
                {
                    Array.Copy(_chunks[_positionChunk], _positionOffset, buffer, offset, fromChunk);
                    offset += fromChunk;
                    count -= fromChunk;
                    result += fromChunk;
                    _positionOffset += fromChunk;
                }
                else
                {
                    _positionOffset = 0;
                    _positionChunk++;
                }
            }
            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPos = 0;

            switch (origin)
            {
                case SeekOrigin.Begin:
                    newPos = offset;
                    break;
                case SeekOrigin.Current:
                    newPos = Position + offset;
                    break;
                case SeekOrigin.End:
                    newPos = Length - offset;
                    break;
            }

            Position = Math.Max(0, Math.Min(newPos, Length));
            return newPos;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count > MAX_CHUNK_SIZE)
            {
                Write(buffer, offset, MAX_CHUNK_SIZE);
                Write(buffer, offset + MAX_CHUNK_SIZE, count - MAX_CHUNK_SIZE);
                return;
            }


            while ((count != 0) && (_positionChunk != _chunks.Count))
            {
                int toChunk = Math.Min(count, _chunks[_positionChunk].Length - _positionOffset);
                if (toChunk != 0)
                {
                    Array.Copy(buffer, offset, _chunks[_positionChunk], _positionOffset, toChunk);
                    offset += toChunk;
                    count -= toChunk;
                    _position += toChunk;
                }

                _positionOffset = 0;
                _positionChunk++;
            }

            if (count != 0)
            {
                var chunk = new byte[count];
                Array.Copy(buffer, offset, chunk, 0, count);
                _chunks.Add(chunk);
                _positionChunk = _chunks.Count;
                _position += count;
            }
        }
    }
}
