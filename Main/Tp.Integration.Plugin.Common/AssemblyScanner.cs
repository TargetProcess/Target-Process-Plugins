// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common
{
    public class AssemblyScanner : IPluginMetadata
    {
        private readonly IAssembliesHost _assembliesHost;
        private readonly Type _profileType;
        private readonly PluginData _pluginAssemblyAttribute;

        public AssemblyScanner(IAssembliesHost assembliesHost)
        {
            _assembliesHost = assembliesHost;
            _profileType = GetProfileTypeByAssemblyScanning();
            _pluginAssemblyAttribute = FindPluginAssemblyAttributeByAssemblyScanning();
            IsNewProfileInitializable = GetIsNewProfileInitializableByAssemblyScanning();
            IsUpdatedProfileInitializable = GetIsUpdatedProfileInitializableByAssemblyScanning();
        }

        #region ProfileType

        private Type GetProfileTypeByAssemblyScanning()
        {
            return GetTypesWith<ProfileAttribute>(true).FirstOrDefault() ?? typeof(object);
        }

        private IEnumerable<Type> GetTypesWith<TAttribute>(bool inherit)
            where TAttribute : Attribute
        {
            return from type in GetLoadedTypes()
                where type.IsDefined(typeof(TAttribute), inherit)
                select type;
        }

        private IEnumerable<Type> GetLoadedTypes()
        {
            return from a in _assembliesHost.GetAssemblies()
                from t in a.GetTypes()
                select t;
        }

        public Type ProfileType
        {
            get { return _profileType; }
        }

        #endregion

        #region Plugin Assembly Info

        public PluginData PluginData
        {
            get { return _pluginAssemblyAttribute; }
        }

        public bool IsSyncronizableProfile
        {
            get
            {
                var profileType = _profileType;
                return (profileType != null && profileType.GetInterfaces().Any(x => x == typeof(ISynchronizableProfile)));
            }
        }

        public bool IsNewProfileInitializable { get; private set; }

        public bool IsUpdatedProfileInitializable { get; private set; }

        private bool GetIsNewProfileInitializableByAssemblyScanning()
        {
            return FindIsDerivedFromType(typeof(NewProfileInitializationSaga<>));
        }

        private bool GetIsUpdatedProfileInitializableByAssemblyScanning()
        {
            return FindIsDerivedFromType(typeof(UpdatedProfileInitializationSaga<>));
        }

        private bool FindIsDerivedFromType(Type baseType)
        {
            var types = GetLoadedTypes();

            var result = from type in types
                where type.Assembly != baseType.Assembly && IsTypeDerivedFromGenericType(type, baseType)
                select type;

            return !result.Empty();
        }

        public bool IsTypeDerivedFromGenericType(Type typeToCheck, Type genericType)
        {
            if (typeToCheck == typeof(object))
            {
                return false;
            }

            if (typeToCheck == null)
            {
                return false;
            }

            if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            return IsTypeDerivedFromGenericType(typeToCheck.BaseType, genericType);
        }

        private PluginData FindPluginAssemblyAttributeByAssemblyScanning()
        {
            var pluginAssembly = _assembliesHost.GetAssemblies().FirstOrDefault(x => x.IsDefined(typeof(PluginAssemblyAttribute), false));

            var pluginData = pluginAssembly.GetCustomAttributes(false).OfType<PluginAssemblyAttribute>().FirstOrDefault().GetData();

            return pluginData;
        }

        #endregion
    }
}
