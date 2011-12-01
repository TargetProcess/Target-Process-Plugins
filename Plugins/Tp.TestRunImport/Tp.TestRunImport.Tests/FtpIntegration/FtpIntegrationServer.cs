// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Diagnostics;
using System.Linq;

namespace Tp.TestRunImport.Tests.FtpIntegration
{
	public class FtpIntegrationServer : IDisposable
	{
		private const string ProcessName = "ftpdmin";
		private readonly Process _ftpProcess;

		public FtpIntegrationServer(string rootDirectory, int port = 21, bool hideFtpWindow = true)
		{
			var processesByName = Process.GetProcessesByName(ProcessName);
			foreach (var p in processesByName.Where(p => !p.HasExited))
			{
				p.Kill();
				p.WaitForExit();
			}
			var psInfo = new ProcessStartInfo
							{
								FileName = string.Format("{0}.exe", ProcessName),
								Arguments = string.Format("-p {0} -ha 127.0.0.1 \"{1}\"", port, rootDirectory),
								WindowStyle = hideFtpWindow ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal
							};
			_ftpProcess = Process.Start(psInfo);
		}

		public void Dispose()
		{
			if (!_ftpProcess.HasExited)
			{
				_ftpProcess.Kill();
				_ftpProcess.WaitForExit();
			}
		}
	}
}