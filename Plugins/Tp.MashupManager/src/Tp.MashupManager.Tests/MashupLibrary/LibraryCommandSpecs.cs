using System.Collections.Generic;
using System.Linq;
using NBehave.Narrator.Framework;
using NServiceBus.Unicast.Transport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.ServiceBus;
using Tp.MashupManager.CustomCommands.Args;
using Tp.MashupManager.CustomCommands.Dtos;
using Tp.MashupManager.MashupLibrary.Package;
using Tp.MashupManager.MashupLibrary.Repository;
using Tp.MashupManager.MashupLibrary.Repository.Config;
using Tp.MashupManager.Tests.MashupLibrary.Repository;
using Tp.MashupManager.Tests.MashupLibrary.Repository.Config;
using Tp.Testing.Common.NBehave;
using Tp.Testing.Common.NUnit;

namespace Tp.MashupManager.Tests.MashupLibrary
{
	[TestFixture]
	[ActionSteps]
    [Category("PartPlugins0")]
	public class LibraryCommandSpecs : MashupManagerTestBase
	{
		public override void SetUp()
		{
			base.SetUp();
			var libraryRepositoryFactoryStub = MockRepository.GenerateStub<ILibraryRepositoryFactory>();
			libraryRepositoryFactoryStub
				.Stub(x => x.GetRepository(Arg<LibraryRepositoryConfig>.Is.Anything))
				.WhenCalled(invocation =>
					{
						LibraryRepositoryMock repositoryMock =
							MockedRepositories.FirstOrDefault(
								x =>
								x.Config.Name == ((LibraryRepositoryConfig) invocation.Arguments[0]).Name) ??
							new LibraryRepositoryMock(
								(LibraryRepositoryConfig) invocation.Arguments[0]);
						MockedRepositories.Add(repositoryMock);
						invocation.ReturnValue = repositoryMock;
					});
			ObjectFactory.Configure(x =>
				{
					x.For<ILibraryRepositoryFactory>().Use(libraryRepositoryFactoryStub);
					x.For<ILibraryRepositoryConfigStorage>().HybridHttpOrThreadLocalScoped().Use<LibraryRepositoryConfigStorageMock>();
					x.Forward<ILibraryRepositoryConfigStorage, LibraryRepositoryConfigStorageMock>();
				});
		}

		[Test]
		public void ShouldRefreshEmptyLibraryOnCommand()
		{
			@"
				Given profile created
				When handle 'RefreshLibrary' command
				Then command should return Success
			".Execute();
		}

		[Test]
		public void ShouldRefreshLibraryOnCommand()
		{
			@"
				Given profile created
					And profile contains following git source configs:
					|name|path          |login|password|
					|Tp3 |http://tp3Path|     |        |
					|Cat |http://catPath|login|pass    |
				When handle 'RefreshLibrary' command
				Then command should return Success
					And following libraries should be refreshed: Tp3,Cat
			".Execute();
		}

		[Test]
		public void ShouldReturnMashupsListOnCommand()
		{
			@"
				Given profile created
					And profile contains following git source configs:
					|name|path          |login|password|
					|Tp3 |http://tp3Path|     |        |
					|Cat |http://catPath|login|pass    |
					And source 'Tp3' contains following packages: a,b
					And source 'Cat' contains following packages: c
				When handle 'GetLibraryRepositories' command
				Then command should return Success
					And following sources should be returned:
					|sourceName|packages|
					|Tp3       |a,b     |
					|Cat       |c       |
					And 2 sources should be returned
			".Execute();
		}
		
		[Test]
		public void ShouldReturnPackageDetailedOnCommand()
		{
			@"
				Given profile created
					And profile contains following git source configs:
					|name|path          |login|password|
					|Tp3 |http://tp3Path|     |        |					
					And source 'Tp3' contains following packages: a,b
					And package 'a' of source 'Tp3' contains ReadmeMarkdown with following content: 'Markdown'
				When handle 'GetPackageDetailed' command with 'Tp3' repositoryName and 'a' packageName
				Then command should return Success
					And following ReadmeMarkdown should be returned: 'Markdown'
			".Execute();
		}
		
		private static readonly List<LibraryRepositoryMock> MockedRepositories = new List<LibraryRepositoryMock>();

		[Given("profile contains following git source configs:")]
		public void SetGitConfigsToProfile(string name, string path, string login, string password)
		{
			var config = new LibraryRepositoryConfig
				{
					Name = name.Trim(),
					Uri = path.Trim(),
					Login = login.Trim(),
					Password = password.Trim()
				};

			StorageConfig.AddConfig(config);
		}

