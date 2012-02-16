// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using StructureMap;
using Tp.BugTracking.ConnectionValidators;
using Tp.Bugzilla.BugzillaQueries;
using Tp.Bugzilla.ConnectionValidators;
using Tp.Bugzilla.Schemas;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.Validation;
using log4net;

namespace Tp.Bugzilla
{
	public class BugzillaService : IBugzillaService
	{
		private readonly IStorageRepository _storageRepository;
		private readonly BugzillaProfile _bugzillaProfile;

		public BugzillaService(IStorageRepository storageRepository)
		{
			_storageRepository = storageRepository;
			_bugzillaProfile = _storageRepository.GetProfile<BugzillaProfile>();
		}

		public BugzillaService()
		{
		}

		public bugzilla_properties CheckConnection()
		{
			return CheckConnection(_bugzillaProfile);
		}

		public bugzilla_properties CheckConnection(BugzillaProfile profile)
		{
			var errors = new PluginProfileErrorCollection();
			try
			{
				SetServerCertificateValidationCallback(profile);

				var validators = new Queue<Validator>();

				var connectionValidator = new ConnectionValidator(profile);
				validators.Enqueue(connectionValidator);

				var scriptValidator = new ScriptValidator(profile);
				validators.Enqueue(scriptValidator);

				var responseValidator = new ResponseValidator(profile, scriptValidator);
				validators.Enqueue(responseValidator);

				var deserializeValidator = new DeserializeValidator(profile, responseValidator);
				validators.Enqueue(deserializeValidator);

				var savedQueryValidator = new SavedQueryValidator(profile);
				validators.Enqueue(savedQueryValidator);

				var settingsValidator = new SettingsValidator(profile, deserializeValidator);
				validators.Enqueue(settingsValidator);

				while (validators.Count > 0)
				{
					var validator = validators.Dequeue();
					validator.Execute(errors);
				}

				if (errors.Any())
				{
					throw new BugzillaPluginProfileException(profile, errors);
				}

				return deserializeValidator.Data;
			}
			catch (BugzillaPluginProfileException)
			{
				throw;
			}
			catch (Exception ex)
			{
				errors.Add(new PluginProfileError
				           	{
				           		FieldName = BugzillaProfile.ProfileField,
				           		Message = string.Format("The connection with {0} is failed. {1}", profile, ex.Message)
				           	});
				throw new BugzillaPluginProfileException(profile, errors);
			}
		}

		public int[] GetChangedBugIds(DateTime date)
		{
			var offset = GetTimeOffset();
			var bugzillaDateTime = date.ToUniversalTime().Add(offset);

			return ObjectFactory.GetInstance<BugzillaUrl>().GetChangedBugsIds(bugzillaDateTime);
		}

		public bugCollection GetBugs(int[] bugIDs)
		{
			if (bugIDs.Length == 0) return new bugCollection();

			SetServerCertificateValidationCallback();

			return ObjectFactory.GetInstance<BugzillaUrl>().GetBugs(bugIDs);
		}

		public List<string> GetStatuses()
		{
			return CheckConnection().statuses.nameCollection.ToArray().Select(x => x.ToString()).ToList();
		}

		public List<string> GetResolutions()
		{
			return CheckConnection().resolutions.nameCollection.ToArray().Select(x => x.ToString()).ToList();
		}

		public void Execute(IBugzillaQuery query)
		{
			ExecuteBugzillaNonQuery(query);
		}

		public TimeSpan GetTimeOffset()
		{
			var response = ExecuteBugzillaQuery(new BugzillaTimezoneQuery());

			TimeSpan timeOffset;
			if(TimeSpan.TryParse(response, out timeOffset))
			{
				return timeOffset;
			}

			throw new ApplicationException(string.Format("Invalid offset value returned from Bugzilla service: {0}", response));
		}

		private string ExecuteBugzillaQuery(IBugzillaQuery query)
		{
			SetServerCertificateValidationCallback();

			try
			{
				return ObjectFactory.GetInstance<BugzillaUrl>().ExecuteOnBugzilla(query);
			}
			catch (Exception ex)
			{
				throw new ApplicationException(
					string.Format("Synchronization failed for following operation: {0}. Profile : '{1}'", query,
								  _bugzillaProfile), ex);
			}
		}

		private void ExecuteBugzillaNonQuery(IBugzillaQuery query)
		{
			var result = ExecuteBugzillaQuery(query);

			if (result != "OK")
				throw new ApplicationException(string.Format(
					"There was exception during performing following operation: {0}. {1}", query.GetOperationDescription(), result));
		}

		private void SetServerCertificateValidationCallback()
		{
			SetServerCertificateValidationCallback(_bugzillaProfile);
		}

		private void SetServerCertificateValidationCallback(BugzillaProfile profile)
		{
			if (ServicePointManager.ServerCertificateValidationCallback == null)
			{
				var validator = new RemoteCertificateValidator(profile);
				ServicePointManager.ServerCertificateValidationCallback = validator.ValidateRemoteCertificate;
			}
		}

		private class RemoteCertificateValidator
		{
			private readonly BugzillaProfile _profile;

			public RemoteCertificateValidator(BugzillaProfile profile)
			{
				_profile = profile;
			}

			public bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain,
			                                      SslPolicyErrors sslpolicyerrors)
			{
				var webRequest = sender as HttpWebRequest;
				if (webRequest != null)
				{
					return webRequest.Address.Host == (new Uri(_profile.Url)).Host;
				}
				return false;
			}
		}
	}
}
