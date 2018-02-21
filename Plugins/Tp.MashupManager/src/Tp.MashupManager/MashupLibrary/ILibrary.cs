// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.MashupManager.CustomCommands.Args;
using Tp.MashupManager.CustomCommands.Dtos;

namespace Tp.MashupManager.MashupLibrary
{
    public interface ILibrary
    {
        void Refresh();
        IEnumerable<LibraryRepositoryDto> GetRepositories();
        PackageDetailedDto GetPackageDetailed(PackageCommandArg commandArg);
    }
}
