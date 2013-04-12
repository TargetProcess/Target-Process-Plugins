using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;

namespace Mercurial
{
    /// <summary>
    /// This class handles executing external executables in order to process
    /// commands.
    /// </summary>
    public static class CommandProcessor
    {
        /// <summary>
        /// This field is used to specify the encoding of input/output streams for processes.
        /// </summary>
        private static readonly Encoding _Encoding = Encoding.GetEncoding("Windows-1252");

        /// <summary>
        /// Executes the given executable to process the given command asynchronously.
        /// </summary>
        /// <param name="workingDirectory">
        /// The working directory while executing the command.
        /// </param>
        /// <param name="executable">
        /// The full path to and name of the executable to execute.
        /// </param>
        /// <param name="command">
        /// The options to the executable.
        /// </param>
        /// <param name="environmentVariables">
        /// An array of <see cref="KeyValuePair{TKey,TValue}"/> objects, containing environment variable
        /// overrides to use while executing the executable.
        /// </param>
        /// <param name="specialArguments">
        /// Any special arguments to pass to the executable, not defined by the <paramref name="command"/>
        /// object, typically common arguments that should always be passed to the executable.
        /// </param>
        /// <param name="callback">
        /// A callback to call when the execution has completed. The <see cref="IAsyncResult.AsyncState"/> value of the
        /// <see cref="IAsyncResult"/> object passed to the <paramref name="callback"/> will be the
        /// <paramref name="command"/> object.
        /// </param>
        /// <returns>
        /// Returns a <see cref="IAsyncResult"/> object that should be passed to <see cref="EndExecute"/> when
        /// execution has completed.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="workingDirectory"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="executable"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="environmentVariables"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="specialArguments"/> is <c>null</c>.</para>
        /// </exception>
        public static IAsyncResult BeginExecute(
            string workingDirectory, string executable, ICommand command, KeyValuePair<string, string>[] environmentVariables,
            IEnumerable<string> specialArguments, AsyncCallback callback)
        {
            return new ExecuteDelegate(Execute).BeginInvoke(
                workingDirectory, executable, command, environmentVariables, specialArguments, callback, command);
        }

        /// <summary>
        /// Finalizes asynchronous execution of the command.
        /// </summary>
        /// <param name="result">
        /// The <see cref="IAsyncResult"/> object returned from <see cref="BeginExecute"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="result"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="result"/> is not a <see cref="IAsyncResult"/> that was returned from <see cref="BeginExecute"/>.</para>
        /// </exception>
        public static void EndExecute(IAsyncResult result)
        {
            if (result == null)
                throw new ArgumentNullException("result");
            var asyncResult = result as AsyncResult;
            if (asyncResult == null)
                throw new ArgumentException("invalid IAsyncResult object passed to CommandProcessor.EndExecute", "result");
            var executeDelegate = asyncResult.AsyncDelegate as ExecuteDelegate;
            if (executeDelegate == null)
                throw new ArgumentException("invalid IAsyncResult object passed to CommandProcessor.EndExecute", "result");
            executeDelegate.EndInvoke(result);
        }

