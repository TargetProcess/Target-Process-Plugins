// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using StructureMap;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.MashupManager
{
    public interface ISingleProfile
    {
        IProfile Profile { get; }
    }

    public class SingleProfile : ISingleProfile
    {
        private readonly IActivityLogger _logger;

        public SingleProfile(IActivityLogger logger)
        {
            _logger = logger;
        }

        public IProfile Profile
        {
            get
            {
                var profiles = ObjectFactory.GetInstance<IProfileCollection>();
                var count = profiles.Count();
                if (count == 0)
                {
                    return null;
                }

                if (count == 1)
                {
                    return profiles.Single();
                }

                _logger.Error("There are more than one profile for Mashup Manager plugin");

                return profiles.First();
            }
        }
    }
}
