// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Tp.BugTracking.Commands.Dtos;
using Tp.BugTracking.Mappers;
using Tp.BugTracking.Settings;
using Tp.Integration.Common;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.Bugzilla.CustomCommand
{
    [DataContract]
    public class BugzillaMappingSource : MappingSource
    {
        [DataMember]
        public MappingSourceEntry CustomFields { get; set; }
    }

    [DataContract]
    public class BugzillaMappings : Mappings
    {
        public BugzillaMappings(Mappings mappings)
        {
            States = mappings.States;
            Severities = mappings.Severities;
            Priorities = mappings.Priorities;
        }

        [DataMember]
        public MappingContainer CustomFields { get; set; }
    }

    [DataContract]
    public class CustomMappingLookup : MappingLookup
    {
        public CustomMappingLookup(string name)
        {
            Name = name;
            Id = GenerateHashCode();
        }

        private int GenerateHashCode()
        {
            char[] chPtr = Name.ToCharArray();
            int num = 0x1505;
            int num2 = num;
            for (int i = 0; i < Name.Length; i += 2)
            {
                num = ((num << 5) + num) ^ chPtr[i];
                if (i == Name.Length - 1)
                {
                    break;
                }
                num2 = ((num2 << 5) + num2) ^ chPtr[i + 1];
            }
            return (num + (num2 * 0x5d588b65));
        }
    }

    public class BugzillaThirdPartyFieldsMapper : ThirdPartyFieldsMapper
    {
        public BugzillaThirdPartyFieldsMapper(IStorageRepository storageRepository) : base(storageRepository)
        {
        }

        public override Mappings Map(MappingSource source)
        {
            var bugzillaMappingSource = source as BugzillaMappingSource;
            return new BugzillaMappings(base.Map(source))
            {
                CustomFields = GetMappingFor(bugzillaMappingSource?.CustomFields, GetMappedCustomMapping())
            };
        }

        private IEnumerable<MappingElement> GetMappedCustomMapping()
        {
            var profile = StorageRepository.IsNull ? null : StorageRepository.GetProfile<IBugTrackingMappingSource>() as BugzillaProfile;
            return profile?.CustomMapping ?? Enumerable.Empty<MappingElement>();
        }
    }

    public class GetMappingCommand : IPluginCommand
    {
        private readonly IThirdPartyFieldsMapper _mapper;

        public GetMappingCommand(IThirdPartyFieldsMapper mapper)
        {
            _mapper = mapper;
        }

        public PluginCommandResponseMessage Execute(string args, UserDTO user)
        {
            var source = args.Deserialize<BugzillaMappingSource>();

            return new PluginCommandResponseMessage
            {
                PluginCommandStatus = PluginCommandStatus.Succeed,
                ResponseData = _mapper.Map(source).Serialize()
            };
        }

        public string Name => "GetMapping";
    }
}
