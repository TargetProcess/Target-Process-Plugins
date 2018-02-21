//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.IO;
using System.Linq;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Mashup;

namespace Tp.Integration.Plugin.Common.PluginLifecycle
{
    /// <summary>
    /// Represents mashups from "Mashups" folder from your plugin directory.
    /// </summary>
    public class DefaultPluginMashupRepository : IPluginMashupRepository
    {
        public const string PluginMashupDefaultPath = "Mashups";
        public string _mashupsPhysicalPath;

        public DefaultPluginMashupRepository()
        {
            _mashupsPhysicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginMashupDefaultPath);
        }

        public PluginMashup[] PluginMashups
        {
            get
            {
                var mashupCollection = new MashupCollection(_mashupsPhysicalPath);
                var result =
                    mashupCollection.Select(MashupToPluginMashupMapper).ToList();
                return result.ToArray();
            }
        }

        protected virtual PluginMashup MashupToPluginMashupMapper(Messages.PluginLifecycle.Mashup mashup)
        {
            return mashup.MashupName == ProfileEditorMashupName.ProfileEditorMashupPrefix
                ? new PluginProfileEditorMashup(mashup.MashupFilePaths)
                : new PluginMashup(mashup.MashupName, mashup.MashupFilePaths, new string[] { });
        }
    }
}
