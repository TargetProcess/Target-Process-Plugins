using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Mercurial
{
    /// <summary>
    /// This class is responsible for decoding the output from the Mercurial
    /// instance running in Command Server mode.
    /// </summary>
    internal static class CommandServerOutputDecoder
    {
        /// <summary>
        /// The encoding used by the standard output from the Mercurial persistent client.
        /// </summary>
        private static readonly Encoding _Encoding = Encoding.GetEncoding("Windows-1252");

        /// <summary>
        /// Retrieve the complete output from executing a command, as
        /// separate standard output, standard error and the exit code.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="StreamReader"/> that output is read from.
        /// </param>
        /// <param name="standardOutput">
        /// Upon exit, if the method returns <c>true</c>, then this parameter has been
        /// changed to contain the standard output from executing the command.
        /// </param>
        /// <param name="standardError">
        /// Upon exit, if the method returns <c>true</c>, then this parameter has been
        /// changed to contain the standard error from executing the command.
        /// </param>
        /// <param name="exitCode">
        /// Upon exit, if the method returns <c>true</c>, then this parameter has been
        /// changed to contain the exit code from executing the command.
        /// </param>
        /// <returns>
        /// <c>true</c> if the output was successfully decoded;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="reader"/> is <c>null</c>.</para>
        /// </exception>
        public static bool GetOutput(StreamReader reader, out string standardOutput, out string standardError, out int exitCode)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            standardOutput = string.Empty;
            standardError = string.Empty;
            exitCode = 0;

            var standardOutputBuilder = new StringBuilder();
            var standardErrorBuilder = new StringBuilder();

            while (true)
            {
                char type;
                string content;
                if (!DecodeOneBlock(reader, out type, out content, out exitCode))
                    return false;
                switch (type)
                {
                    case 'o':
                        standardOutputBuilder.Append(content);
                        break;

                    case 'e':
                        standardErrorBuilder.Append(content);
                        break;

                    case 'r':
                        standardOutput = standardOutputBuilder.ToString();
                        standardError = standardErrorBuilder.ToString();
                        return true;
                }
            }
        }

        /// <summary>
        /// Decodes one block of output from the reader.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="StreamReader"/> to decode a block from.
        /// </param>
        /// <param name="type">
        /// Upon return from the method, if the method returns <c>true</c>, then
        /// this parameter contains the one-character type of the output, which is
        /// either 'o', 'e' or 'r'.
        /// </param>
        /// <param name="content">
        /// Upon return from the method, if the method returns <c>true</c>, then
        /// this parameter contains the decoded string content of the 'o' or 'e' type.
        /// </param>
        /// <param name="exitCode">
        /// Upon return from the method, if the method returns <c>true</c>, then
        /// this parameter contains the exit code of the command, for the 'r' type.
        /// </param>
        /// <returns>
        /// <c>true</c> if the output was successfully decoded;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="reader"/> is <c>null</c>.</para>
        /// </exception>
        public static bool DecodeOneBlock(StreamReader reader, out char type, out string content, out int exitCode)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            type = '\0';
            content = string.Empty;
            exitCode = 0;

            int typeValue = reader.Read();
            if (typeValue == -1)
                return false;

            type = (char)typeValue;
            switch (type)
            {
                case 'o':
                case 'e':
                    content = DecodeString(reader);
                    return true;

                case 'r':
                    var length = DecodeInt32(reader);
                    if (length != 4)
                        return false;
                    exitCode = DecodeInt32(reader);
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Decodes and returns a single string from the <see cref="StreamReader"/>.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="StreamReader"/> to decode the string from.
        /// </param>
        /// <returns>
        /// The decoded string or <c>null</c> if the data could not be decoded.
        /// </returns>
        /// <remarks>
        /// Note that this method will not check that <paramref name="reader"/> is non-null.
        /// </remarks>
        private static string DecodeString(StreamReader reader)
        {
            var length = DecodeInt32(reader);
            if (length < 0)
                return null;

            // max: 1MB, to avoid memory problems with corrupt data
            if (length > 1 * 1024 * 1024)
                return null;

            var buffer = new byte[length];
            for (int index = 0; index < length; index++)
            {
                int unencodedByte = reader.Read();
                if (unencodedByte == -1)
                    return null;
                buffer[index] = (byte)unencodedByte;
            }

            return _Encoding.GetString(buffer);
        }

        /// <summary>
        /// Decodes and returns a single 32-bit integer from the <see cref="StreamReader"/>.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="StreamReader"/> to decode the 32-bit integer from.
        /// </param>
        /// <returns>
        /// The decoded integer, or -1 if the data could not be decoded.
        /// </returns>
        /// <remarks>
        /// Note that this method will not check that <paramref name="reader"/> is non-null.
        /// </remarks>
        private static int DecodeInt32(StreamReader reader)
        {
            int b1 = reader.Read();
            int b2 = reader.Read();
            int b3 = reader.Read();
            int b4 = reader.Read();

            if (b1 == -1 || b2 == -1 || b3 == -1 || b4 == -1)
                return -1;

            return b4 | (b3 << 8) | (b2 << 16) | (b1 << 24);
        }
    }
}
