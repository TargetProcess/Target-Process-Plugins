using NUnit.Framework;
using Tp.Integration.Messages;
using Tp.Integration.Messages.PluginLifecycle;

namespace Tp.MashupManager.Tests
{
    [TestFixture]
    [Category("PartPlugins0")]
    public class MashupConfigTests
    {
        [Test]
        public void AccountsConfigPropertyTest()
        {
            //Assert.AreEqual(?, MashupConfig.AccountsConfigLine((AccountName) null)); behavior is not yet defined
            Assert.AreEqual("Accounts:myAccount", MashupConfig.AccountsConfigLine("myAccount"));
            Assert.AreEqual("Accounts:account1,account2",
                MashupConfig.AccountsConfigLine(new[] { new AccountName("account1"), new AccountName("account2") }));
            Assert.AreEqual("Accounts:account1,account2", MashupConfig.AccountsProperty.Write(new[] {"account1", "account2"}));
        }

        [Test]
        public void PlaceholdersConfigPropertyTest()
        {
            Assert.AreEqual(string.Empty, MashupConfig.PlaceholdersProperty.Write((string) null));
            Assert.AreEqual(string.Empty, MashupConfig.PlaceholdersProperty.Write(""));
            Assert.AreEqual("Placeholders:footer", MashupConfig.PlaceholdersProperty.Write("footer"));
            Assert.AreEqual("Placeholders:footer,header", MashupConfig.PlaceholdersProperty.Write(new[] { "footer", "header" }));
        }

        [Test]
        public void IsEnabledConfigPropertyTest()
        {
            Assert.AreEqual("IsEnabled:true", MashupConfig.IsEnabledProperty.Write(true));
            Assert.AreEqual("IsEnabled:false", MashupConfig.IsEnabledProperty.Write(false));

            bool value;

            Assert.IsFalse(MashupConfig.IsEnabledProperty.TryParse("", out value));
            Assert.IsFalse(MashupConfig.IsEnabledProperty.TryParse("IsEnabled", out value));
            Assert.IsFalse(MashupConfig.IsEnabledProperty.TryParse("IsEnabled:", out value));

            Assert.IsTrue(MashupConfig.IsEnabledProperty.TryParse("IsEnabled:true", out value));
            Assert.IsTrue(value, "Parse 'true'");

            Assert.IsTrue(MashupConfig.IsEnabledProperty.TryParse("IsEnabled:TRUE", out value));
            Assert.IsTrue(value, "Parse 'true' case-insensitively");

            Assert.IsTrue(MashupConfig.IsEnabledProperty.TryParse("IsEnabled:xxx", out value));
            Assert.IsFalse(value, "Any string except 'true' should be interpreted as 'false'");
        }

        [Test]
        public void CreationDateConfigPropertyTest()
        {
            Assert.AreEqual("", MashupConfig.CreationDateProperty.Write(0));
            Assert.AreEqual("CreationDate:1485250272726", MashupConfig.CreationDateProperty.Write(1485250272726));

            ulong value;

            Assert.IsFalse(MashupConfig.CreationDateProperty.TryParse("", out value));
            Assert.IsFalse(MashupConfig.CreationDateProperty.TryParse("CreationDate:text", out value));

            Assert.IsTrue(MashupConfig.CreationDateProperty.TryParse("CreationDate:1485250272726", out value));
            Assert.AreEqual(1485250272726, value);
        }

        [Test]
        public void CreatedByConfigPropertyTest()
        {
            Assert.AreEqual(string.Empty, MashupConfig.CreatedByProperty.Write(null));
            Assert.AreEqual(string.Empty, MashupConfig.CreatedByProperty.Write(new MashupUserInfo()));
            Assert.AreEqual("CreatedBy:#12345 Some User", MashupConfig.CreatedByProperty.Write(new MashupUserInfo
            {
                Id = 12345,
                Name = "Some User"
            }));

            MashupUserInfo value;

            Assert.IsFalse(MashupConfig.CreatedByProperty.TryParse("CreatedBy:", out value));
            Assert.IsFalse(MashupConfig.CreatedByProperty.TryParse("CreatedBy:bla bla", out value));
            Assert.IsFalse(MashupConfig.CreatedByProperty.TryParse("CreatedBy:#13", out value));
            Assert.IsFalse(MashupConfig.CreatedByProperty.TryParse("CreatedBy:#13 ", out value));

            Assert.IsTrue(MashupConfig.CreatedByProperty.TryParse("CreatedBy:#13 Some user name", out value));
            Assert.AreEqual(13, value.Id);
            Assert.AreEqual("Some user name", value.Name);
        }
    }
}
