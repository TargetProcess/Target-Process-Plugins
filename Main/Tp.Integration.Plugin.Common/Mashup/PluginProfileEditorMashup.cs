// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage;

namespace Tp.Integration.Plugin.Common.Mashup
{
    /// <summary>
    /// Represents a mashup for editing plugin profile.
    /// </summary>
    public class PluginProfileEditorMashup : PluginMashup
    {
        /// <summary>
        /// Creates a new instance of PluginProfileEditorMashup.
        /// </summary>
        /// <param name="filePaths">The collection of paths to files to be included in mashup. Paths may be relative or absolute.</param>
        public PluginProfileEditorMashup(IEnumerable<string> filePaths)
            : base(ProfileEditorMashupName.ProfileEditorMashupPrefix, filePaths, new string[] { })
        {
        }

        /// <summary>
        /// Add default placeholder if no explicit configuration
        /// </summary>
        /// <returns></returns>
        public override PluginMashupScript[] GetScripts()
        {
            var scripts = base.GetScripts();
            if (HasExplicitConfig) return scripts;

            var newScripts = new List<PluginMashupScript>(scripts)
            {
                new PluginMashupScript
                {
                    FileName = string.Format("{0}.cfg", ProfileEditorMashupName.ProfileEditorMashupPrefix),
                    ScriptContent = string.Format("Placeholders:{0}", GetDefaultPlaceholderName())
                }
            };
            return newScripts.ToArray();
        }

        private static string GetDefaultPlaceholderName()
        {
            return new ProfileEditorMashupName(ObjectFactory.GetInstance<IPluginContext>().PluginName.Value).Value;
        }
    }
}
