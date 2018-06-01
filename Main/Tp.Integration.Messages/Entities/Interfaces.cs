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
        String CustomField61 { get; set; }
        String CustomField62 { get; set; }
        String CustomField63 { get; set; }
        String CustomField64 { get; set; }
        String CustomField65 { get; set; }
        String CustomField66 { get; set; }
        String CustomField67 { get; set; }
        String CustomField68 { get; set; }
        String CustomField69 { get; set; }
        String CustomField70 { get; set; }
        String CustomField71 { get; set; }
        String CustomField72 { get; set; }
        String CustomField73 { get; set; }
        String CustomField74 { get; set; }
        String CustomField75 { get; set; }
        String CustomField76 { get; set; }
        String CustomField77 { get; set; }
        String CustomField78 { get; set; }
        String CustomField79 { get; set; }
        String CustomField80 { get; set; }
        String CustomField81 { get; set; }
        String CustomField82 { get; set; }
        String CustomField83 { get; set; }
        String CustomField84 { get; set; }
        String CustomField85 { get; set; }
        String CustomField86 { get; set; }
        String CustomField87 { get; set; }
        String CustomField88 { get; set; }
        String CustomField89 { get; set; }
        String CustomField90 { get; set; }
        String CustomField91 { get; set; }
        String CustomField92 { get; set; }
        String CustomField93 { get; set; }
        String CustomField94 { get; set; }
        String CustomField95 { get; set; }
        String CustomField96 { get; set; }
        String CustomField97 { get; set; }
        String CustomField98 { get; set; }
        String CustomField99 { get; set; }
        String CustomField100 { get; set; }
    }
}
