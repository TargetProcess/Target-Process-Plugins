// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus.Unicast.Transport;
using Newtonsoft.Json;
using Tp.BugTracking.Commands.Dtos;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.ServiceBus;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.Synchronization.Mapping
{
    public abstract class MappingTestBase<T> : BugzillaTestBase
    {
        protected abstract MappingSourceEntry Source { get; }
        protected abstract MappingContainer Mapping { get; }
        protected abstract List<T> StoredEntities { get; }
        protected abstract MappingContainer GetFromMappings(Mappings mappings);
        protected abstract Func<int, string, T> CreateEntityForTargetProcess { get; }

        protected void CheckMappingResultBase(string key, string value)
        {
            var tpValue = JsonConvert.DeserializeObject<MappingLookup>(value);
            var commandResult = Context.CommandResponse.ResponseData.Deserialize<Mappings>();

            GetFromMappings(commandResult).Where(x => x.Key == key && LookupsEquals(x.Value, tpValue)).Count().Should(
                Be.EqualTo(1),
                "GetFromMappings(commandResult).Where(x => x.Key == key && LookupsEquals(x.Value, tpValue)).Count().Should(Be.EqualTo(1))");
        }

        protected void MapBase()
        {
            HandlePluginCommand("GetMapping", Context.MappingSource.Serialize());
            Context.CommandResponse = TransportMock.TpQueue.GetMessages<PluginCommandResponseMessage>().Single();
        }

        protected void CheckMappingsItemsCountBase()
        {
            var mappingContainer = Context.CommandResponse.ResponseData.Deserialize<Mappings>();
            Source.ThirdPartyItems.Count.Should(Be.EqualTo(GetFromMappings(mappingContainer).Count),
                "Source.ThirdPartyItems.Count.Should(Be.EqualTo(GetFromMappings(mappingContainer).Count))");
        }

        private bool LookupsEquals(MappingLookup left, MappingLookup right)
        {
            if (left == null)
            {
                return right == null;
            }

            return left.Equals(right);
        }

        private void HandlePluginCommand(string commandName, string args)
        {
            TransportMock.TpQueue.Clear();

            var command = new ExecutePluginCommandCommand { CommandName = commandName, Arguments = args };

            TransportMock.HandleMessageFromTp(
                new List<HeaderInfo>
                {
                    new HeaderInfo { Key = BusExtensions.ACCOUNTNAME_KEY, Value = AccountName.Empty.Value },
                    new HeaderInfo { Key = BusExtensions.PROFILENAME_KEY, Value = Profile.Name.Value }
                },
                command);
        }

        protected void CreateMappingForProfileBase(string key, string value)
        {
            Mapping.Add(new MappingElement
            {
                Key = key,
                Value =
                    JsonConvert.DeserializeObject<MappingLookup>(value)
            });
        }

        protected void CreateInBugzillaBase(string name)
        {
            Source.ThirdPartyItems.Add(name);
        }

        protected void CreateEntityInTargetProcessBase(int id, string name)
        {
            Source.TpItems.Add(new MappingLookup { Id = id, Name = name });
            StoredEntities.Add(CreateEntityForTargetProcess(id, name));
        }
    }
}
