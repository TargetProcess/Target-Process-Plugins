// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Integration.Plugin.Common.Validation
{
	/// <summary>
	/// Inherit your profile from this interface to implement validation logic.
	/// </summary>
	public interface IValidatable
	{
		/// <summary>
		/// Validates profile and accumulate errors.
		/// </summary>
		/// <param name="errors">Validation errors. Add your errors to this collection and they will be returned with response to profile add\update commands.</param>
		void Validate(PluginProfileErrorCollection errors);
	}
}