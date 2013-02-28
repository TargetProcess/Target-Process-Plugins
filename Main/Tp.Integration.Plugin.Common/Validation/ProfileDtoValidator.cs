// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Text.RegularExpressions;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.Integration.Plugin.Common.Validation
{
	/// <summary>
	/// Performs base validation and custom validation logic.
	/// </summary>
	public class ProfileDtoValidator : IValidatable
	{
		public const string ALLOWED_PROFILE_NAME_CHARS = "^[ _0-9a-zA-Z-]*$";
		private readonly PluginProfileDto _dto;

		public ProfileDtoValidator(PluginProfileDto dto)
		{
			_dto = dto;
		}

		public void Validate(PluginProfileErrorCollection errors)
		{
			ValidateNameIsNotEmpty(errors);

			ValidateNameHasValidCharacters(errors);

			ValidateSettings(errors);
		}

		private void ValidateSettings(PluginProfileErrorCollection errors)
		{
			var settings = _dto.Settings as IValidatable;
			if (settings != null)
			{
				settings.Validate(errors);
			}
		}

		private void ValidateNameIsNotEmpty(PluginProfileErrorCollection errors)
		{
			if (_dto.Name.IsNullOrWhitespace())
			{
				errors.Add(new PluginProfileError {FieldName = "Name", Message = "Profile Name is required"});
			}
		}

		private void ValidateNameHasValidCharacters(PluginProfileErrorCollection errors)
		{
			if (!IsValid(_dto.Name))
			{
				errors.Add(new PluginProfileError
				           	{
				           		FieldName = "Name",
				           		Message = "You can only use letters, numbers, space, dash and underscore symbol in Profile Name"
				           	});
			}
		}

		public static bool IsValid(string input)
		{
			var regex = new Regex(ALLOWED_PROFILE_NAME_CHARS);

			return regex.IsMatch(input);
		}
	}
}