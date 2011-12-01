﻿//-----------------------------------------------------------------------------
// This code was generated by a tool.
// Changes to this file will be lost if the code is regenerated.
//-----------------------------------------------------------------------------
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Common
{
    /// <summary>
    /// Data Transfer object of Request. Represents Request.
    /// </summary>
	[Serializable]
	public partial class RequestDTO : DataTransferObject
	{
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>		
		[PrimaryKey]
		public override int? ID
		{
			get { return RequestID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				RequestID = value;
			}
		}

        /// <summary>
        /// Gets or sets the Request ID.
        /// </summary>
        /// <value>The Request ID.</value>
		[PrimaryKey]
		public int? RequestID { get; set; }
		

		/// <summary>
        /// Gets or sets the Name. Entity name or title
        /// </summary>
        /// <value>The Name.</value>
		public String Name { get; set; }

		/// <summary>
        /// Gets or sets the Description. Entity description
        /// </summary>
        /// <value>The Description.</value>
		public String Description { get; set; }

		/// <summary>
        /// Gets or sets the Start Date. For example, start date of the iteration. Relevant for Iteration, Project, Release.
        /// </summary>
        /// <value>The Start Date.</value>
		public DateTime? StartDate { get; set; }

		/// <summary>
        /// Gets or sets the End Date. For example, end date of the iteration. Relevant for Iteration, Project, Release.
        /// </summary>
        /// <value>The End Date.</value>
		public DateTime? EndDate { get; set; }

		/// <summary>
        /// Gets or sets the Create Date. Date when entity has been created
        /// </summary>
        /// <value>The Create Date.</value>
		public DateTime? CreateDate { get; set; }

		/// <summary>
        /// Gets or sets the Modify Date. Date when entity has been modified
        /// </summary>
        /// <value>The Modify Date.</value>
		public DateTime? ModifyDate { get; set; }

		/// <summary>
        /// Gets or sets the Last Comment Date. Last comment date
        /// </summary>
        /// <value>The Last Comment Date.</value>
		public DateTime? LastCommentDate { get; set; }

		/// <summary>
        /// Gets or sets the Numeric Priority. Calculated priority of entity. Valid for UserStory and Bug for now
        /// </summary>
        /// <value>The Numeric Priority.</value>
		public Double? NumericPriority { get; set; }

		/// <summary>
        /// Gets or sets the Custom Field1. Reserved property for custom field
        /// </summary>
        /// <value>The Custom Field1.</value>
		public String CustomField1 { get; set; }

		/// <summary>
        /// Gets or sets the Custom Field2. Reserved property for custom field
        /// </summary>
        /// <value>The Custom Field2.</value>
		public String CustomField2 { get; set; }

		/// <summary>
        /// Gets or sets the Custom Field3. Reserved property for custom field
        /// </summary>
        /// <value>The Custom Field3.</value>
		public String CustomField3 { get; set; }

		/// <summary>
        /// Gets or sets the Custom Field4. Reserved property for custom field
        /// </summary>
        /// <value>The Custom Field4.</value>
		public String CustomField4 { get; set; }

		/// <summary>
        /// Gets or sets the Custom Field5. Reserved property for custom field
        /// </summary>
        /// <value>The Custom Field5.</value>
		public String CustomField5 { get; set; }

		/// <summary>
        /// Gets or sets the Custom Field6. Reserved property for custom field
        /// </summary>
        /// <value>The Custom Field6.</value>
		public String CustomField6 { get; set; }

		/// <summary>
        /// Gets or sets the Custom Field7. Reserved property for custom field
        /// </summary>
        /// <value>The Custom Field7.</value>
		public String CustomField7 { get; set; }

		/// <summary>
        /// Gets or sets the Custom Field8. Reserved property for custom field
        /// </summary>
        /// <value>The Custom Field8.</value>
		public String CustomField8 { get; set; }

		/// <summary>
        /// Gets or sets the Custom Field9. Reserved property for custom field
        /// </summary>
        /// <value>The Custom Field9.</value>
		public String CustomField9 { get; set; }

		/// <summary>
        /// Gets or sets the Custom Field10. Reserved property for custom field
        /// </summary>
        /// <value>The Custom Field10.</value>
		public String CustomField10 { get; set; }

		/// <summary>
        /// Gets or sets the Custom Field11. Reserved property for custom field
        /// </summary>
        /// <value>The Custom Field11.</value>
		public String CustomField11 { get; set; }

		/// <summary>
        /// Gets or sets the Custom Field12. Reserved property for custom field
        /// </summary>
        /// <value>The Custom Field12.</value>
		public String CustomField12 { get; set; }

		/// <summary>
        /// Gets or sets the Custom Field13. Reserved property for custom field
        /// </summary>
        /// <value>The Custom Field13.</value>
		public String CustomField13 { get; set; }

		/// <summary>
        /// Gets or sets the Custom Field14. Reserved property for custom field
        /// </summary>
        /// <value>The Custom Field14.</value>
		public String CustomField14 { get; set; }

		/// <summary>
        /// Gets or sets the Custom Field15. Reserved property for custom field
        /// </summary>
        /// <value>The Custom Field15.</value>
		public String CustomField15 { get; set; }

		/// <summary>
        /// Gets or sets the Effort. Total effort of assignable. Can be set if only one ActorEffort for assignable exists
        /// </summary>
        /// <value>The Effort.</value>
		public Decimal? Effort { get; set; }

		/// <summary>
        /// Gets or sets the Effort Completed. Effort spent on assignment. Read-only calculated field
        /// </summary>
        /// <value>The Effort Completed.</value>
		public Decimal? EffortCompleted { get; set; }

		/// <summary>
        /// Gets or sets the Effort To Do. Effort required to complete assignment. Read-only calculated field
        /// </summary>
        /// <value>The Effort To Do.</value>
		public Decimal? EffortToDo { get; set; }

		/// <summary>
        /// Gets or sets the Time Spent. Total time spent on assignment. Read-only calculated field
        /// </summary>
        /// <value>The Time Spent.</value>
		public Decimal? TimeSpent { get; set; }

		/// <summary>
        /// Gets or sets the Time Remain. Total time remaining to complete assignment for Role. Read-only calculated field
        /// </summary>
        /// <value>The Time Remain.</value>
		public Decimal? TimeRemain { get; set; }

		/// <summary>
        /// Gets or sets the Source Type. Source Type
        /// </summary>
        /// <value>The Source Type.</value>
		public RequestSourceEnum? SourceType { get; set; }

		/// <summary>
        /// Gets or sets the Is Private. visibility of request
        /// </summary>
        /// <value>The Is Private.</value>
		public Boolean? IsPrivate { get; set; }

		/// <summary>
        /// Gets or sets the Is Replied. Is Replied
        /// </summary>
        /// <value>The Is Replied.</value>
		public Boolean? IsReplied { get; set; }
		
		/// <summary>
        /// Gets or sets the Last Comment User ID. User who added last comment
        /// </summary>
        /// <value>The Last Comment User ID.</value>
		[ForeignKey]
		public Int32? LastCommentUserID { get; set; }
		
		/// <summary>
        /// Gets or sets the Owner ID. Person who added the entity
        /// </summary>
        /// <value>The Owner ID.</value>
		[ForeignKey]
		public Int32? OwnerID { get; set; }
		
		/// <summary>
        /// Gets or sets the Last Editor ID. Person who edited entity last time
        /// </summary>
        /// <value>The Last Editor ID.</value>
		[ForeignKey]
		public Int32? LastEditorID { get; set; }
		
		/// <summary>
        /// Gets or sets the Entity State ID. State of assignable. For example, User Story may be in Open or Done state
        /// </summary>
        /// <value>The Entity State ID.</value>
		[ForeignKey]
		public Int32? EntityStateID { get; set; }
		
		/// <summary>
        /// Gets or sets the Priority ID. Priority of assignable. For example, User Story may have Must Have or Nice To Have priority
        /// </summary>
        /// <value>The Priority ID.</value>
		[ForeignKey]
		public Int32? PriorityID { get; set; }
		
		/// <summary>
        /// Gets or sets the Project ID. Assignable should belong to Project
        /// </summary>
        /// <value>The Project ID.</value>
		[ForeignKey]
		public Int32? ProjectID { get; set; }
		
		/// <summary>
        /// Gets or sets the Iteration ID. Assignable may be assigned to Iteration or may be in Backlog (Iteration is not defined in this case)
        /// </summary>
        /// <value>The Iteration ID.</value>
		[ForeignKey]
		public Int32? IterationID { get; set; }
		
		/// <summary>
        /// Gets or sets the Parent ID. Used for Task only (Task parent is a User Story)
        /// </summary>
        /// <value>The Parent ID.</value>
		[ForeignKey]
		public Int32? ParentID { get; set; }
		
		/// <summary>
        /// Gets or sets the Release ID. Assignable may be assigned to Release or may be in project Backlog (Release is not defined in this case)
        /// </summary>
        /// <value>The Release ID.</value>
		[ForeignKey]
		public Int32? ReleaseID { get; set; }
		
		/// <summary>
        /// Gets or sets the Request Type ID. Request Type
        /// </summary>
        /// <value>The Request Type ID.</value>
		[ForeignKey]
		public Int32? RequestTypeID { get; set; }
		

		
		/// <summary>
        /// Gets or sets the Entity Type Name. Type of the entity. For example, Bug, TestCase, Task
        /// </summary>
        /// <value>The Entity Type Name.</value>
		[RelationName]
		public virtual string EntityTypeName { get; set; }
		
		/// <summary>
        /// Gets or sets the Entity State Name. State of assignable. For example, User Story may be in Open or Done state
        /// </summary>
        /// <value>The Entity State Name.</value>
		[RelationName]
		public virtual string EntityStateName { get; set; }
		
		/// <summary>
        /// Gets or sets the Priority Name. Priority of assignable. For example, User Story may have Must Have or Nice To Have priority
        /// </summary>
        /// <value>The Priority Name.</value>
		[RelationName]
		public virtual string PriorityName { get; set; }
		
		/// <summary>
        /// Gets or sets the Project Name. Assignable should belong to Project
        /// </summary>
        /// <value>The Project Name.</value>
		[RelationName]
		public virtual string ProjectName { get; set; }
		
		/// <summary>
        /// Gets or sets the Iteration Name. Assignable may be assigned to Iteration or may be in Backlog (Iteration is not defined in this case)
        /// </summary>
        /// <value>The Iteration Name.</value>
		[RelationName]
		public virtual string IterationName { get; set; }
		
		/// <summary>
        /// Gets or sets the Parent Name. Used for Task only (Task parent is a User Story)
        /// </summary>
        /// <value>The Parent Name.</value>
		[RelationName]
		public virtual string ParentName { get; set; }
		
		/// <summary>
        /// Gets or sets the Release Name. Assignable may be assigned to Release or may be in project Backlog (Release is not defined in this case)
        /// </summary>
        /// <value>The Release Name.</value>
		[RelationName]
		public virtual string ReleaseName { get; set; }
		
		/// <summary>
        /// Gets or sets the Request Type Name. Request Type
        /// </summary>
        /// <value>The Request Type Name.</value>
		[RelationName]
		public virtual string RequestTypeName { get; set; }
		
	}
	
	
	/// <summary>
    /// Request fields
    /// </summary>
	public enum RequestField
	{
        /// <summary>
        /// Name
        /// </summary>		
		Name,
        /// <summary>
        /// Description
        /// </summary>		
		Description,
        /// <summary>
        /// Start Date
        /// </summary>		
		StartDate,
        /// <summary>
        /// End Date
        /// </summary>		
		EndDate,
        /// <summary>
        /// Create Date
        /// </summary>		
		CreateDate,
        /// <summary>
        /// Modify Date
        /// </summary>		
		ModifyDate,
        /// <summary>
        /// Last Comment Date
        /// </summary>		
		LastCommentDate,
        /// <summary>
        /// Numeric Priority
        /// </summary>		
		NumericPriority,
        /// <summary>
        /// Custom Field1
        /// </summary>		
		CustomField1,
        /// <summary>
        /// Custom Field2
        /// </summary>		
		CustomField2,
        /// <summary>
        /// Custom Field3
        /// </summary>		
		CustomField3,
        /// <summary>
        /// Custom Field4
        /// </summary>		
		CustomField4,
        /// <summary>
        /// Custom Field5
        /// </summary>		
		CustomField5,
        /// <summary>
        /// Custom Field6
        /// </summary>		
		CustomField6,
        /// <summary>
        /// Custom Field7
        /// </summary>		
		CustomField7,
        /// <summary>
        /// Custom Field8
        /// </summary>		
		CustomField8,
        /// <summary>
        /// Custom Field9
        /// </summary>		
		CustomField9,
        /// <summary>
        /// Custom Field10
        /// </summary>		
		CustomField10,
        /// <summary>
        /// Custom Field11
        /// </summary>		
		CustomField11,
        /// <summary>
        /// Custom Field12
        /// </summary>		
		CustomField12,
        /// <summary>
        /// Custom Field13
        /// </summary>		
		CustomField13,
        /// <summary>
        /// Custom Field14
        /// </summary>		
		CustomField14,
        /// <summary>
        /// Custom Field15
        /// </summary>		
		CustomField15,
        /// <summary>
        /// Effort
        /// </summary>		
		Effort,
        /// <summary>
        /// Effort Completed
        /// </summary>		
		EffortCompleted,
        /// <summary>
        /// Effort To Do
        /// </summary>		
		EffortToDo,
        /// <summary>
        /// Time Spent
        /// </summary>		
		TimeSpent,
        /// <summary>
        /// Time Remain
        /// </summary>		
		TimeRemain,
        /// <summary>
        /// Source Type
        /// </summary>		
		SourceType,
        /// <summary>
        /// Is Private
        /// </summary>		
		IsPrivate,
        /// <summary>
        /// Is Replied
        /// </summary>		
		IsReplied,
        /// <summary>
        /// Last Comment User ID
        /// </summary>		
		LastCommentUserID,
        /// <summary>
        /// Owner ID
        /// </summary>		
		OwnerID,
        /// <summary>
        /// Last Editor ID
        /// </summary>		
		LastEditorID,
        /// <summary>
        /// Entity State ID
        /// </summary>		
		EntityStateID,
        /// <summary>
        /// Priority ID
        /// </summary>		
		PriorityID,
        /// <summary>
        /// Project ID
        /// </summary>		
		ProjectID,
        /// <summary>
        /// Iteration ID
        /// </summary>		
		IterationID,
        /// <summary>
        /// Parent ID
        /// </summary>		
		ParentID,
        /// <summary>
        /// Release ID
        /// </summary>		
		ReleaseID,
        /// <summary>
        /// Request Type ID
        /// </summary>		
		RequestTypeID,
        /// <summary>
        /// Entity Type Name
        /// </summary>		
		EntityTypeName,
        /// <summary>
        /// Entity State Name
        /// </summary>		
		EntityStateName,
        /// <summary>
        /// Priority Name
        /// </summary>		
		PriorityName,
        /// <summary>
        /// Project Name
        /// </summary>		
		ProjectName,
        /// <summary>
        /// Iteration Name
        /// </summary>		
		IterationName,
        /// <summary>
        /// Parent Name
        /// </summary>		
		ParentName,
        /// <summary>
        /// Release Name
        /// </summary>		
		ReleaseName,
        /// <summary>
        /// Request Type Name
        /// </summary>		
		RequestTypeName,
	}
}