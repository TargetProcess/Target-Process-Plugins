// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Net;
using MiniHttpd;

namespace Tp.TestRunImport.Tests.HttpIntegration
{
	internal class HttpIntegrationServer
	{
		private readonly HttpWebServer _webServer;

		public HttpIntegrationServer(string rootDirectory, int port = 23)
		{
			_webServer = new HttpWebServer(IPAddress.Parse("127.0.0.1"), port, new DriveDirectory(rootDirectory));
			_webServer.Start();
		}

		public void Dispose()
		{
			if (_webServer != null && _webServer.IsRunning)
			{
				_webServer.Dispose();
			}
		}
	}
}