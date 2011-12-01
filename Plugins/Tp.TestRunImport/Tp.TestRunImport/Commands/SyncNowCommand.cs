// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Xml;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.TestRunImport.Messages;
using Tp.Integration.Plugin.TestRunImport.Streams;
using Tp.Integration.Plugin.TestRunImport.TestRunImport;
using Tp.Integration.Plugin.TestRunImport.TestRunImportReaders;

namespace Tp.Integration.Plugin.TestRunImport.Commands
{
	public class SyncNowCommand : IPluginCommand
	{
		private readonly IStorageRepository _storageRepository;
		private readonly ILocalBus _localBus;
		private readonly IStreamFactory _streamFactory;
		private readonly ITestRunImportResultsReaderFactory _resultsReaderFactory;

		public SyncNowCommand(IStorageRepository storageRepository, ILocalBus localBus, IStreamFactory streamFactory,
		                                      ITestRunImportResultsReaderFactory resultsReaderFactory)
		{
			_storageRepository = storageRepository;
			_localBus = localBus;
			_streamFactory = streamFactory;
			_resultsReaderFactory = resultsReaderFactory;
		}

		public PluginCommandResponseMessage Execute(string args)
		{
			return OnExecute();
		}

		private PluginCommandResponseMessage OnExecute()
		{
			var resultMessage = new PluginCommandResponseMessage
			                    	{ResponseData = string.Empty, PluginCommandStatus = PluginCommandStatus.Succeed};

			var profile = _storageRepository.GetProfile<TestRunImportPluginProfile>();
			if (profile.FrameworkType == FrameworkTypes.FrameworkTypes.Selenium && profile.PostResultsToRemoteUrl)
			{
				return resultMessage;
			}

			try
			{
				var uri = new Uri(profile.ResultsFilePath);
				var factoryResult = _streamFactory.OpenStreamIfModified(uri, new LastModifyResult(), profile.PassiveMode);
				if (factoryResult != null)
				{
					_storageRepository.Get<LastModifyResult>().Clear();
					_storageRepository.Get<LastModifyResult>().Add(factoryResult.LastModifyResult);

					using (factoryResult.Stream)
					{
						using (var reader = new StreamReader(factoryResult.Stream))
						{
							try
							{
								var result = _resultsReaderFactory.GetResolver(profile, reader).GetTestRunImportResults();
								if (result.Count > 0)
								{
									_localBus.SendLocal(new TestRunImportResultDetectedLocalMessage
									{
										TestRunImportInfo =
											new TestRunImportInfo { TestRunImportResults = result.ToArray() }
									});
								}
							}
							catch (ApplicationException ex)
							{
								resultMessage.ResponseData = ex.Message;
								resultMessage.PluginCommandStatus = PluginCommandStatus.Error;
							}
							catch (XmlException ex)
							{
								resultMessage.ResponseData = string.Format("Error parsing results XML file; {0}", ex.Message);
								resultMessage.PluginCommandStatus = PluginCommandStatus.Error;
							}
							catch (Exception ex)
							{
								resultMessage.ResponseData = string.Format("Error parsing results XML file; {0}", ex.Message);
								resultMessage.PluginCommandStatus = PluginCommandStatus.Error;
							}
						}
					}
				}
			}
			catch (UriFormatException ex)
			{
				resultMessage.ResponseData = string.Format("Specified path has invalid format. {0}", ex.Message);
				resultMessage.PluginCommandStatus = PluginCommandStatus.Error;
			}
			catch (ApplicationException ex)
			{
				resultMessage.ResponseData = string.Format("Specified path has invalid format. {0}", ex.Message);
				resultMessage.PluginCommandStatus = PluginCommandStatus.Error;
			}
			catch (Exception ex)
			{
				resultMessage.ResponseData = string.Format("Could not read file \"{0}\": {1}", profile.ResultsFilePath, ex.Message);
				resultMessage.PluginCommandStatus = PluginCommandStatus.Error;
			}
			return resultMessage;
		}

		public string Name
		{
			get { return "SyncNow"; }
		}
	}
}