        /// <summary>
        /// Executes the given executable to process the given command.
        /// </summary>
        /// <param name="workingDirectory">
        /// The working directory while executing the command.
        /// </param>
        /// <param name="executable">
        /// The full path to and name of the executable to execute.
        /// </param>
        /// <param name="command">
        /// The options to the executable.
        /// </param>
        /// <param name="environmentVariables">
        /// An array of <see cref="KeyValuePair{TKey,TValue}"/> objects, containing environment variable
        /// overrides to use while executing the executable.
        /// </param>
        /// <param name="specialArguments">
        /// Any special arguments to pass to the executable, not defined by the <paramref name="command"/>
        /// object, typically common arguments that should always be passed to the executable.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="workingDirectory"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="executable"/> is <c>null</c> or empty.</para>
        /// <para>- or -</para>
        /// <para><paramref name="command"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="environmentVariables"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="specialArguments"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="MercurialException">
        /// <para>The executable did not finish in the allotted time.</para>
        /// </exception>
        public static void Execute(
            string workingDirectory, string executable, ICommand command, KeyValuePair<string, string>[] environmentVariables,
            IEnumerable<string> specialArguments)
        {
            if (StringEx.IsNullOrWhiteSpace(workingDirectory))
                throw new ArgumentNullException("workingDirectory");
            if (StringEx.IsNullOrWhiteSpace(executable))
                throw new ArgumentNullException("executable");
            if (command == null)
                throw new ArgumentNullException("command");
            if (environmentVariables == null)
                throw new ArgumentNullException("environmentVariables");
            if (specialArguments == null)
                throw new ArgumentNullException("specialArguments");

            command.Validate();
            command.Before();

            IEnumerable<string> arguments = specialArguments;
            arguments = arguments.Concat(command.Arguments.Where(a => !StringEx.IsNullOrWhiteSpace(a)));
            arguments = arguments.Concat(command.AdditionalArguments.Where(a => !StringEx.IsNullOrWhiteSpace(a)));

            string argumentsString = string.Join(" ", arguments.ToArray());

            var psi = new ProcessStartInfo
            {
                FileName = executable,
                WorkingDirectory = workingDirectory,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                ErrorDialog = false,
                Arguments = command.Command + " " + argumentsString,
            };
            foreach (var kvp in environmentVariables)
                psi.EnvironmentVariables[kvp.Key] = kvp.Value;
            psi.StandardErrorEncoding = _Encoding;
            psi.StandardOutputEncoding = _Encoding;

            if (command.Observer != null)
                command.Observer.Executing(command.Command, argumentsString);

            Process process = Process.Start(psi);
            try
            {
                Func<StreamReader, Action<string>, string> reader;
                if (command.Observer != null)
                {
                    reader = delegate(StreamReader streamReader, Action<string> logToObserver)
                    {
                        var output = new StringBuilder();
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            logToObserver(line);
                            if (output.Length > 0)
                                output.Append(Environment.NewLine);
                            output.Append(line);
                        }
                        return output.ToString();
                    };
                }
                else
                    reader = (StreamReader streamReader, Action<string> logToObserver) => streamReader.ReadToEnd();

                IAsyncResult outputReader = reader.BeginInvoke(process.StandardOutput, line => command.Observer.Output(line), null, null);
                IAsyncResult errorReader = reader.BeginInvoke(process.StandardError, line => command.Observer.ErrorOutput(line), null, null);

                int timeout = Timeout.Infinite;
                if (command.Timeout > 0)
                    timeout = 1000 * command.Timeout;

                if (!process.WaitForExit(timeout))
                {
                    try
                    {
                        process.Kill();
                        process = null;
                    }
                    catch (InvalidOperationException){}
                    
                    if (command.Observer != null)
                        command.Observer.Executed(psi.FileName, psi.Arguments, 0, string.Empty, string.Empty);
                    throw new MercurialTimeoutException("The executable did not complete within the alloted time");
                }

                string standardOutput = reader.EndInvoke(outputReader);
                string errorOutput = reader.EndInvoke(errorReader);

                if (command.Observer != null)
                    command.Observer.Executed(command.Command, argumentsString, process.ExitCode, standardOutput, errorOutput);

                command.After(process.ExitCode, standardOutput, errorOutput);
            }
            finally
            {
                if (process != null)
                    process.Dispose();
            }
        }

        #region Nested type: ExecuteDelegate

        /// <summary>
        /// A delegate that matches <see cref="Execute"/>.
        /// </summary>
        /// <param name="workingDirectory">
        /// The working directory while executing the command.
        /// </param>
        /// <param name="executable">
        /// The full path to and name of the executable to execute.
        /// </param>
        /// <param name="command">
        /// The options to the executable.
        /// </param>
        /// <param name="environmentVariables">
        /// An array of <see cref="KeyValuePair{TKey,TValue}"/> objects, containing environment variable
        /// overrides to use while executing the executable.
        /// </param>
        /// <param name="specialArguments">
        /// Any special arguments to pass to the executable, not defined by the <paramref name="command"/>
        /// object, typically common arguments that should always be passed to the executable.
        /// </param>
        private delegate void ExecuteDelegate(
            string workingDirectory, string executable, ICommand command, KeyValuePair<string, string>[] environmentVariables,
            IEnumerable<string> specialArguments);

        #endregion
    }
}