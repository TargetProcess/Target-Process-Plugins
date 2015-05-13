using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using log4net;

namespace Tp.Utils.Sys
{
	/// <summary>
	/// Executes command in a console window, reads its output.
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	// TODO remove or move
	public class Executor
	{
		private const int ERROR_FILE_NOT_FOUND = 2;
		private const int ERROR_ACCESS_DENIED = 5;
		private const int TIMEOUT = 1*60*1000;

		private readonly ILog _log = LogManager.GetLogger(typeof (Executor));

		private int _timeOut = TIMEOUT;

		/// <summary>
		/// If the command does not respond in the specified amoung of time in milliseconds it will be killed.
		/// </summary>
		public int TimeOut
		{
			get { return _timeOut; }
			set { _timeOut = value; }
		}

		/// <summary>
		/// Executes the specified command with the specified arguments, reads command's output and error output using UTF8 Encoding
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="arguments">Command arguments.</param>
		/// <param name="output">Command output.</param>
		/// <param name="errorOutput">Command error output.</param>
		/// <returns>Return value from command.</returns>
		/// <exception cref="ArgumentException">If <c>command</c> is <c>null</c> or empty string.</exception>
		public int Execute(string command, string arguments,
		                   out string output, out string errorOutput)
		{
			return Execute(command, arguments, Encoding.UTF8, null, out output, out errorOutput);
		}

		/// <summary>
		/// Executes the specified command with the specified arguments, writes command's input, reads command's output and error output using UTF8 Encoding
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="arguments">Command arguments.</param>
		/// <param name="input">Input to the command.</param>
		/// <param name="output">Command output.</param>
		/// <param name="errorOutput">Command error output.</param>
		/// <returns>Return value from command.</returns>
		/// <exception cref="ArgumentException">If <c>command</c> is <c>null</c> or empty string.</exception>
		public int Execute(string command, string arguments,
		                   string input, out string output, out string errorOutput)
		{
			return Execute(command, arguments, Encoding.UTF8, input, out output, out errorOutput);
		}

		/// <summary>
		/// Executes the specified command with the specified arguments, reads command's output and error output.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="arguments">Command arguments.</param>
		/// <param name="encoding">The encoding to decode standart output and standard error stream, or <c>null</c> if using default encoding.</param>
		/// <param name="output">Command output.</param>
		/// <param name="errorOutput">Command error output.</param>
		/// <returns>Return value from command.</returns>
		/// <exception cref="ArgumentException">If <c>command</c> is <c>null</c> or empty string.</exception>
		public int Execute(string command, string arguments, Encoding encoding,
		                   out string output, out string errorOutput)
		{
			return Execute(command, arguments, encoding, null, out output, out errorOutput);
		}

		/// <summary>
		/// Executes the specified command with the specified arguments, writes command's input, reads command's output and error output.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="arguments">Command arguments.</param>
		/// <param name="encoding">The encoding to decode standart output and standard error stream, or <c>null</c> if using default encoding.</param>
		/// <param name="input">Input to the command.</param>
		/// <param name="output">Command output.</param>
		/// <param name="errorOutput">Command error output.</param>
		/// <returns>Return value from command.</returns>
		/// <exception cref="ArgumentException">If <c>command</c> is <c>null</c> or empty string.</exception>
		public int Execute(string command, string arguments, Encoding encoding,
		                   string input, out string output, out string errorOutput)
		{
			if (string.IsNullOrEmpty(command))
			{
				throw new ArgumentException("Command name cannot be null or empty string", "command");
			}

			if (_log.IsDebugEnabled)
			{
				if (string.IsNullOrEmpty(arguments))
				{
					_log.Debug(string.Format("Executing command '{0}' without arguments", command));
				}
				else
				{
					_log.Debug(string.Format("Executing command '{0}' with arguments '{1}'", command, arguments));
				}
			}

			Process process = CreateProcess(command, arguments, encoding);

			var outputBuffer = new StringBuilder();
			var errorOutputBuffer = new StringBuilder();

			var or = new Thread(() => Read(process.StandardOutput, outputBuffer));
			var er = new Thread(() => Read(process.StandardError, errorOutputBuffer));

			StartProcess(process, command);

			or.Start();
			er.Start();

			if (!string.IsNullOrEmpty(input))
			{
				process.StandardInput.Write(input);
			}

			WaitProcessForExit(process, command);

			or.Join();
			er.Join();

			output = outputBuffer.ToString();
			errorOutput = errorOutputBuffer.ToString();

			if (_log.IsDebugEnabled)
			{
				_log.Debug(string.Format("Command '{0}' completed with return value {1}", command, process.ExitCode));
			}

			return process.ExitCode;
		}

