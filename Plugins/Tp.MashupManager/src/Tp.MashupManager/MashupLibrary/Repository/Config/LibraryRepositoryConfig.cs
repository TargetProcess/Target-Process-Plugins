// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.MashupManager.MashupLibrary.Repository.Config
{
    public struct LibraryRepositoryConfig : ILibraryRepositoryConfig
    {
        public string Name { get; set; }
        public string Uri { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
