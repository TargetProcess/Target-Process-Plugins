// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace Tp.Bugzilla.LegacyProfileConversion
{
    public class LegacyMappingParser
    {
        public LegacyMappingParser()
        {
            _maps.Add(LegacyBugzillaProfileFields.Users, new StringDictionary());
            _maps.Add(LegacyBugzillaProfileFields.Severities, new StringDictionary());
            _maps.Add(LegacyBugzillaProfileFields.Priorities, new StringDictionary());
            _maps.Add(LegacyBugzillaProfileFields.EntityStates, new StringDictionary());
        }

        public StringDictionary Users
        {
            get { return _maps["Users"]; }
        }

        public StringDictionary Severities
        {
            get { return _maps["Severities"]; }
        }

        public StringDictionary Priorities
        {
            get { return _maps["Priorities"]; }
        }

        public StringDictionary EntityStates
        {
            get { return _maps["EntityStates"]; }
        }

        private readonly Dictionary<string, StringDictionary> _maps = new Dictionary<string, StringDictionary>();

        public string Maps
        {
            set
            {
                var reader = new StringReader(value);
                var mapLine = reader.ReadLine();
                while (!string.IsNullOrEmpty(mapLine))
                {
                    ReadMap(mapLine);
                    mapLine = reader.ReadLine();
                }
            }
            get
            {
                var builder = new StringBuilder();

                foreach (KeyValuePair<string, StringDictionary> map in _maps)
                {
                    WriteMap(builder, map.Key, map.Value);
                }

                return builder.ToString();
            }
        }

        private void ReadMap(string mapLine)
        {
            var mapName = mapLine.Split(':')[0];

            var maps = _maps.Where(m => m.Key == mapName).ToList();

            if (!maps.Any())
                return;

            var map = maps.Single();

            var values = mapLine.Replace(string.Format("{0}:", mapName), string.Empty).Split(new[] { ';' },
                StringSplitOptions.
                    RemoveEmptyEntries);
            foreach (string value in values)
            {
                string[] parts = value.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                map.Value.Add(parts[0], parts[1]);
            }
        }

        private static void ReadMap(StringDictionary map, TextReader reader, string mapName)
        {
            map.Clear();

            var mappings = reader.ReadLine().Replace(string.Format("{0}:", mapName), string.Empty).Split(new[] { ';' },
                StringSplitOptions.
                    RemoveEmptyEntries);

            foreach (string mapping in mappings)
            {
                string[] parts = mapping.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                map.Add(parts[0], parts[1]);
            }
        }

        private static void WriteMap(StringBuilder builder, string mapName, StringDictionary map)
        {
            builder.AppendFormat("{0}:", mapName);

            foreach (DictionaryEntry keyValuePair in map)
            {
                builder.AppendFormat("{0}:{1};", keyValuePair.Key, keyValuePair.Value);
            }

            builder.AppendLine();
        }
    }
}
