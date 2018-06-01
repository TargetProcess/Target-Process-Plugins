using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Tp.Integration.Messages;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;

namespace Tp.Integration.Plugin.Common.Tests
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class PluginProfileDtoSerializerTests
    {
        [DataContract]
        class Profile
        {
            [DataMember]
            public string PublicProp { get; set; }

            [DataMember]
            [SecretMember]
            public string PrivateProp { get; set; }
        }

        [Test]
        public void TestSerializeWithPrivateMembers()
        {
            var serialized = new PluginProfileDtoSerializer<Profile>().SerializeForClient(new PluginProfileTypedDto<Profile>
            {
                Settings = new Profile { PrivateProp = "private", PublicProp = "public" }
            });
            dynamic deserialized = JsonConvert.DeserializeObject(serialized);
            Assert.AreEqual("public", deserialized.Settings.PublicProp.ToString());
            Assert.AreEqual("", deserialized.Settings.PrivateProp.ToString());
        }

        [Test]
        public void TestDeserializeWithPrivateMembers_NoProfiles()
        {
            const string json = "{\"Settings\": {\"PublicProp\": \"public\", \"PrivateProp\": null}}";
            var profiles = new ProfileCollection("", new List<ProfileDomainObject>());
            var profile = new PluginProfileDtoSerializer<Profile>().DeserializeProfileFromClient(json, p => profiles[p]);
            Assert.AreEqual("public", ((Profile)profile.Settings).PublicProp);            
            Assert.AreEqual(null, ((Profile)profile.Settings).PrivateProp);            
        }

        [Test]
        public void TestDeserializeWithPrivateMembers_NoProfile()
        {
            const string json = "{\"Settings\": {\"PublicProp\": \"public\", \"PrivateProp\": null}, \"Name\": \"P\"}";
            var profiles = new ProfileCollection("",
                new ProfileDomainObject(new ProfileName("P1"), new AccountName(), true, typeof(Profile)).Yield());
            var profile = new PluginProfileDtoSerializer<Profile>().DeserializeProfileFromClient(json, p => profiles[p]);
            Assert.AreEqual("public", ((Profile)profile.Settings).PublicProp);
            Assert.AreEqual(null, ((Profile)profile.Settings).PrivateProp);
        }

        [Test]
        public void TestDeserializeWithPrivateMembers_NullValue()
        {
            const string json = "{\"Settings\": {\"PublicProp\": \"public\", \"PrivateProp\": null}, \"Name\": \"P\"}";
            var profiles = new ProfileCollection("",
                new ProfileDomainObject(new ProfileName("P"), new AccountName(), true, typeof(Profile))
                    { Settings = new Profile { PrivateProp = "private" } }.Yield());
            var profile = new PluginProfileDtoSerializer<Profile>().DeserializeProfileFromClient(json, p => profiles[p]);
            Assert.AreEqual("public", ((Profile)profile.Settings).PublicProp);
            Assert.AreEqual("private", ((Profile)profile.Settings).PrivateProp);
        }

        [Test]
        public void TestDeserializeWithPrivateMembers_SetValue()
        {
            const string json = "{\"Settings\": {\"PublicProp\": \"public\", \"PrivateProp\": \"newprivate\"}, \"Name\": \"P\"}";
            var profiles = new ProfileCollection("",
                new ProfileDomainObject(new ProfileName("P"), new AccountName(), true, typeof(Profile))
                    { Settings = new Profile { PrivateProp = "private" } }.Yield());
            var profile = new PluginProfileDtoSerializer<Profile>().DeserializeProfileFromClient(json, p => profiles[p]);
            Assert.AreEqual("public", ((Profile)profile.Settings).PublicProp);
            Assert.AreEqual("newprivate", ((Profile)profile.Settings).PrivateProp);
        }
    }
}
