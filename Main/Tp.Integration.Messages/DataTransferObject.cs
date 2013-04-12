// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Tp.Integration.Common
{
	/// <summary>
	/// It is a basic class for all DTOs in system
	/// </summary>
	[Serializable][DataContract]
	public class DataTransferObject
	{
		/// <summary>
		/// Gets or sets the ID.
		/// </summary>
		/// <value>The ID.</value>
		[XmlElement(Order = 0)][DataMember(IsRequired = true)]
		public virtual int? ID { get; set; }

		public override string ToString()
		{
			return string.Format("{0} ID : {1}", GetType(), ID);
		}
	}
}