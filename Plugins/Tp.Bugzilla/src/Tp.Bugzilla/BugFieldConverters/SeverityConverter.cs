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
    public class SeverityConverter : GuessConverterBase<BugzillaBug, SeverityDTO>
    {
        public SeverityConverter(IStorageRepository storageRepository, IActivityLogger logger) : base(storageRepository, logger)
        {
        }

        protected override void SetValue(ConvertedBug dto, int id)
        {
            dto.BugDto.SeverityID = id;
        }

        protected override SeverityDTO GetFromStorage(string value)
        {
            return GetDtoStorage().SingleOrDefault(s => s.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase));
        }

        protected override string GetThirdPartyValue(BugzillaBug thirdPartyBug)
        {
            return thirdPartyBug.bug_severity;
        }

        protected override MappingContainer Map
        {
            get { return StorageRepository.GetProfile<BugzillaProfile>().SeveritiesMapping; }
        }

        protected override BugField BugField
        {
            get { return BugField.SeverityID; }
        }

        protected override string BugFieldName
        {
            get { return "Severity"; }
        }
    }
}
