//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Runtime.Serialization;
using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.SourceControl.Settings
{
	[DataContract, KnownType(typeof(ISourceControlConnectionSettingsSource))]
	public class ConnectionSettings : ISourceControlConnectionSettingsSource
	{
		public const string UriField = "Uri";
		public const string LoginField = "Login";
		public const string PasswordField = "Password";

		private string _uri;

		[DataMember]
		public string Uri
		{
			get { return _uri; }
			set
			{
				if (value != null)
				{
					_uri = value.Trim();
				}
			}
		}

		[DataMember]
		public string Login { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public string StartRevision { get; set; }

		[DataMember]
		public MappingContainer UserMapping { get; set; }
	}
}