namespace Tp.Integration.Plugin.Common.Domain
{
	class PluginCurrentObjectContext : IPluginCurrentObjectContext
	{
		private readonly IPluginContext _context;
		private readonly IAccountCollection _accountCollection;

		public PluginCurrentObjectContext(IPluginContext context, IAccountCollection accountCollection)
		{
			_context = context;
			_accountCollection = accountCollection;
		}

		IProfileReadonly IPluginCurrentObjectContextReadonly.Profile
		{
			get { return Profile; }
		}

		IProfileCollectionReadonly IPluginCurrentObjectContextReadonly.ProfileCollection
		{
			get
			{
				IAccountReadonly account = Account;
				return account.Profiles;
			}
		}

		IProfileCollection IPluginCurrentObjectContext.ProfileCollection
		{
			get { return Account.Profiles; }
		}

		public IProfile Profile
		{
			get
			{
				IPluginCurrentObjectContext c = this;
				return c.ProfileCollection[_context.ProfileName];
			}
		}
		private IAccount Account
		{
			get{ return _accountCollection.GetOrCreate(_context.AccountName); }
		}
	}
}
