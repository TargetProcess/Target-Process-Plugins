using System;
using System.Xml.Serialization;

namespace Tp.Integration.Common
{ // ReSharper disable InconsistentNaming

    public partial interface IGeneralListItemDTO : IGeneralNumericPriorityListItemDTO
    {
    }

    public partial interface IGeneralDTO : IGeneralListItemDTO, IGeneralFieldExtensionDTO
    {
    }

    public partial class EmailAttachmentDTO : IEmailAttachmentDTO
    {
    }


    public partial class FeatureDTO
    {
        string IAssignableDTO.CommentOnChangingState
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }

    public partial class EpicDTO
    {
        string IAssignableDTO.CommentOnChangingState
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }


    public partial class ProgramDTO
    {
        [XmlIgnore]
        int? IGeneralNumericPriorityListItemDTO.EntityTypeID
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }

    public partial class ProjectDTO
    {
        [XmlIgnore]
        int? IGeneralNumericPriorityListItemDTO.EntityTypeID
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }

    public partial class TestPlanDTO
    {
        [XmlIgnore]
        string IAssignableDTO.CommentOnChangingState
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [XmlIgnore]
        int? IAssignableDTO.PriorityID
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [XmlIgnore]
        string IAssignableDTO.PriorityName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [XmlIgnore]
        decimal? IAssignableDTO.Progress
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }

    public partial class TermDTO
    {
        public TermDTO Clone()
        {
            return (TermDTO) MemberwiseClone();
        }
    }

    // ReSharper restore InconsistentNaming
}
