// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.PluginCommand.Embedded
{
    internal interface IPluginProfileDtoSerializer
    {
        string Serialize(PluginProfileDto profile);
        string Serialize(PluginProfileDto[] profile);
        PluginProfileDto DeserializeProfile(string serializedPluginProfile);
    }

    internal class PluginProfileDtoSerializer<T> : IPluginProfileDtoSerializer where T : class, new()
    {
        public PluginProfileDto DeserializeProfile(string serializedPluginProfile)
        {
            return serializedPluginProfile.Deserialize<PluginProfileTypedDto<T>>(new[] { typeof(T) });
        }

        string IPluginProfileDtoSerializer.Serialize(PluginProfileDto profile)
        {
            return ((PluginProfileTypedDto<T>) profile).Serialize(new[] { typeof(T) });
        }

        string IPluginProfileDtoSerializer.Serialize(PluginProfileDto[] profile)
        {
            return profile.Cast<PluginProfileTypedDto<T>>().ToArray().Serialize(new[] { typeof(T) });
        }
    }

    public static class PluginProfileDtoSerializationExtensions
    {
        public static string Serialize(this PluginProfileDto pluginProfileDto)
        {
            return CreateProfileSerializer().Serialize(CreateTypedProfile(pluginProfileDto));
        }

        public static string Serialize(this PluginProfileDto[] pluginProfileDtos)
        {
            var typedProfileDtos = pluginProfileDtos.Select(CreateTypedProfile);
            return CreateProfileSerializer().Serialize(typedProfileDtos.ToArray());
        }

        private static IPluginProfileDtoSerializer CreateProfileSerializer()
        {
            var makeGenericType =
                typeof(PluginProfileDtoSerializer<>).MakeGenericType(SettingsType);
            return (IPluginProfileDtoSerializer) Activator.CreateInstance(makeGenericType);
        }

        private static PluginProfileDto CreateTypedProfile(PluginProfileDto pluginProfileDto)
        {
            var profileGenericType = typeof(PluginProfileTypedDto<>).MakeGenericType(SettingsType);
            var typedProfile = (PluginProfileDto) Activator.CreateInstance(profileGenericType);
            typedProfile.Settings = pluginProfileDto.Settings;
            typedProfile.Name = pluginProfileDto.Name;
            return typedProfile;
        }

        private static Type SettingsType
        {
            get { return ObjectFactory.GetInstance<IPluginMetadata>().ProfileType; }
        }

        public static PluginProfileDto DeserializeProfile(this string serializedPluginProfile)
        {
            return CreateProfileSerializer().DeserializeProfile(serializedPluginProfile);
        }
    }
}
