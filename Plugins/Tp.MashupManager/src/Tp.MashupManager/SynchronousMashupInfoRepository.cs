using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;
using Tp.MashupManager.CustomCommands.Args;

namespace Tp.MashupManager
{
    public class SynchronousMashupInfoRepository : IMashupInfoRepository
    {
        private readonly IPluginContext _context;

        private readonly IAccountLocker _locker;

        private readonly IMashupInfoRepository _inner;

        public SynchronousMashupInfoRepository(IPluginContext context, IAccountLocker locker, IMashupInfoRepository inner)
        {
            _context = context;
            _locker = locker;
            _inner = inner;
        }

        public PluginProfileErrorCollection Add(Mashup dto, bool generateUniqueName)
        {
            return _locker.ExecuteInAccountLock(() => _inner.Add(dto, generateUniqueName), _context.AccountName);
        }

        public PluginProfileErrorCollection Update(UpdateMashupCommandArg commandArg)
        {
            return _locker.ExecuteInAccountLock(() => _inner.Update(commandArg), _context.AccountName);
        }

        public PluginProfileErrorCollection Delete(string mashupName)
        {
            return _locker.ExecuteInAccountLock(() => _inner.Delete(mashupName), _context.AccountName);
        }
    }
}