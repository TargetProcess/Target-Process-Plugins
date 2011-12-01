﻿//-----------------------------------------------------------------------------
// This code was generated by a tool.
// Changes to this file will be lost if the code is regenerated.
//-----------------------------------------------------------------------------
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Common
{
    /// <summary>
    /// Data Transfer object of Test Case Run. Represents specific run of test case against test plan.
    /// </summary>
	[Serializable]
	public partial class TestCaseRunDTO : DataTransferObject
	{
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>		
		[PrimaryKey]
		public override int? ID
		{
			get { return TestCaseRunID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				TestCaseRunID = value;
			}
		}

        /// <summary>
        /// Gets or sets the Test Case Run ID.
        /// </summary>
        /// <value>The Test Case Run ID.</value>
		[PrimaryKey]
		public int? TestCaseRunID { get; set; }
		

		/// <summary>
        /// Gets or sets the Run Date. Date when test case was run
        /// </summary>
        /// <value>The Run Date.</value>
		public DateTime? RunDate { get; set; }

		/// <summary>
        /// Gets or sets the Passed. Defines whether test case passed or failed. True - passed, False - failed
        /// </summary>
        /// <value>The Passed.</value>
		public Boolean? Passed { get; set; }

		/// <summary>
        /// Gets or sets the Runned. Defines whether test case was run. True - run, False - not run
        /// </summary>
        /// <value>The Runned.</value>
		public Boolean? Runned { get; set; }

		/// <summary>
        /// Gets or sets the Comment. Comment
        /// </summary>
        /// <value>The Comment.</value>
		public String Comment { get; set; }
		
		/// <summary>
        /// Gets or sets the Test Plan Run ID. Reference to test plan run
        /// </summary>
        /// <value>The Test Plan Run ID.</value>
		[ForeignKey]
		public Int32? TestPlanRunID { get; set; }
		
		/// <summary>
        /// Gets or sets the Test Case Test Plan ID. Reference to test plan and test case
        /// </summary>
        /// <value>The Test Case Test Plan ID.</value>
		[ForeignKey]
		public Int32? TestCaseTestPlanID { get; set; }
		

		
		/// <summary>
        /// Gets or sets the Test Plan Run Name. Reference to test plan run
        /// </summary>
        /// <value>The Test Plan Run Name.</value>
		[RelationName]
		public virtual string TestPlanRunName { get; set; }
		
	}
	
	
	/// <summary>
    /// Test Case Run fields
    /// </summary>
	public enum TestCaseRunField
	{
        /// <summary>
        /// Run Date
        /// </summary>		
		RunDate,
        /// <summary>
        /// Passed
        /// </summary>		
		Passed,
        /// <summary>
        /// Runned
        /// </summary>		
		Runned,
        /// <summary>
        /// Comment
        /// </summary>		
		Comment,
        /// <summary>
        /// Test Plan Run ID
        /// </summary>		
		TestPlanRunID,
        /// <summary>
        /// Test Case Test Plan ID
        /// </summary>		
		TestCaseTestPlanID,
        /// <summary>
        /// Test Plan Run Name
        /// </summary>		
		TestPlanRunName,
	}
}