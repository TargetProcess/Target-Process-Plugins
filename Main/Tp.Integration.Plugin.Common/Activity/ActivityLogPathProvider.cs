// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Core;
using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Activity
{
    internal class ActivityLogPathProvider : IActivityLogPathProvider
    {
        public const string CommonLogFolderName = "common";

        public virtual string GetLogPathFor(string accountName, string profileName, string fileName)
        {
            return GetLogPathFor(accountName, profileName, fileName, 0);
        }

        public virtual string GetLogPathFor(string accountName, string profileName, string fileName, int chunk)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            var logFile = fileName;
            if (chunk > 0)
            {
                logFile = logFile + "." + chunk;
            }

            return GetProfileLogDirectoryFor(accountName, profileName)
                .Combine(logFile);
        }

        public string GetProfileLogDirectoryFor(string accountName, string profileName)
        {
            return GetAccountNameOrCommon(accountName)
                .Combine(profileName);
        }

        public virtual string GetFileNameFrom(string fullName)
        {
            return fullName.GetFileName();
        }

        public string GetAccountNameFrom(string fileName)
        {
            var result = string.Empty;
            var parent = fileName.GetDirectoryName();
            if (!string.IsNullOrEmpty(parent))
            {
                result = parent.GetDirectoryName();
                if (CommonLogFolderName.Equals(result))
                {
                    result = AccountName.Empty.Value;
                }
            }

            return result;
        }

        public string GetProfileNameFrom(string fileName)
        {
            return fileName.GetDirectoryName().GetFileName();
        }

        private static string GetAccountNameOrCommon(string accountName)
        {
            return accountName == AccountName.Empty ? CommonLogFolderName : accountName;
        }
    }
}
