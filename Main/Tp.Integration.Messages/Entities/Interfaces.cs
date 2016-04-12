using System;
using Tp.Integration.Messages.Entities;

namespace Tp.Integration.Common
{
	public interface IEmailAttachmentDTO
	{
		Byte[] Buffer { get; set; }
		String Name { get; set; }
	}

	public interface IDataTransferObject
	{
		int? ID { get; set; }

		T Accept<T>(IDTOVisitor<T> visitor);
	}

	public interface ICustomFieldHolderDTO
	{
		Field[] CustomFieldsMetaInfo { get; set; }

		String CustomField1 { get; set; }
		String CustomField2 { get; set; }
		String CustomField3 { get; set; }
		String CustomField4 { get; set; }
		String CustomField5 { get; set; }
		String CustomField6 { get; set; }
		String CustomField7 { get; set; }
		String CustomField8 { get; set; }
		String CustomField9 { get; set; }
		String CustomField10 { get; set; }
		String CustomField11 { get; set; }
		String CustomField12 { get; set; }
		String CustomField13 { get; set; }
		String CustomField14 { get; set; }
		String CustomField15 { get; set; }
		String CustomField16 { get; set; }
		String CustomField17 { get; set; }
		String CustomField18 { get; set; }
		String CustomField19 { get; set; }
		String CustomField20 { get; set; }
		String CustomField21 { get; set; }
		String CustomField22 { get; set; }
		String CustomField23 { get; set; }
		String CustomField24 { get; set; }
		String CustomField25 { get; set; }
		String CustomField26 { get; set; }
		String CustomField27 { get; set; }
		String CustomField28 { get; set; }
		String CustomField29 { get; set; }
		String CustomField30 { get; set; }
		String CustomField31 { get; set; }
		String CustomField32 { get; set; }
		String CustomField33 { get; set; }
		String CustomField34 { get; set; }
		String CustomField35 { get; set; }
		String CustomField36 { get; set; }
		String CustomField37 { get; set; }
		String CustomField38 { get; set; }
		String CustomField39 { get; set; }
		String CustomField40 { get; set; }
		String CustomField41 { get; set; }
		String CustomField42 { get; set; }
		String CustomField43 { get; set; }
		String CustomField44 { get; set; }
		String CustomField45 { get; set; }
		String CustomField46 { get; set; }
		String CustomField47 { get; set; }
		String CustomField48 { get; set; }
		String CustomField49 { get; set; }
		String CustomField50 { get; set; }
		String CustomField51 { get; set; }
		String CustomField52 { get; set; }
		String CustomField53 { get; set; }
		String CustomField54 { get; set; }
		String CustomField55 { get; set; }
		String CustomField56 { get; set; }
		String CustomField57 { get; set; }
		String CustomField58 { get; set; }
		String CustomField59 { get; set; }
		String CustomField60 { get; set; }
	}
}