		/// <summary>
		/// Executes the specified command with the specified arguments, reads command's output and error output.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="arguments">Command arguments.</param>
		/// <param name="output">Command output.</param>
		/// <param name="errorOutput">Command error output.</param>
		/// <returns>Return value from command</returns>
		public int Execute(string command, string arguments,
		                   out byte[] output, out string errorOutput)
		{
			return Execute(command, arguments, null, out output, out errorOutput);
		}

		/// <summary>
		/// Executes the specified command with the specified arguments, reads command's output and error output.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="arguments">Command arguments.</param>
		/// <param name="input">Input to the command.</param>
		/// <param name="output">Command output.</param>
		/// <param name="errorOutput">Command error output.</param>
		/// <returns>Return value from command</returns>
		public int Execute(string command, string arguments,
		                   string input, out byte[] output, out string errorOutput)
		{
			if (_log.IsDebugEnabled)
			{
				_log.Debug(string.Format("Executing command '{0}' with arguments '{1}'", command, arguments));
			}

			Process process = CreateProcess(command, arguments, Encoding.UTF8);

			var outputBuffer = new MemoryStream();
			var errorOutputBuffer = new StringBuilder();

			var or = new Thread(() => Read(process.StandardOutput.BaseStream, outputBuffer));
			var er = new Thread(() => Read(process.StandardError, errorOutputBuffer));

			StartProcess(process, command);

			or.Start();
			er.Start();

			if (!string.IsNullOrEmpty(input))
			{
				process.StandardInput.Write(input);
			}

			WaitProcessForExit(process, command);

			or.Join();
			er.Join();

			output = outputBuffer.ToArray();
			errorOutput = errorOutputBuffer.ToString();

			if (_log.IsDebugEnabled)
			{
				_log.Debug(string.Format("Command '{0}' completed with return value {1}", command, process.ExitCode));
			}

			return process.ExitCode;
		}

		private static Process CreateProcess(string command, string arguments, Encoding encoding)
		{
			var process = new Process
			{
				StartInfo =
					{
						FileName = command,
						UseShellExecute = false,
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true,
						RedirectStandardInput = true,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						Arguments = arguments,
						StandardOutputEncoding = encoding ?? Encoding.Default,
						StandardErrorEncoding = encoding ?? Encoding.Default,
					}
			};

			return process;
		}

		private static void StartProcess(Process process, string command)
		{
			try
			{
				process.Start();
			}
			catch (Win32Exception ex)
			{
				if (ex.NativeErrorCode == ERROR_FILE_NOT_FOUND)
				{
					throw new ApplicationException(string.Format("Cannot find executable: '{0}'", command));
				}
				if (ex.NativeErrorCode == ERROR_ACCESS_DENIED)
				{
					throw new ApplicationException(string.Format("Access denied to executable: '{0}'", command));
				}
				throw new ApplicationException(string.Format("Cannot execute command: '{0}'", command), ex);
			}
			catch (Exception ex)
			{
				throw new ApplicationException(string.Format("Cannot execute command: '{0}'", command), ex);
			}
		}

		private void WaitProcessForExit(Process process, string command)
		{
			if (!process.WaitForExit(_timeOut))
			{
				if (_log.IsDebugEnabled)
				{
					_log.Debug(string.Format("Command '{0}' did not finish in the specified amount of time and will be killed", command));
				}

				try
				{
					process.Kill();
				}
				catch (Exception ex)
				{
					if (_log.IsDebugEnabled)
					{
						_log.Debug(string.Format("Error killing timed out command '{0}'", command), ex);
					}
				}
				throw new ProcessTimeOutException(string.Format("Command '{0}' timed out and was killed.", command));
			}
		}

		private void Read(TextReader reader, StringBuilder output)
		{
			try
			{
				string line = reader.ReadLine();
				while (line != null)
				{
					if (output.Length > 0)
					{
						output.Append('\n');
					}
					output.Append(line);
					line = reader.ReadLine();
				}
			}
			catch (Exception ex)
			{
				_log.Error("Error reading process output", ex);
			}
		}

		private void Read(Stream input, Stream output)
		{
			try
			{
				using (var binaryWriter = new BinaryWriter(output))
				{
					var buffer = new byte[1024];
					int byteLen = input.Read(buffer, 0, buffer.Length);
					while (byteLen > 0)
					{
						binaryWriter.Write(buffer, 0, byteLen);
						byteLen = input.Read(buffer, 0, buffer.Length);
					}
				}
			}
			catch (Exception ex)
			{
				_log.Error("Error reading process output", ex);
			}
		}
	}
}