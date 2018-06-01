// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using System.Runtime.Serialization;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.PluginCommand.Embedded
{
    /// <summary>
    /// Properties marked with this attribute will not be serialized to client.
    /// Works only with properties which type is a reference type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SecretMemberAttribute : Attribute
    {
    }

    internal interface IPluginProfileDtoSerializer
    {
        string Serialize(PluginProfileDto profile);
        string SerializeForClient(PluginProfileDto profile);
        string SerializeForClient(PluginProfileDto[] profile);
        PluginProfileDto DeserializeProfileFromClient(string serializedPluginProfile, Func<string, IProfile> profileProvider);
    }

    internal class PluginProfileDtoSerializer<T> : IPluginProfileDtoSerializer where T : class, new()
    {
        public PluginProfileDto DeserializeProfileFromClient(string serializedPluginProfile, Func<string, IProfile> profileProvider)
        {
            var deserialized = serializedPluginProfile.Deserialize<PluginProfileTypedDto<T>>(typeof(T));
            PopulateSecretMembers(profileProvider(deserialized.Name), deserialized);
            return deserialized;
        }

        public string Serialize(PluginProfileDto profile)
        {
            return ((PluginProfileTypedDto<T>) profile).Serialize(new[] { typeof(T) });
        }

        public string SerializeForClient(PluginProfileDto profile)
        {
            return EvictSecretMembers((PluginProfileTypedDto<T>)profile).Serialize(new[] { typeof(T) });
        }

        public string SerializeForClient(PluginProfileDto[] profile)
        {
            return profile
                .Cast<PluginProfileTypedDto<T>>()
                .Select(EvictSecretMembers)
                .ToArray()
                .Serialize(new[] { typeof(T) });
        }

        private static PluginProfileTypedDto<T> EvictSecretMembers(PluginProfileTypedDto<T> profile)
        {
            var props = profile.Settings.GetType().GetProperties();
            var secretMembers = props.Where(p => p.GetCustomAttribute<SecretMemberAttribute>().HasValue).ToArray();
            if (secretMembers.Empty())
            {
                return profile;
            }

            var newProfile = new PluginProfileTypedDto<T> { Name = profile.Name, Settings = new T() };
            props.Except(secretMembers).ForEach(p =>
            {
                if (!p.GetCustomAttribute<IgnoreDataMemberAttribute>().HasValue)
                {
                    p.SetValue(newProfile.Settings, p.GetValue(profile.Settings));
                }
            });
            return newProfile;
        }

        private static void PopulateSecretMembers(IProfile profile, PluginProfileTypedDto<T> deserialized)
        {
            if (profile.IsNull)
            {
                return;
            }

            typeof(T)
                .GetProperties()
                .Where(p => p.GetCustomAttribute<SecretMemberAttribute>().HasValue)
                .ForEach(p =>
                {
                    if (p.GetValue(deserialized.Settings) == null)
                    {
                        p.SetValue(deserialized.Settings, p.GetValue(profile.Settings));
                    }
                });
        }
    }

    public static class PluginProfileDtoSerializationExtensions
    {
        public static string Serialize(this PluginProfileDto pluginProfileDto)
        {
            return CreateProfileSerializer().Serialize(CreateTypedProfile(pluginProfileDto));
        }

        public static string SerializeForClient(this PluginProfileDto pluginProfileDto)
        {
            return CreateProfileSerializer().SerializeForClient(CreateTypedProfile(pluginProfileDto));
        }

        public static string SerializeForClient(this PluginProfileDto[] pluginProfileDtos)
        {
            var typedProfileDtos = pluginProfileDtos.Select(CreateTypedProfile);
            return CreateProfileSerializer().SerializeForClient(typedProfileDtos.ToArray());
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

        private static Type SettingsType => ObjectFactory.GetInstance<IPluginMetadata>().ProfileType;

        public static PluginProfileDto DeserializeProfile(this string serializedPluginProfile, Func<string, IProfile> profileProvider)
        {
            return CreateProfileSerializer().DeserializeProfileFromClient(serializedPluginProfile, profileProvider);
        }
    }
}
