// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using MailBee.Mime;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.PopEmailIntegration.EmailReader.Client
{
	public interface IEmailClient : IDisposable
	{
		void Connect();
		void Login();
		void Disconnect();
		string[] GetServerUids();
		void CheckConnection(PluginProfileErrorCollection errors);
		MailMessage DownloadMessage(string uid);
		bool IsDsnMessage(MailMessage message);
	}
}