// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;

namespace Tp.Testing.Common.NBehave
{
	/// <summary>
	/// Assembly which contains action steps should be marked with this attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class ActionStepsAssemblyAttribute : Attribute
	{
	}
}