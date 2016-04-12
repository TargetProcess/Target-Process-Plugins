using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Tp.MashupManager.MashupLibrary.Repository.Config;
using Tp.Testing.Common.NUnit;

namespace Tp.MashupManager.Tests.MashupLibrary
{
	[TestFixture]
    [Category("PartPlugins0")]
	public class LibraryRepositoryConfigStorageTests
	{
		private readonly IEnumerable<ILibraryRepositoryConfig> _expectedProfileConfigs =
			new[]
				{
					new LibraryRepositoryConfig
						{
							Login = "ProfileLogin",
							Name = "ProfileName",
							Password = "ProfilePassword",
							Uri = "ProfileUri"
						}
				}
				.Cast<ILibraryRepositoryConfig>();

		private readonly IEnumerable<ILibraryRepositoryConfig> _expectedDefaultConfigs =
			new[]
				{
					new LibraryRepositoryConfig
						{
							Login = "DefaultLogin",
							Name = "DefaultName",
							Password = "DefaultPassword",
							Uri = "DefaultUri"
						}
				}
				.Cast<ILibraryRepositoryConfig>();

		[Test]
		public void ShouldReturnDefaultConfigsIfProfileIsNotCreatedYet()
		{
			var singleProfileStub = MockRepository.GenerateStub<ISingleProfile>();
			singleProfileStub.Stub(x => x.Profile).Return(null);
			var configStorage = new LibraryRepositoryConfigStorage(singleProfileStub){DefaultConfigs = _expectedDefaultConfigs};
			var configs = configStorage.GetConfigs();

			configs.Should(Be.EqualTo(_expectedDefaultConfigs), "library repository configs is not equal to expected");
		}

		[Test]
		public void ShouldReturnDefaultConfigsIfProfileDoesNotContainConfigs()
		{
			var singleProfileStub = MockRepository.GenerateStub<ISingleProfile>();
			singleProfileStub
				.Stub(x => x.Profile.GetProfile<MashupManagerProfile>())
				.Return(new MashupManagerProfile());
			var configStorage = new LibraryRepositoryConfigStorage(singleProfileStub){DefaultConfigs = _expectedDefaultConfigs};
			var configs = configStorage.GetConfigs();

			configs.Should(Be.EqualTo(_expectedDefaultConfigs), "library repository configs is not equal to expected");
		}

		[Test]
		public void ShouldReturnConfigsFromProfileIfTheyExist()
		{
			var singleProfileStub = MockRepository.GenerateStub<ISingleProfile>();
			singleProfileStub
				.Stub(x => x.Profile.GetProfile<MashupManagerProfile>())
				.Return(new MashupManagerProfile
					{
						LibraryRepositoryConfigs = _expectedProfileConfigs.Cast<LibraryRepositoryConfig>().ToArray()
					});
			var configStorage = new LibraryRepositoryConfigStorage(singleProfileStub) { DefaultConfigs = _expectedDefaultConfigs };
			var configs = configStorage.GetConfigs();

			configs.Should(Be.EqualTo(_expectedProfileConfigs), "library repository configs is not equal to expected");
		}
	}
}