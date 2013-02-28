using System.Reflection;
using NUnit.Framework;
using Tp.Mercurial;
using System;
using Tp.Integration.Plugin.Common.Validation;
using Tp.Testing.Common.NUnit;

namespace Tp.Mercurial.Tests
{
    [TestFixture]
    public class ModelsTest
    {
        [Test]
        public void ValidateCredentialsInUriTest()
        {
            string uri = RunValidation("https://vanya@server.com", "vanya", "123456");
            uri.Should(Be.EqualTo("https://vanya:123456@server.com"));
        }

        [Test]
        public void ValidateCredentialsInUriWithoutLoginTest()
        {
            string uri = RunValidation("https://server.com", "vanya", "123456");
            uri.Should(Be.EqualTo("https://vanya:123456@server.com"));
        }

        [Test]
        public void ValidateCredentialsInUriWithoutPasswordTest()
        {
            string uri = RunValidation("https://server.com", "vanya", "");
            uri.Should(Be.EqualTo("https://vanya@server.com"));
        }

        [Test]
        public void ValidateCredentialsInUriWithLoginSubstringTest()
        {
            string uri = RunValidation("https://server.com/vanya", "vanya", "123456");
            uri.Should(Be.EqualTo("https://vanya:123456@server.com/vanya"));
        }

        [Test]
        public void ValidateCredentialsRightUriTest()
        {
            string uri = RunValidation("https://vanya:123456@server.com/vanya", "vanya", "123456");
            uri.Should(Be.EqualTo("https://vanya:123456@server.com/vanya"));
        }

        [Test]
        public void ValidateCredentialsWithDiffCreditialsTest()
        {
            string uri = RunValidation("https://vanya:123456@server.com/vanya", "kolya", "password");
            uri.Should(Be.EqualTo("https://vanya:123456@server.com/vanya"));
        }

        private string RunValidation(string uri, string login, string password)
        {
            var profile = new MercurialPluginProfile();
            PluginProfileErrorCollection errors = new PluginProfileErrorCollection();

            profile.Uri = uri;
            profile.Login = login;
            profile.Password = password;

            var methodInfo = typeof(MercurialPluginProfile).GetMethod(
                "ValidateCredentialsInUri",
                BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(profile, new object[] { errors });

            return profile.Uri;
        }
    }
}
