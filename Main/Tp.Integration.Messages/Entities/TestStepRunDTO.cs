// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Tp.Integration.Common
{
	/// <summary>
	///     Data Transfer object of Test Step Run. Represents specific run of test case against test plan.
	/// </summary>
	[Serializable]
	[DataContract]
	public partial class TestStepRunDTO : DataTransferObject
	{
		/// <summary>
		///     Gets or sets the ID.
		/// </summary>
		/// <value>The ID.</value>
		[PrimaryKey]
		public override int? ID
		{
			get { return TestStepRunID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				TestStepRunID = value;
			}
		}

		/// <summary>
		///     Gets or sets the Test Step Run ID.
		/// </summary>
		/// <value>The Test Step Run ID.</value>
		[PrimaryKey]
		[DataMember]
		[XmlElement(Order = 3)]
		public int? TestStepRunID { get; set; }

		/// <summary>
		///     Gets or sets the Passed. Defines whether test case passed or failed. True - passed, False - failed
		/// </summary>
		/// <value>The Passed.</value>
		[DataMember]
		[XmlElement(Order = 4)]
		public Boolean? Passed { get; set; }

		/// <summary>
		///     Gets or sets the Runned. Defines whether test case was run. True - run, False - not run
		/// </summary>
		/// <value>The Runned.</value>
		[DataMember]
		[XmlElement(Order = 5)]
		public Boolean? Runned { get; set; }

		/// <summary>
		///     Gets or sets the Run Order. Defines the order of Test Step Runs
		/// </summary>
		/// <value>The Run Order.</value>
		[DataMember]
		[XmlElement(Order = 6)]
		public int? RunOrder { get; set; }

		/// <summary>
		///     Gets or sets the Test Case Run ID. Reference to test case run
		/// </summary>
		/// <value>The Test Plan Run ID.</value>
		[ForeignKey]
		[DataMember]
		[XmlElement(Order = 8)]
		public Int32? TestCaseRunID { get; set; }

		/// <summary>
		///     Gets or sets the Test Step ID. Reference to test step
		/// </summary>
		/// <value>The Test Case Test Plan ID.</value>
		[ForeignKey]
		[DataMember]
		[XmlElement(Order = 9)]
		public Int32? TestStepID { get; set; }
	}


	/// <summary>
	///     Test Step Run fields
	/// </summary>
	public enum TestStepRunField
	{
		/// <summary>
		///     Passed
		/// </summary>
		Passed,

		/// <summary>
		///     Runned
		/// </summary>
		Runned,

		/// <summary>
		///     Run Order
		/// </summary>
		RunOrder,

		/// <summary>
		///     Test Case Run ID
		/// </summary>
		TestCaseRunID,

		/// <summary>
		///     Test Step ID
		/// </summary>
		TestStepID
	}
}
