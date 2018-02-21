// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.MashupManager.MashupLibrary.Package;

namespace Tp.MashupManager.MashupLibrary.Repository
{
    public interface ILibraryRepository
    {
        void Refresh();
        IEnumerable<LibraryPackage> GetPackages();
        Mashup GetPackageMashup(string packageName);
        LibraryPackageDetailed GetPackageDetailed(string packageName);
    }
}
