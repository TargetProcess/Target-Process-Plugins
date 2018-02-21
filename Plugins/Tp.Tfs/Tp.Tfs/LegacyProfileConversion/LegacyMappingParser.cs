// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System;

namespace Tp.Tfs.LegacyProfileConversion
{
    public class LegacyMappingParser
    {
        public LegacyMappingParser()
        {
            _maps.Add("Users", new StringDictionary());
        }

        public StringDictionary Users
        {
            get { return _maps["Users"]; }
        }

        private readonly Dictionary<string, StringDictionary> _maps = new Dictionary<string, StringDictionary>();

        public string Maps
        {
            set
            {
                var reader = new StringReader(value);

                foreach (var map in _maps)
                {
                    ReadMap(map.Value, reader, map.Key);
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
