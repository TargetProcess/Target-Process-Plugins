// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.Tests.Concurrency.Utils
{
	struct ChessOptions
	{
		public bool Debug { get; set; }
		public bool Repro { get; set; }
		public bool Trace { get; set; }
	}

	internal class Chess
	{
		private const string ERROR_FILE_NAME = "error.chess";

		public static void RunRemotely(Expression<Action> test, ChessOptions options = new ChessOptions())
		{
			var m = (MethodCallExpression)test.Body;
			Run(m.Method, options);
		}

		public static bool Run(Action test)
		{
			try
			{
				test();
				return true;
			}
			catch (Exception e)
			{
				File.WriteAllText(ERROR_FILE_NAME, e.ToString());
				return false;
			}
		}

		private static void Run(MethodBase methodInfo, ChessOptions options)
		{
			Process chessProcess = RunInChess(methodInfo, options);
			chessProcess.WaitForExit();
			DumpOutput(chessProcess, Console.Out);
			string errorCode = GetErrorCode();
			if (errorCode != "0")
			{
				if (File.Exists(ERROR_FILE_NAME))
				{
					Console.WriteLine(File.ReadAllText(ERROR_FILE_NAME));
				}
				NUnit.Framework.Assert.Fail("Chess return error code {0}", errorCode);
			}
		}

		private static void DumpOutput(Process chessProcess, TextWriter textWriter)
		{
			textWriter.WriteLine(chessProcess.StandardOutput.ReadToEnd());
			textWriter.WriteLine(chessProcess.StandardError.ReadToEnd());
		}

		private static Process RunInChess(MethodBase methodInfo, ChessOptions options)
		{
			string extraArguments = string.Format(" /arg:{0} /arg:{1}", methodInfo.DeclaringType, methodInfo.Name);
			string testAssemblyName = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Name;
			string testClassName = typeof (ChessTest).FullName;
			string domainAssemblyName = new AssemblyName(typeof (IAccount).Assembly.FullName).Name;
			string chessCommandLine = string.Format("{0}.dll /testclass:{1} /ia:{2} /detectraces", testAssemblyName, testClassName, domainAssemblyName);
			if (options.Debug)
			{
				chessCommandLine += " /break:s";
			}
			if(options.Repro)
			{
				chessCommandLine += " /repro";
			}
			if(options.Trace)
			{
				chessCommandLine += " /trace";
			}

			if(!File.Exists(@"Concurrency\chess\mchess.exe"))
			{
				NUnit.Framework.Assert.Ignore("Chess.exe is not present in Libs folder. Test will not run. Upload chess from http://research.microsoft.com/en-us/projects/chess/download.aspx");
			}

			var chessProcess = Process.Start(new ProcessStartInfo(@"Concurrency\chess\mchess.exe", chessCommandLine + extraArguments)
				              	{
				              		CreateNoWindow = true,
				              		UseShellExecute = false,
				              		RedirectStandardOutput = true,
				              		RedirectStandardError = true
				              	});
			return chessProcess;
		}

		private static string GetErrorCode()
		{
			XNamespace n = "http://research.microsoft.com/chess";
			var document = XDocument.Load("results.xml");
			return document.Root.Element(n + "finalStats").Attribute("exitCode").Value;
		}
	}
}