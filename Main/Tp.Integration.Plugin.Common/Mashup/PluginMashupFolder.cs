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
    public class PluginMashupFolder : PluginMashup
    {
        public PluginMashupFolder(string mashupName, string folder, string placeholder)
            : this(mashupName, folder, new[] { placeholder })
        {
        }

        public PluginMashupFolder(string mashupName, string folder, string[] placeholders)
            : base(mashupName, GetFilesFromFolder(folder), placeholders)
        {
        }

        private static IEnumerable<string> GetFilesFromFolder(string folder)
        {
            return Directory.GetFiles(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder)).Select(x => Path.Combine(folder, Path.GetFileName(x)));
        }
    }
}
