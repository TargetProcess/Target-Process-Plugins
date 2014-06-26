// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Testing.Common;
using Tp.Testing.Common.NUnit;

namespace Tp.Integration.Plugin.Common.Tests.Common
{
	[TestFixture]
    [Category("PartPlugins1")]
	public class PluginProfileSerializationTests
	{
		[Test]
		public void ShouldBeAbleToSerializeProfile()
		{
			TransportMock.CreateWithoutStructureMapClear(typeof (SampleProfileSerialized).Assembly);
			var pluginProfile = new PluginProfileDto
			                    	{
			                    		Name = "TestProfile",
			                    		Settings = new SampleProfileSerialized {StringValue = "components"}
			                    	};

			var serializedProfile = pluginProfile.Serialize();
			var deserializedProfile = serializedProfile.DeserializeProfile();

			deserializedProfile.Name.Should(Be.EqualTo("TestProfile"));
			deserializedProfile.Settings.Should(Be.Not.Null, "Settings weren't deserialized");
			var settings = (SampleProfileSerialized) deserializedProfile.Settings;
			settings.StringValue.Should(Be.EqualTo("components"));
		}
	}
}