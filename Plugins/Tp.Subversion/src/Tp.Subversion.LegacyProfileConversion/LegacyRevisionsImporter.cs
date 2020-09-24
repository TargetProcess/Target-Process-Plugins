// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Plugin.Common.Domain;
using Tp.LegacyProfileConvertsion.Common;

namespace Tp.Subversion.LegacyProfileConversion
{
    public class LegacyRevisionsImporter : LegacyProfileConvertor
    {
        public LegacyRevisionsImporter(IConvertorArgs args, IAccountCollection accountCollection)
            : base(args, accountCollection)
        {
        }

        protected override void ExecuteForProfile(PluginProfile legacyProfile, IAccount account)
        {
            var profile = GetProfile(account, legacyProfile);

            if (profile == null)
            {
                return;
            }

            MigrateRevisions(legacyProfile, profile);

            profile.Save();
        }

        private static IProfile GetProfile(IAccount account, PluginProfile legacyProfile)
        {
            var convertedProfileName = $"{legacyProfile.ProfileName} _re-converted_";

            return account.Profiles.FirstOrDefault(x => x.Name == convertedProfileName) ??
                account.Profiles.FirstOrDefault(x => x.Name == legacyProfile.ProfileName);
        }

        protected override IEnumerable<PluginProfile> GetLegacyProfiles()
        {
            return _context.PluginProfiles.Where(x => x.Active == true && x.PluginName == "Subversion Integration");
        }
    }
}
