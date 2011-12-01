// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Text;
using System;

namespace Tp.SourceControl.VersionControlSystem
{
	/// <summary>
	/// Notifies about errors in source control intergration.
	/// </summary>
	public class VersionControlException : ApplicationException
	{
		/// <summary>
		/// Creates new instance of this class.
		/// </summary>
		/// <param name="message">Error message</param>
		public VersionControlException(string message)
			: base(message) {}

		/// <summary>
		/// Creates new instance of this class.
		/// </summary>
		/// <param name="message">Error message.</param>
		/// <param name="innerException">Inner exception.</param>
		public VersionControlException(string message, Exception innerException)
			: base(message, innerException) {}
	}
}