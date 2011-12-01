// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using StructureMap;
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
			return serializedPluginProfile.Deserialize<PluginProfileTypedDto<T>>(new[] {typeof (T)});
		}

		string IPluginProfileDtoSerializer.Serialize(PluginProfileDto profile)
		{
			return ((PluginProfileTypedDto<T>) profile).Serialize(new[] {typeof (T)});
		}

		string IPluginProfileDtoSerializer.Serialize(PluginProfileDto[] profile)
		{
			return profile.Cast<PluginProfileTypedDto<T>>().ToArray().Serialize(new[] {typeof (T)});
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
				typeof (PluginProfileDtoSerializer<>).MakeGenericType(SettingsType);
			return (IPluginProfileDtoSerializer) Activator.CreateInstance(makeGenericType);
		}

		private static PluginProfileDto CreateTypedProfile(PluginProfileDto pluginProfileDto)
		{
			var profileGenericType = typeof (PluginProfileTypedDto<>).MakeGenericType(SettingsType);
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

	public static class SerializationExtensions
	{
		public static string Serialize<T>(this T obj) where T : class
		{
			return obj.Serialize(new Type[] {});
		}

		public static string Serialize<T>(this T obj, IEnumerable<Type> knownTypes) where T : class
		{
			List<Type> knownTypesList = knownTypes.ToList();
			knownTypesList.Add(obj.GetType());
			var serializer = new DataContractJsonSerializer(typeof (T), knownTypesList);

			var ms = new MemoryStream();
			serializer.WriteObject(ms, obj);

			return Encoding.UTF8.GetString(ms.ToArray());
		}

		public static T Deserialize<T>(this string content) where T : class
		{
			return content.Deserialize<T>(new Type[] {});
		}

		public static object Deserialize(this string content, Type contentValueType, params Type[] knownTypes)
		{
			var serializer = new DataContractJsonSerializer(contentValueType, knownTypes);
			byte[] buffer = Encoding.UTF8.GetBytes(content);
			return
				serializer.ReadObject(JsonReaderWriterFactory.CreateJsonReader(buffer, 0, buffer.Length, Encoding.UTF8,
				                                                               XmlDictionaryReaderQuotas.Max, null));
		}

		public static T Deserialize<T>(this string content, params Type[] knownTypes)
		{
			return (T) content.Deserialize(typeof (T), knownTypes);
		}
	}
}