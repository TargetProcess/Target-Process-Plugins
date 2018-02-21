// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tp.Integration.Plugin.Common.Mashup
{
    /// <summary>
    /// Represents a mashup for editing plugin profile.
    /// </summary>
    public class PluginProfileEditorMashupFolder : PluginProfileEditorMashup
    {
        /// <summary>
        /// Creates a new instance of PluginProfileEditorMashupFolder.
        /// </summary>
        /// <param name="folder">The folder with javascript files of mashup. All files from this folder will be included in mashup.</param>
        public PluginProfileEditorMashupFolder(string folder)
            : base(GetFilesFromFolder(folder))
        {
        }

        private static IEnumerable<string> GetFilesFromFolder(string folder)
        {
            return Directory.GetFiles(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder)).Select(x => Path.Combine(folder, Path.GetFileName(x)));
        }
    }
}
