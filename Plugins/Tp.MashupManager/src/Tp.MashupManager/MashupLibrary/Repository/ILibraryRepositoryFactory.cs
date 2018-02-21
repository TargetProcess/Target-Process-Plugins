// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.MashupManager.MashupLibrary.Repository.Config;

namespace Tp.MashupManager.MashupLibrary.Repository
{
    public interface ILibraryRepositoryFactory
    {
        ILibraryRepository GetRepository(ILibraryRepositoryConfig config);
    }
}
