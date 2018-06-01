//
// THIS FILE IS AUTOGENERATED! ANY MANUAL MODIFICATIONS WILL BE LOST!
//

using System;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Tp.Integration.Common;
using Tp.Integration.Messages.Entities;

namespace Tp.Integration.Common
{
	// Autogenerated from GlobalSetting.hbm.xml properties: GlobalSettingID: Int32?, CompanyName: String, SMTPServer: String, SMTPPort: Int32?, SMTPLogin: String, SMTPPassword: String, SMTPAuthentication: Boolean?, SMTPSender: String, SMTPEnableSSLViaSTARTTLS: Boolean?, IsEmailNotificationsEnabled: Boolean?, HelpDeskPortalPath: String, AppHostAndPath: String, NotifyRequester: Boolean?, NotifyAutoCreatedRequester: Boolean?, DisableHttpAccess: Boolean?, CsvExportDelimiter: String, SecureJsonp: Boolean?, EntitiesCount: Int32?, MaxGeneratedSampleDataId: Int32?, SsoSettingJson: String, WorkspaceId: String, IsDefaultFullProjectAccess: Boolean?, DefaultRichEditor: RichEditorTypeEnum?
	public partial interface IGlobalSettingDTO : IDataTransferObject
	{
		String CompanyName { get; set; }
		String SMTPServer { get; set; }
		Int32? SMTPPort { get; set; }
		String SMTPLogin { get; set; }
		String SMTPPassword { get; set; }
		Boolean? SMTPAuthentication { get; set; }
		String SMTPSender { get; set; }
		Boolean? SMTPEnableSSLViaSTARTTLS { get; set; }
		Boolean? IsEmailNotificationsEnabled { get; set; }
		String HelpDeskPortalPath { get; set; }
		String AppHostAndPath { get; set; }
		Boolean? NotifyRequester { get; set; }
		Boolean? NotifyAutoCreatedRequester { get; set; }
		Boolean? DisableHttpAccess { get; set; }
		String CsvExportDelimiter { get; set; }
		Boolean? SecureJsonp { get; set; }
		Int32? EntitiesCount { get; set; }
		Int32? MaxGeneratedSampleDataId { get; set; }
		String SsoSettingJson { get; set; }
		String WorkspaceId { get; set; }
		Boolean? IsDefaultFullProjectAccess { get; set; }
		RichEditorTypeEnum? DefaultRichEditor { get; set; }
	}

	[Serializable]
	[DataContract]
	public partial class GlobalSettingDTO : DataTransferObject, IGlobalSettingDTO
	{
		[PrimaryKey]
		public override int? ID
		{
			get { return GlobalSettingID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				GlobalSettingID = value;
			}
		}
		[PrimaryKey]
		[DataMember]
		[XmlElement(Order=3)]
		public Int32? GlobalSettingID { get; set; }

		
		[DataMember]
		[XmlElement(Order=4)]
		public String CompanyName { get; set; }

		
		[DataMember]
		[XmlElement(Order=6)]
		public String SMTPServer { get; set; }

		
		[DataMember]
		[XmlElement(Order=7)]
		public Int32? SMTPPort { get; set; }

		
		[DataMember]
		[XmlElement(Order=8)]
		public String SMTPLogin { get; set; }

		
		[DataMember]
		[XmlElement(Order=9)]
		public String SMTPPassword { get; set; }

		
		[DataMember]
		[XmlElement(Order=10)]
		public Boolean? SMTPAuthentication { get; set; }

		
		[DataMember]
		[XmlElement(Order=11)]
		public String SMTPSender { get; set; }

		
		[DataMember]
		[XmlElement(Order=12)]
		public Boolean? IsEmailNotificationsEnabled { get; set; }

		
		[DataMember]
		[XmlElement(Order=13)]
		public String HelpDeskPortalPath { get; set; }

		
		[DataMember]
		[XmlElement(Order=14)]
		public String AppHostAndPath { get; set; }

		
		[DataMember]
		[XmlElement(Order=15)]
		public Boolean? NotifyRequester { get; set; }

		
		[DataMember]
		[XmlElement(Order=16)]
		public Boolean? NotifyAutoCreatedRequester { get; set; }

		
		[DataMember]
		[XmlElement(Order=17)]
		public Boolean? DisableHttpAccess { get; set; }

		
		[DataMember]
		[XmlElement(Order=18)]
		public String CsvExportDelimiter { get; set; }

		
		[DataMember]
		[XmlElement(Order=19)]
		public Boolean? SecureJsonp { get; set; }

		
		[DataMember]
		[XmlElement(Order=21)]
		public Int32? EntitiesCount { get; set; }

		
		[DataMember]
		[XmlElement(Order=22)]
		public Int32? MaxGeneratedSampleDataId { get; set; }

		
		[DataMember]
		[XmlElement(Order=23)]
		public String SsoSettingJson { get; set; }

		
		[DataMember]
		[XmlElement(Order=24)]
		public RichEditorTypeEnum? DefaultRichEditor { get; set; }

		
		[DataMember]
		[XmlElement(Order=25)]
		public String WorkspaceId { get; set; }

		
		[DataMember]
		[XmlElement(Order=26)]
		public Boolean? SMTPEnableSSLViaSTARTTLS { get; set; }

		
		[DataMember]
		[XmlElement(Order=27)]
		public Boolean? IsDefaultFullProjectAccess { get; set; }
	}

	public enum GlobalSettingField
	{
		CompanyName,
		SMTPServer,
		SMTPPort,
		SMTPLogin,
		SMTPPassword,
		SMTPAuthentication,
		SMTPSender,
		IsEmailNotificationsEnabled,
		HelpDeskPortalPath,
		AppHostAndPath,
		NotifyRequester,
		NotifyAutoCreatedRequester,
		DisableHttpAccess,
		CsvExportDelimiter,
		SecureJsonp,
		EntitiesCount,
		MaxGeneratedSampleDataId,
		SsoSettingJson,
		DefaultRichEditor,
		WorkspaceId,
		SMTPEnableSSLViaSTARTTLS,
		IsDefaultFullProjectAccess,
	}
}