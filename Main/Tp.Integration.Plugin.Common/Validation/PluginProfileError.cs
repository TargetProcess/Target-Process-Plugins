// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System.Runtime.Serialization;

namespace Tp.Integration.Plugin.Common.Validation
{
	/// <summary>
	/// Stores inforamtion about profile validation error.
	/// </summary>
	[DataContract]
	public class PluginProfileError
	{
        public PluginProfileError()
        {
            Status = PluginProfileErrorStatus.Error;
        }

		/// <summary>
		/// Error message.
		/// </summary>
		[DataMember]
		public string Message { get; set; }

		/// <summary>
		/// Profile field that did not pass validation.
		/// </summary>
		[DataMember]
		public string FieldName { get; set; }

		/// <summary>
		/// Additional info about error.
		/// </summary>
		[DataMember]
		public string AdditionalInfo { get; set; }

	    /// <summary>
	    /// Error status. Variants: 'Error', 'Warning'
	    /// </summary>
        [DataMember]
        public PluginProfileErrorStatus Status { get; set; }

		public override string ToString()
		{
			return string.Format("{0}: {1}", FieldName, Message);
		}
	}
}