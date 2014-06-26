// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NBehave.Narrator.Framework;
using NUnit.Framework;
using Tp.Integration.Plugin.Common.Validation;
using Tp.PopEmailIntegration.EmailReader.Client;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.PopEmailIntegration
{
	[TestFixture, ActionSteps]
    [Category("PartPlugins0")]
	public class CheckConnectionSpecs
	{
		private ConnectionSettings _settings;
		private readonly PluginProfileErrorCollection _errors = new PluginProfileErrorCollection();
		
		[Test, Ignore]
		public void ShouldCheckValidPop3Connection()
		{
			@"
				Given server set to 'pop.targetprocess.com', port to '110', use ssl to 'false', credentials to 'test3@targetprocess.com:test3'
				When check connection
				Then connection should be resolved as valid
			"
				.Execute();
		}

		[Test, Ignore]
		public void ShouldCheckInValidCredentials()
		{
			@"
				Given server set to 'pop.targetprocess.com', port to '110', use ssl to 'false', credentials to 'login:pass'
				When check connection
				Then server should not login
			"
				.Execute();
		}

		[Given("server set to '$server', port to '$port', use ssl to '$ssl', credentials to '$credentials")]
		public void InitMailSettings(string server, int port, bool ssl, string credentials)
		{
			var credentialsPair = credentials.Split(':');

			_settings = new ConnectionSettings
			            	{
			            		Login = credentialsPair[0],
			            		Password = credentialsPair[1],
			            		MailServer = server,
			            		Port = port,
			            		Protocol = "pop3",
			            		UseSSL = ssl
			            	};
		}

		[When("check connection")]
		public void CheckConnection()
		{
			var client = new MailBeePop3EmailClient(_settings);
			client.CheckConnection(_errors);
		}

		[Then("connection should be resolved as valid")]
		public void CheckConnectionIsValid()
		{
			_errors.Any().Should(Be.False);
		}

		[Then("server should not login")]
		public void ShouldNotLogin()
		{
			_errors.Count().Should(Be.EqualTo(2));
			_errors.Select(e => e.FieldName).Should(Be.EquivalentTo(new []{"Login", "Password"}));
		}
	}
}