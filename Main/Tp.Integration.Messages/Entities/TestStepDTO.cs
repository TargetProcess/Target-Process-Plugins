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
	///     Data Transfer object of Test Step. Represents specific test step of test case.
	/// </summary>
	[Serializable]
	[DataContract]
	public partial class TestStepDTO : DataTransferObject
	{
		/// <summary>
		///     Gets or sets the ID.
		/// </summary>
		/// <value>The ID.</value>
		[PrimaryKey]
		public override int? ID
		{
			get { return TestStepID; }
			set
			{
				if (value == int.MinValue)
					value = null;

				TestStepID = value;
			}
		}

		/// <summary>
		///     Gets or sets the Test Step  ID.
		/// </summary>
		/// <value>The Test Step  ID.</value>
		[PrimaryKey]
		[DataMember]
		[XmlElement(Order = 3)]
		public int? TestStepID { get; set; }

		/// <summary>
		///     Gets or sets the Description. Defines actions which should be performed at current Test Step
		/// </summary>
		/// <value>The Passed.</value>
		[DataMember]
		[XmlElement(Order = 4)]
		public string Description { get; set; }

		/// <summary>
		///     Gets or sets the Result. Defines expected results
		/// </summary>
		/// <value>The Runned.</value>
		[DataMember]
		[XmlElement(Order = 5)]
		public string Result { get; set; }

		/// <summary>
		///     Gets or sets the Run Order. Defines the order of Test Step Runs
		/// </summary>
		/// <value>The Run Order.</value>
		[DataMember]
		[XmlElement(Order = 6)]
		public int? RunOrder { get; set; }

		/// <summary>
		///     Gets or sets the Test Case  ID. Reference to test case
		/// </summary>
		/// <value>The Test Plan Run ID.</value>
		[ForeignKey]
		[DataMember]
		[XmlElement(Order = 8)]
		public Int32? TestCaseID { get; set; }
	}


	/// <summary>
	///     Test Step Run fields
	/// </summary>
	public enum TestStepField
	{
		/// <summary>
		///     Description
		/// </summary>
		Description,

		/// <summary>
		///     Result
		/// </summary>
		Result,

		/// <summary>
		///     Run Order
		/// </summary>
		RunOrder,

		/// <summary>
		///     Test Case ID
		/// </summary>
		TestCaseID,
	}
}
