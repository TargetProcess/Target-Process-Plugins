using System.Linq;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Messages;
using Tp.Integration.Messages.AccountLifecycle;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Handlers;
using Tp.Integration.Plugin.Common.Tests.Common;
using Tp.Integration.Plugin.TaskCreator.Tests;
using Tp.Integration.Testing.Common;

namespace Tp.Integration.Plugin.Common.Tests.Handlers
{
	using Plugin.Common.Activity;

	[TestFixture]
    [Category("PartPlugins1")]
	public sealed class AccountHandlerTests
	{
		private AccountHandler _accountHandler;
		private AccountCollectionMock _accountCollection;
		private TpBusMock _bus;
		private PluginContextMock _pluginContextMock;

		public AccountHandlerTests()
		{
			_accountCollection = new AccountCollectionMock();
			_bus = new TpBusMock();
			_pluginContextMock = new PluginContextMock();
			_accountHandler = new AccountHandler(_accountCollection, _bus, _pluginContextMock,
			                                     new TpLogManager(new ActivityLogPathProvider(), _pluginContextMock),
			                                     ObjectFactory.GetInstance<TransportMock>());
		}

		[TestCase]
		public void RemoveAccount()
		{
			var accountName = new AccountName("Account");
			_accountCollection.GetOrCreate(accountName);
			Assert.IsTrue(_accountCollection.Any(c => c.Name == accountName.Value));
			_accountHandler.Handle(new AccountRemovedMessage(accountName.Value));
			_accountHandler.Handle(_bus.PluginCommands.OfType<AccountRemovedLastStepMessage>().Single());
			Assert.IsFalse(_accountCollection.Any(c => c.Name == accountName.Value));
		}

		[TestCase]
		public void AddAccount()
		{
			var accountName = "Account";
			Assert.IsFalse(_accountCollection.Any(c => c.Name == accountName));

			_accountHandler.Handle(new AccountAddedMessage(accountName));
			Assert.IsTrue(_accountCollection.Any(c => c.Name == accountName));
			Assert.AreEqual(1, _bus.PluginCommands.Count);

			var message = _bus.PluginCommands[0] as PluginAccountMessageSerialized;
			Assert.IsNotNull(message);

			var accounts = message.GetAccounts();
			Assert.AreEqual(1, accounts.Length);

			var account = accounts[0];
			Assert.AreEqual(accountName, account.Name.Value);
		}
	}
}
