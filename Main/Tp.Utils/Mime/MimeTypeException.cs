// 
// Copyright (c) 2005-2008 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Generic;
using System.Text;

namespace Tp.Utils.Mime
{
	/// <summary>
	/// Summary description for MimeTypeException.
	/// </summary>
	public class MimeTypeException : ApplicationException
	{
		#region Class Constructor

		/// <summary>
		/// 
		/// </summary>
		public MimeTypeException()
			: base("Mime detection exception") {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strMsg"></param>
		public MimeTypeException(String strMsg)
			: base(strMsg) {}

		#endregion
	}
}