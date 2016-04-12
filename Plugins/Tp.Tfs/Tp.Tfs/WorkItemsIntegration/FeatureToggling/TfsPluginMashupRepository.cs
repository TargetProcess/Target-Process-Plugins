// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Linq;
using StructureMap;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.Mashup;
using log4net;

namespace Tp.Tfs.WorkItemsIntegration.FeatureToggling
{
	public class TfsPluginMashupRepository : IPluginMashupRepository
	{
		public const string PluginMashupDefaultPath = "Mashups";
		public const string ProfileEditorMashupPath = "Mashups\\ProfileEditor";
		public const string WorkItemsMashupPath = "Mashups\\ProfileEditor\\WorkItemsIntegration";
		public const string SourceControlMashupPath = "Mashups\\ProfileEditor\\SourceControl";

		private readonly string _mashupsPhysicalPath;
		private readonly string _workItemsMashupsPhysicalPath;
		private readonly string _sourceControlMashupsPhysicalPath;
		private readonly string _profileEditorPhysicalPath;
		private readonly ILog _log;
		private readonly object _gate = new object();

		public TfsPluginMashupRepository()
		{
			_mashupsPhysicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginMashupDefaultPath);
			_workItemsMashupsPhysicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, WorkItemsMashupPath);
			_sourceControlMashupsPhysicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SourceControlMashupPath);
			_profileEditorPhysicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProfileEditorMashupPath);

			_log = ObjectFactory.GetInstance<ILogManager>().GetLogger(GetType());
		}

		public PluginMashup[] PluginMashups
		{
			get
			{
				lock (_gate)
				{
					bool state = ConfigHelper.GetWorkItemsState();

					CopyScripts(state);

					var mashupCollection = new MashupCollection(_mashupsPhysicalPath);
					var result = mashupCollection.
						Where(mashup => mashup.MashupName != ProfileEditorMashupName.ProfileEditorMashupPrefix).
						Select(mashup => new PluginMashup(mashup.MashupName, mashup.MashupFilePaths, new string[] {})).ToList();

					var profileEditorMashup =
						mashupCollection.FirstOrDefault(mashup => mashup.MashupName == ProfileEditorMashupName.ProfileEditorMashupPrefix);

					if (profileEditorMashup != null)
					{
						var profileEditor = new PluginProfileEditorMashup(
							profileEditorMashup.MashupFilePaths.Where(
								path => !path.Contains("WorkItemsIntegration") && !path.Contains("SourceControl")));

						result.Add(profileEditor);
					}

					return result.ToArray();
				}
			}
		}

		private void CopyScripts(bool state)
		{
			ClearProfileEditorDirectory();

			Copy(state ? _workItemsMashupsPhysicalPath : _sourceControlMashupsPhysicalPath);
		}

		private void ClearProfileEditorDirectory()
		{
			string[] filesToDelete = Directory.GetFiles(_profileEditorPhysicalPath, "*.*", SearchOption.TopDirectoryOnly);

			foreach (var file in filesToDelete)
				File.Delete(file);
		}

		private void Copy(string sourceDirectory)
		{
			if (!Directory.Exists(sourceDirectory))
			{
				_log.WarnFormat("Failed to find '{0}' directory.", sourceDirectory);
				return;
			}

			string[] files = Directory.GetFiles(sourceDirectory, "*.*");

			foreach (string file in files)
			{
				var fileName = Path.GetFileName(file);
				var destFile = Path.Combine(_profileEditorPhysicalPath, fileName);
				File.Copy(file, destFile, true);
			}
		}
	}
}
