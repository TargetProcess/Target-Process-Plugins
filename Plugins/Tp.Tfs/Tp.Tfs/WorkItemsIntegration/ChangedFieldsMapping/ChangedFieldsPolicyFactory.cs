// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Tp.Integration.Common;
using Tp.Tfs.WorkItemsIntegration.ChangedFieldsMapping;

namespace Tp.Tfs.WorkItemsIntegration.ChangedFieldsPolicy
{
    public class ChangedFieldsPolicyFactory
    {
        private static ChangedFieldsPolicyFactory _factory;
        private static readonly object LockObject = new object();

        private ChangedFieldsPolicyFactory()
        {
        }

        public static IChangedFieldsMappingPolicy CreatePolicy(DataTransferObject objectDTO)
        {
            lock (LockObject)
            {
                if (_factory == null)
                    _factory = new ChangedFieldsPolicyFactory();
            }

            if (objectDTO is UserStoryDTO)
                return new UserStoryChangedFieldsPolicy();

            if (objectDTO is BugDTO)
                return new UserStoryChangedFieldsPolicy();

            if (objectDTO is FeatureDTO)
                return new UserStoryChangedFieldsPolicy();

            if (objectDTO is RequestDTO)
                return new UserStoryChangedFieldsPolicy();

            throw new ArgumentException("Wrong DTO type", objectDTO.GetType().FullName);
        }

        public static IChangedFieldsMappingPolicy CreatePolicy(string typeName)
        {
            lock (LockObject)
            {
                if (_factory == null)
                    _factory = new ChangedFieldsPolicyFactory();
            }

            switch (typeName)
            {
                case Constants.Bug:
                    return new BugChangedFieldsPolicy();
                case Constants.UserStory:
                    return new UserStoryChangedFieldsPolicy();
                case Constants.Feature:
                    return new FeatureChangedFieldsPolicy();
                case Constants.Request:
                    return new RequestChangedFieldsPolicy();
            }

            throw new ArgumentException("Wrong DTO type name", typeName);
        }
    }
}
