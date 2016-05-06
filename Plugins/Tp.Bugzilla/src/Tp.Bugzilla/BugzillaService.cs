// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Tp.BugTracking.ConnectionValidators;
using Tp.Bugzilla.BugzillaQueries;
using Tp.Bugzilla.ConnectionValidators;
using Tp.Bugzilla.Schemas;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Bugzilla
{
	public class BugzillaService : IBugzillaService
	{
		private readonly BugzillaProfile _bugzillaProfile;

		public BugzillaService(IStorageRepository storageRepository)
		{
			_bugzillaProfile = storageRepository.GetProfile<BugzillaProfile>();
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
				var validators = new Queue<Validator>();

				var connectionValidator = new ConnectionValidator(profile);
				validators.Enqueue(connectionValidator);

				var scriptValidator = new ScriptValidator(profile);
				validators.Enqueue(scriptValidator);

				var responseValidator = new ResponseValidator(profile, scriptValidator);
				validators.Enqueue(responseValidator);

				var deserializeValidator = new DeserializeValidator(profile, responseValidator);
				validators.Enqueue(deserializeValidator);

				var settingsValidator = new SettingsValidator(profile, deserializeValidator);
				validators.Enqueue(settingsValidator);

				var savedQueryValidator = new SavedQueryValidator(profile);
				validators.Enqueue(savedQueryValidator);

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
						Message = $"The connection with {profile} is failed. {ex.Message}"
					});
				throw new BugzillaPluginProfileException(profile, errors);
			}
		}

		public int[] GetChangedBugIds(DateTime? date)
		{
			var offset = GetTimeOffset();
			DateTime? bugzillaDateTime = null;
			if (date.HasValue)
			{
				bugzillaDateTime = date.Value.ToUniversalTime().Add(offset);
			}

			return ObjectFactory.GetInstance<BugzillaUrl>().GetChangedBugsIds(bugzillaDateTime);
		}

		public bugCollection GetBugs(int[] bugIDs)
		{
			if (bugIDs.Length == 0) return new bugCollection();

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
			if (TimeSpan.TryParse(response, out timeOffset))
			{
				return timeOffset;
			}

			throw new ApplicationException($"Invalid offset value returned from Bugzilla service: {response}");
		}

		private string ExecuteBugzillaQuery(IBugzillaQuery query)
		{
			try
			{
				return ObjectFactory.GetInstance<BugzillaUrl>().ExecuteOnBugzilla(query);
			}
			catch (Exception ex)
			{
				throw new ApplicationException(
					$"Synchronization failed for following operation: {query}. Profile : '{_bugzillaProfile}'", ex);
			}
		}

		private void ExecuteBugzillaNonQuery(IBugzillaQuery query)
		{
			var result = ExecuteBugzillaQuery(query);

			if (result != "OK")
				throw new ApplicationException($"There was exception during performing following operation: {query.GetOperationDescription()}. {result}");
		}
	}
}
