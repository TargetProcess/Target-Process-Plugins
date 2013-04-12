using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Mercurial
{
    /// <summary>
    /// This class implements <see cref="IClient"/> by spinning up an instance when
    /// first instantiated, and keeping the instance around and communicating with it
    /// over standard input and output, using the new "command server mode" introduced
    /// in Mercurial 1.9.
    /// </summary>
    public sealed class PersistentClient : IClient, IDisposable
    {
        /// <summary>
        /// This is the backing field for the <see cref="RepositoryPath"/> property.
        /// </summary>
        private readonly string _RepositoryPath;

        /// <summary>
        /// This field holds a link to the persistent client running alongside Mercurial.Net.
        /// </summary>
        private Process _Process;

        private object _syncProcessObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentClient"/> class.
        /// </summary>
        /// <param name="repositoryPath">
        /// The path to the repository this <see cref="PersistentClient"/> will handle.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repositoryPath"/> is <c>null</c> or empty.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The <see cref="PersistentClient"/> is not supported for the specified repository path,
        /// or the current Mercurial client.
        /// </exception>
        public PersistentClient(string repositoryPath)
        {
            if (StringEx.IsNullOrWhiteSpace(repositoryPath))
                throw new ArgumentNullException("repositoryPath");

            if (!IsSupported(repositoryPath))
                throw new NotSupportedException("The persistent client is not supported for the given repository or by the current Mercurial client");

            _RepositoryPath = repositoryPath;
            StartPersistentMercurialClient();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="PersistentClient"/> class.
        /// </summary>
        ~PersistentClient()
        {
            Dispose();
        }

        /// <summary>
        /// Gets the path to the repository this <see cref="PersistentClient"/> is handling.
        /// </summary>
        public string RepositoryPath
        {
            get
            {
                return _RepositoryPath;
            }
        }

        /// <summary>
        /// Stops command executing.
        /// </summary>
        public void CancelExecuting()
        {
        }

        /// <summary>
        /// Executes the given <see cref="IMercurialCommand"/> command against
        /// the Mercurial repository.
        /// </summary>
        /// <param name="command">
        /// The <see cref="IMercurialCommand"/> command to execute.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="MercurialException">
        /// HG did not complete within the allotted time.
        /// </exception>
        public void Execute(IMercurialCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (_Process == null)
                StartPersistentMercurialClient();

            command.Validate();
            command.Before();

            IEnumerable<string> arguments = new[]
            {
                command.Command,
                 "--noninteractive",
                 "--encoding",
                 "cp1252",
            };
            arguments = arguments.Concat(command.Arguments.Where(a => !StringEx.IsNullOrWhiteSpace(a)));
            arguments = arguments.Concat(command.AdditionalArguments.Where(a => !StringEx.IsNullOrWhiteSpace(a)));

            var commandParts = arguments.ToArray();

            string commandEncoded = string.Join("\0", commandParts.Select(p => p.Trim('"')).ToArray());
            int length = commandEncoded.Length;
            var commandBuffer = new StringBuilder();
            commandBuffer.Append("runcommand\n");
            commandBuffer.Append((char)((length >> 24) & 0xff));
            commandBuffer.Append((char)((length >> 16) & 0xff));
            commandBuffer.Append((char)((length >> 8) & 0xff));
            commandBuffer.Append((char)(length & 0xff));
            commandBuffer.Append(commandEncoded);

            string commandArguments = null;
            if (command.Observer != null)
            {
                commandArguments = string.Join(" ", commandParts.Skip(1).ToArray());
                command.Observer.Executing(command.Command, commandArguments);
            }
            
            byte[] buffer = Encoding.GetEncoding("iso-8859-1").GetBytes(commandBuffer.ToString());
            foreach (byte b in buffer)
            {
                _Process.StandardInput.BaseStream.WriteByte(b);
                _Process.StandardInput.BaseStream.Flush();
            }

            string standardOutput;
            string standardError;
            int exitCode;

            if (CommandServerOutputDecoder.GetOutput(_Process.StandardOutput, out standardOutput, out standardError, out exitCode))
            {
                if (command.Observer != null)
                {
                    using (var lineReader = new StringReader(standardOutput))
                    {
                        string line;
                        while ((line = lineReader.ReadLine()) != null)
                            command.Observer.Output(line);
                    }
                    using (var lineReader = new StringReader(standardError))
                    {
                        string line;
                        while ((line = lineReader.ReadLine()) != null)
                            command.Observer.ErrorOutput(line);
                    }
                    command.Observer.Executed(command.Command, commandArguments, exitCode, standardOutput, standardError);
                }
                command.After(exitCode, standardOutput, standardError);
                return;
            }

            StopPersistentMercurialClient();
            throw new MercurialExecutionException("Unable to decode output from executing command, spinning down persistent client");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            StopPersistentMercurialClient();
        }

        /// <summary>
        /// Determine if this class is supported by the given repository and current
        /// Mercurial client version.
        /// </summary>
        /// <param name="repositoryPath">
        /// The path to the repository to check supportability for.
        /// </param>
        /// <returns>
        /// <c>true</c> if <see cref="PersistentClient"/> is supported for the
        /// given repository and for the current Mercurial client;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="repositoryPath"/> is <c>null</c> or empty.</para>
        /// </exception>
        public static bool IsSupported(string repositoryPath)
        {
            if (StringEx.IsNullOrWhiteSpace(repositoryPath))
                throw new ArgumentNullException("repositoryPath");

            if (!Directory.Exists(repositoryPath))
                return false;

            // TODO: Determine if we need to check if the .hg folder is an actual repository
            if (!Directory.Exists(Path.Combine(repositoryPath, ".hg")))
                return false;

            if (ClientExecutable.CurrentVersion < new Version(1, 9, 0, 0))
                return false;

            return true;
        }

        /// <summary>
        /// This spins up a Mercurial client in command server mode for the
        /// repository.
        /// </summary>
        private void StartPersistentMercurialClient()
        {
            lock (_syncProcessObject)
            {
                if (_Process == null)
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = ClientExecutable.ClientPath,
                        WorkingDirectory = _RepositoryPath,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        ErrorDialog = false,
                        Arguments = "serve --cmdserver pipe --noninteractive --encoding cp1252",
                    };
                    psi.EnvironmentVariables.Add("LANGUAGE", "EN");
                    psi.EnvironmentVariables.Add("HGENCODING", "cp1252");

                    psi.StandardOutputEncoding = Encoding.GetEncoding("iso-8859-1");
                    psi.StandardErrorEncoding = Encoding.GetEncoding("iso-8859-1");

                    _Process = Process.Start(psi);
                    DecodeInitialBlock();        
                }
            }
        }

        /// <summary>
        /// Decodes the initial block that Mercurial outputs when it is spun up.
        /// </summary>
        private void DecodeInitialBlock()
        {
            char type;
            string content;
            int exitCode;
            CommandServerOutputDecoder.DecodeOneBlock(_Process.StandardOutput, out type, out content, out exitCode);
        }

        /// <summary>
        /// This spins down the Mercurial client that was spun up by
        /// <see cref="StartPersistentMercurialClient"/>.
        /// </summary>
        private void StopPersistentMercurialClient()
        {
            lock (_syncProcessObject)
            {
                if (_Process != null && !_Process.HasExited)
                {
                    //_Process.Dispose();
                    //_Process.Close();
                    ////_Process.WaitForExit();
                    
                    //if (_Process.StandardInput.BaseStream.CanWrite)
                        _Process.StandardInput.Write("dummycommandforceservertoquit\n");

                    //if (_Process.StandardInput..BaseStream != null)
                        _Process.StandardInput.Close();

                    //if (!_Process.HasExited)
                        _Process.WaitForExit();

                    _Process = null;
                }    
            }
        }
    }
}
