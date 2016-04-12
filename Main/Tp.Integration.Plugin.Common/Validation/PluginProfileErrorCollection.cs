// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tp.Integration.Plugin.Common.Validation
{
	/// <summary>
	/// Represents a collection of profile validation errors.
	/// </summary>
	[CollectionDataContract]
	public class PluginProfileErrorCollection : IEnumerable<PluginProfileError>
	{
		private readonly List<PluginProfileError> _errors = new List<PluginProfileError>();

		public IEnumerator<PluginProfileError> GetEnumerator()
		{
			return _errors.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Adds the error to collection.
		/// </summary>
		/// <param name="error">The error to add.</param>
		public void Add(PluginProfileError error)
		{
			_errors.Add(error);
		}
	}
}