// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Integration.Plugin.Common.PluginLifecycle
{
    internal class PluginIcon
    {
        private readonly IActivityLogger _log;
        private readonly string _iconFilePath;

        public PluginIcon(IPluginMetadata pluginMetadata, IActivityLogger log)
        {
            _log = log;
            var iconFileRelativePath = pluginMetadata.PluginData.IconFilePath;
            if (!string.IsNullOrEmpty(iconFileRelativePath))
            {
                _iconFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    pluginMetadata.PluginData.IconFilePath);
            }
        }

        public string GetIconContent()
        {
            if (string.IsNullOrEmpty(_iconFilePath))
            {
                return string.Empty;
            }

            if (!File.Exists(_iconFilePath))
            {
                _log.WarnFormat("Cannot find plugin icon by path : '{0}'", _iconFilePath);
                return string.Empty;
            }

            return Convert.ToBase64String(File.ReadAllBytes(_iconFilePath));
        }
    }
}
