// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;

namespace Tp.Integration.Plugin.TestRunImport
{
	[Profile]
	[DataContract]
	public class TestRunImportPluginProfile : TestRunImportSettings, ISynchronizableProfile, IValidatable
	{
		public const string PatternTestIdGroupName = "testId";
		public const string PatternTestNameGroupName = "testName";

		public TestRunImportPluginProfile()
		{
			FrameworkType = FrameworkTypes.FrameworkTypes.None;
			SynchronizationInterval = 24;
		}

		#region ISynchronizableProfile members

		[DataMember]
		public int SynchronizationInterval { get; set; }

		#endregion

		public void Validate(PluginProfileErrorCollection errors)
		{
			if (FrameworkType == FrameworkTypes.FrameworkTypes.Selenium && PostResultsToRemoteUrl)
			{
				ValidateAuthTokenUserId(errors);
				ValidateRemoteResultsUrl(errors);
			}
			else
			{
				ValidateResultsFilePath(errors);
				ValidateIntegrationInterval(errors);
			}
			ValidateProject(errors);
			ValidateTestPlan(errors);
			ValidateFramework(errors);
			ValidateRegExp(errors);
		}

		public void ValidateMapperData(PluginProfileErrorCollection errors)
		{
			ValidateResultsFilePathForMapping(errors);
			ValidateProject(errors);
			ValidateTestPlan(errors);
			ValidateFramework(errors);
			ValidateRegExp(errors);
		}

		public void ValidateSeleniumUrlData(PluginProfileErrorCollection errors)
		{
			ValidateAuthTokenUserId(errors);
		}

		private void ValidateProject(PluginProfileErrorCollection errors)
		{
			if (Project <= 0)
			{
				errors.Add(new PluginProfileError { FieldName = "Project", Message = "Project should be specified" });
			}
		}

		private void ValidateAuthTokenUserId(PluginProfileErrorCollection errors)
		{
			if (AuthTokenUserId <= 0)
			{
				errors.Add(new PluginProfileError { FieldName = "AuthTokenUserId", Message = "User for authentication should be specified" });
			}
		}

		private void ValidateTestPlan(PluginProfileErrorCollection errors)
		{
			if (TestPlan <= 0)
			{
				errors.Add(new PluginProfileError { FieldName = "TestPlan", Message = "Test Plan should be specified" });
			}
		}

		private void ValidateFramework(PluginProfileErrorCollection errors)
		{
			if (FrameworkType == FrameworkTypes.FrameworkTypes.None)
			{
				errors.Add(new PluginProfileError { FieldName = "FrameworkType", Message = "Framework type should be specified" });
			}
		}

		private void ValidateRemoteResultsUrl(PluginProfileErrorCollection errors)
		{
			if (string.IsNullOrEmpty(RemoteResultsUrl) || string.IsNullOrEmpty(RemoteResultsUrl.Trim()))
			{
				errors.Add(new PluginProfileError { FieldName = "RemoteResultsUrl", Message = string.Empty });
			}
		}

		private void ValidateResultsFilePath(PluginProfileErrorCollection errors)
		{
			ValidateResultsFilePathForMapping(errors, false);
		}

		private void ValidateResultsFilePathForMapping(PluginProfileErrorCollection errors, bool checkFileExists = true)
		{
			if (string.IsNullOrEmpty(ResultsFilePath) || string.IsNullOrEmpty(ResultsFilePath.Trim()))
			{
				errors.Add(new PluginProfileError { FieldName = "ResultsFilePath", Message = "Test result XML file path should be specified" });
			}
			else
			{
				try
				{
					ResultsFilePath = ResultsFilePath.Trim();
					var request = WebRequest.Create(new Uri(ResultsFilePath));

					if (request is FileWebRequest)
					{
						var path = request.RequestUri.LocalPath;
						var fileInfo = new FileInfo(path);
						if (checkFileExists && !fileInfo.Exists)
						{
							errors.Add(new PluginProfileError { FieldName = "ResultsFilePath", Message = string.Format("File \"{0}\" does not exist", path) });
						}
					}
					else if (request is HttpWebRequest)
					{
						if (checkFileExists)
						{
							request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
							var response = (HttpWebResponse) request.GetResponse();
							response.Close();
						}
					}
					else if (request is FtpWebRequest)
					{
						if (checkFileExists)
						{
							var ftpWebRequest = (FtpWebRequest) request;
							ftpWebRequest.UsePassive = PassiveMode;
							request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
							var response = (FtpWebResponse) request.GetResponse();
							response.Close();
						}
					}
					else
					{
						errors.Add(new PluginProfileError
									{
										FieldName = "ResultsFilePath",
										Message = string.Format("Unsupported resource \"{0}\"", request.RequestUri)
									});
					}
				}
				catch (Exception ex)
				{
					errors.Add(new PluginProfileError
								{
									FieldName = "ResultsFilePath",
									Message = ex.Message
								});
				}
			}
		}

		private void ValidateIntegrationInterval(PluginProfileErrorCollection errors)
		{
			if (SynchronizationInterval <= 0)
			{
				errors.Add(new PluginProfileError
							{
								FieldName = "SynchronizationInterval",
								Message = "Integration interval should be an integer greater than zero"
							});
			}
			SynchronizationInterval = SynchronizationInterval * 60;
		}

		private void ValidateRegExp(PluginProfileErrorCollection errors)
		{
			if (RegExp == null) return;
			RegExp = RegExp.Trim();
			if (RegExp == string.Empty)
			{
				RegExp = null;
			}
			else
			{
				try
				{
					var rx = new Regex(RegExp);
					var names = rx.GetGroupNames();
					var s1 = Array.Find(names, s => s == PatternTestIdGroupName);
					var s2 = Array.Find(names, s => s == PatternTestNameGroupName);
					if ((s1 == null && s2 == null) || (s1 != null && s2 != null))
					{
						errors.Add(new PluginProfileError
									{
										FieldName = "RegExp",
										Message =
											string.Format("Regular expression must define either &lt;{0}&gt; or &lt;{1}&gt; group",
														  PatternTestIdGroupName,
														  PatternTestNameGroupName)
									});
					}
				}
				catch (ArgumentException ex)
				{
					errors.Add(new PluginProfileError { FieldName = "RegExp", Message = string.Format("Cannot parse regular expression: {0}", ex.Message) });
				}
			}
		}
	}
}