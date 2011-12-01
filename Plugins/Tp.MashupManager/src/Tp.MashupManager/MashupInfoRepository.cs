// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using StructureMap;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.MashupManager
{
	public class MashupInfoRepository : IMashupInfoRepository
	{
		private readonly ITpBus _bus;
		private readonly IActivityLogger _logger;
		private readonly IPluginContext _context;
		public const string ProfileName = "Mashups";

		public MashupInfoRepository(IActivityLogger logger, IPluginContext context, ITpBus bus)
		{
			_bus = bus;
			_logger = logger;
			_context = context;
		}

		public PluginProfileErrorCollection Add(MashupDto dto)
		{
			var errors = new PluginProfileErrorCollection();

			ValidateNameNotEmpty(dto, errors);
			ValidateNameContainsOnlyValidChars(dto, errors);
			ValidateNameUniqueness(dto, errors);

			if (!errors.Any())
			{
				AddMashupNameToPlugin(dto);

				Profile.Get<MashupDto>(dto.Name).Add(dto);
				_bus.Send(dto.CreatePluginMashupMessage());

				_logger.InfoFormat("Add mashup commnad sent to TP (Mashup '{0}' for account '{1}')", dto.Name, _context.AccountName.Value);
			}
			return errors;
		}

		public PluginProfileErrorCollection Update(UpdateMashupDto dto)
		{
			if (dto.IsNameChanged())
			{
				var addErrors = Add(dto);
				if (!addErrors.Any())
				{
					var deleteErrors = Delete(dto.OldName);
					foreach (var error in deleteErrors)
					{
						addErrors.Add(error);
					}
				}

				return addErrors;
			}

			return UpdateInternal(dto);
		}

		public PluginProfileErrorCollection Delete(string name)
		{
			DeleteMashupNameToPlugin(name);

			Profile.Get<MashupDto>(name).Remove(m => m.Name == name);

			var dto = MashupDto.CreateEmptyMashup(name);

			_bus.Send(dto.CreatePluginMashupMessage());

			_logger.InfoFormat("Clean mashup commnad sent to TP (Mashup '{0}' for account '{1}')", dto.Name, _context.AccountName.Value);

			return new PluginProfileErrorCollection();
		}

		private IProfile Profile
		{
			get { return ObjectFactory.GetInstance<ISingleProfile>().Profile; }
		}

		private MashupManagerProfile ManagerProfile
		{
			get
			{
				return Profile != null
						? Profile.GetProfile<MashupManagerProfile>()
						: null;
			}
		}

		private void AddMashupNameToPlugin(MashupDto dto)
		{
			var names = ManagerProfile != null ? ManagerProfile.MashupNames : new MashupNames();
			names.Add(dto.Name);
			CreateOrUpdateProfile(names);
		}

		private void DeleteMashupNameToPlugin(string name)
		{
			var names = ManagerProfile.MashupNames;
			names.Remove(name);

			CreateOrUpdateProfile(names);
		}

		private PluginProfileErrorCollection UpdateInternal(UpdateMashupDto dto)
		{
			var errors = new PluginProfileErrorCollection();
			ValidateNameNotEmpty(dto, errors);
			ValidateNameContainsOnlyValidChars(dto, errors);

			if (!errors.Any())
			{
				Profile.Get<MashupDto>(dto.Name).ReplaceWith(dto);
				_bus.Send(dto.CreatePluginMashupMessage());

				_logger.InfoFormat("Update mashup commnad sent to TP (Mashup '{0}' for account '{1}')", dto.Name,
				                   _context.AccountName.Value);

				return new PluginProfileErrorCollection();
			}
			return errors;
		}

		private void ValidateNameNotEmpty(MashupDto dto, PluginProfileErrorCollection errors)
		{
			if (string.IsNullOrEmpty(dto.Name))
				errors.Add(new PluginProfileError
				           	{
				           		FieldName = MashupDto.NameField,
				           		Message = "Mashup with the same name already exists"
				           	});
		}

		private void ValidateNameContainsOnlyValidChars(MashupDto dto, PluginProfileErrorCollection errors)
		{
			if (!ProfileDtoValidator.IsValid(dto.Name))
				errors.Add(new PluginProfileError
				           	{
				           		FieldName = MashupDto.NameField,
				           		Message = "You can only use letters, numbers, space and underscore symbol in Mashup name"
				           	});
		}

		private void ValidateNameUniqueness(MashupDto dto, PluginProfileErrorCollection errors)
		{
			if (errors.Any())
				return;

			var existsSuchName = ManagerProfile != null && ManagerProfile.MashupNames
			                                               	.Any(
			                                               		m => m.Equals(dto.Name, StringComparison.InvariantCultureIgnoreCase));

			if (existsSuchName)
			{
				errors.Add(new PluginProfileError
				           	{
				           		FieldName = MashupDto.NameField,
				           		Message = "Mashup with the same name already exists"
				           	});
			}
		}

		private void CreateOrUpdateProfile(MashupNames names)
		{
			var command = ObjectFactory.GetInstance<AddOrUpdateProfileCommand>();

			var pluginProfileDto = new PluginProfileDto
			{
				Name = ProfileName,
				Settings = new MashupManagerProfile
				{
					MashupNames = names
				}
			};

			command.Execute(pluginProfileDto.Serialize());
		}
	}
}
