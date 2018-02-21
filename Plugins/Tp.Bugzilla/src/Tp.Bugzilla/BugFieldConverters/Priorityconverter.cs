// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using Tp.BugTracking.BugFieldConverters;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;

namespace Tp.Bugzilla.BugFieldConverters
{
    public class PriorityConverter : GuessConverterBase<BugzillaBug, PriorityDTO>
    {
        public PriorityConverter(IStorageRepository storageRepository, IActivityLogger logger) : base(storageRepository, logger)
        {
        }

        protected override void SetValue(ConvertedBug dto, int id)
        {
            dto.BugDto.PriorityID = id;
        }

        protected override PriorityDTO GetFromStorage(string value)
        {
            return GetDtoStorage().SingleOrDefault(s => s.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase));
        }

        protected override string GetThirdPartyValue(BugzillaBug thirdPartyBug)
        {
            return thirdPartyBug.priority;
        }

        protected override MappingContainer Map
        {
            get { return StorageRepository.GetProfile<BugzillaProfile>().PrioritiesMapping; }
        }

        protected override BugField BugField
        {
            get { return BugField.PriorityID; }
        }

        protected override string BugFieldName
        {
            get { return "Priority"; }
        }
    }
}
