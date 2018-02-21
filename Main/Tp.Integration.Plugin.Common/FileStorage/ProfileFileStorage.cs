// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.IO;
using StructureMap;
using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.FileStorage
{
    public class ProfileFileStorage : IProfileFileStorage
    {
        private const string CommonFolderName = "Common";
        private readonly string _folderName;

        public ProfileFileStorage(string accountName, string profileName)
        {
            _folderName = Path.Combine(ObjectFactory.GetInstance<PluginDataFolder>().Path, GetAccountNameOrCommon(accountName),
                profileName);
        }

        private static string GetAccountNameOrCommon(string accountName)
        {
            return accountName == AccountName.Empty ? CommonFolderName : accountName;
        }

        public string GetFolder()
        {
            if (!Directory.Exists(_folderName))
            {
                Directory.CreateDirectory(_folderName);
            }

            return _folderName;
        }

        public void Clear()
        {
            var folder = GetFolder();

            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
        }
    }
}
