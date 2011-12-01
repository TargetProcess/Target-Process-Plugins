// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.IO;
using System.Xml.Serialization;
using Tp.Bugzilla.BugFieldConverters;

namespace Tp.Bugzilla
{
	/// <summary>
	/// Parses XML and return bugzilla bug DTO object
	/// </summary>
	public class BugzillaParser<T>
	{
		public T Parse(string content)
		{
			TextReader reader = new StringReader(DescriptionConverter.CleanUpContent(content));
			var serializer = new XmlSerializer(typeof (T));
			var bugzillaObject = (T) serializer.Deserialize(reader);
			return bugzillaObject;
		}

		public T Parse(Stream stream)
		{
			var serializer = new XmlSerializer(typeof (T));
			TextReader reader = new StreamReader(stream);

			var content = DescriptionConverter.CleanUpContent(reader.ReadToEnd());

			var bugzillaObject = (T) serializer.Deserialize(new StringReader(content));
			return bugzillaObject;
		}
	}
}