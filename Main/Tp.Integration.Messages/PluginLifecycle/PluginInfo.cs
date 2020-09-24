﻿using System;

namespace Tp.Integration.Messages.PluginLifecycle
{
    [Serializable]
    public class PluginInfo
    {
        public PluginInfo()
            : this(string.Empty)
        {
        }

        public PluginInfo(PluginName name)
        {
            Name = name;
            Accounts = Array.Empty<PluginAccount>();
        }

        public PluginName Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public PluginAccount[] Accounts { get; set; }
        public string PluginInputQueue { get; set; }
        public string PluginIconContent { get; set; }
        public bool IsHidden { get; set; }
    }
}