		[Given(@"source '$sourceName' contains following packages: (?<packages>([^,]+,?\s*)+)")]
		public void SetPackagesToSource(string sourceName, string[] packages)
		{
			ILibraryRepositoryConfig config = StorageConfig.GetConfigs().Single(c => c.Name == sourceName);
			var repositoryMock =
				(LibraryRepositoryMock) ObjectFactory.GetInstance<ILibraryRepositoryFactory>().GetRepository(config);
			repositoryMock.Sources.Add(new LibraryRepositoryDto
				{
					Name = config.Name,
					Packages = packages.Select(x => new LibraryPackage {Name = x})
				});
		}

		[Given(@"package '$packageName' of source '$sourceName' contains ReadmeMarkdown with following content: '$markdownContent'")]
		public void SetReadmeMarkdownToPackage(string packageName, string sourceName, string markdownContent)
		{
			ILibraryRepositoryConfig config = StorageConfig.GetConfigs().Single(c => c.Name == sourceName);
			var repositoryMock =
				(LibraryRepositoryMock)ObjectFactory.GetInstance<ILibraryRepositoryFactory>().GetRepository(config);
			repositoryMock.PackagesDetailed.Add(new LibraryPackageDetailed
				{
					Name = packageName,
					BaseInfo = new LibraryPackageBaseInfo {CompatibleTpVersion = new LibraryPackageCompatibleTpVersion()},
					ReadmeMarkdown = markdownContent
				});
		}

		[When("handle '$commandName' command")]
		public void HandleLibraryCommand(string commandName)
		{
			TransportMock.HandleMessageFromTp(
				new List<HeaderInfo>
					{
						new HeaderInfo {Key = BusExtensions.ACCOUNTNAME_KEY, Value = AccountName.Empty.Value},
						new HeaderInfo {Key = BusExtensions.PROFILENAME_KEY, Value = Profile.Name.Value}
					}
				, new ExecutePluginCommandCommand
					{
						CommandName = commandName
					});

			_response = TransportMock.TpQueue.GetMessages<PluginCommandResponseMessage>().Last();
		}

		[When("handle 'GetPackageDetailed' command with '$repositoryName' repositoryName and '$packageName' packageName")]
		public void HandleGetPackageDetaildCommand(string repositoryName, string packageName)
		{
			TransportMock.HandleMessageFromTp(
				new List<HeaderInfo>
					{
						new HeaderInfo {Key = BusExtensions.ACCOUNTNAME_KEY, Value = AccountName.Empty.Value},
						new HeaderInfo {Key = BusExtensions.PROFILENAME_KEY, Value = Profile.Name.Value}
					}
				, new ExecutePluginCommandCommand
				{
					CommandName = "GetPackageDetailed",
					Arguments = (new PackageCommandArg{RepositoryName = repositoryName, PackageName = packageName}).Serialize()
				});

			_response = TransportMock.TpQueue.GetMessages<PluginCommandResponseMessage>().Last();
		}

		[Then("command should return Success")]
		public void CommandShouldReturnSuccess()
		{
			_response.PluginCommandStatus.Should(Is.EqualTo(PluginCommandStatus.Succeed));
		}

		[Then(@"following libraries should be refreshed: (?<libraries>([^,]+,?\s*)+)")]
		public void CheckLibrariesRefreshed(string[] libraries)
		{
			MockedRepositories.SelectMany(x => x.RefreshedLibraries).Select(p => p.Name).Should(Be.EquivalentTo(libraries));
		}

		[Then("following sources should be returned:")]
		public void CheckReturnedSources(string sourceName, string packages)
		{
			IEnumerable<LibraryRepositoryDto> commandResult = GetReturnedSources();
			commandResult
				.Single(r => r.Name == sourceName.Trim()).Packages
				.Select(p => p.Name)
				.Should(Be.EquivalentTo(packages.Split(',').Select(p => p.Trim())));
		}

		[Then("$count sources should be returned")]
		public void CheckReturnedSourcesCount(int count)
		{
			GetReturnedSources().Count().Should(Is.EqualTo(count));
		}

		[Then("following ReadmeMarkdown should be returned: '$markdownContent'")]
		public void CheckReturnedReadmeMarkdown(string markdownContent)
		{
			_response.ResponseData.Deserialize<PackageDetailedDto>().ReadmeMarkdown.Should(Be.EqualTo(markdownContent));
		}

		private static LibraryRepositoryConfigStorageMock StorageConfig
		{
			get { return ObjectFactory.GetInstance<LibraryRepositoryConfigStorageMock>(); }
		}

		private IEnumerable<LibraryRepositoryDto> GetReturnedSources()
		{
			return _response.ResponseData.Deserialize<IEnumerable<LibraryRepositoryDto>>();
		}
	}
}