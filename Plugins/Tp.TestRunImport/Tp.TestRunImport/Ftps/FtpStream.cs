using System;
using System.IO;

namespace Tp.Integration.Plugin.TestRunImport.Ftps
{
    /// <summary>
    /// Incapsulates a Stream used during FTP get and put commands.
    /// </summary>
    internal class FtpStream : Stream
    {
        [Flags]
        public enum EAllowedOperation { Read = 1, Write = 2 }

        readonly Stream _innerStream;
        readonly FtpStreamCallback _streamClosedCallback;
        readonly EAllowedOperation _allowedOp;

        internal FtpStream(Stream innerStream, EAllowedOperation allowedOp, FtpStreamCallback streamClosedCallback)
        {
            _innerStream = innerStream;
            _streamClosedCallback = streamClosedCallback;
            _allowedOp = allowedOp;
        }

        public override bool CanRead => _innerStream.CanRead && (_allowedOp & EAllowedOperation.Read) == EAllowedOperation.Read;

        public override bool CanSeek => _innerStream.CanSeek;

        public override bool CanWrite => _innerStream.CanWrite && (_allowedOp & EAllowedOperation.Write) == EAllowedOperation.Write;

        public override void Flush()
        {
            _innerStream.Flush();
        }

        public override long Length => _innerStream.Length;

        public override long Position
        {
            get
            {
                return _innerStream.Position;
            }
            set
            {
                _innerStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!CanRead)
                throw new AlexPilotti.FTPS.Common.FTPException("Operation not allowed");

            return _innerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!CanWrite)
                throw new AlexPilotti.FTPS.Common.FTPException("Operation not allowed");

            _innerStream.Write(buffer, offset, count);
        }

        public override void Close()
        {
            base.Close();
            _streamClosedCallback();
        }
    }
}