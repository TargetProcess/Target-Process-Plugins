namespace Tp.Integration.Plugin.Common.Storage.Persisters
{
    partial class PluginDatabaseModelDataContext
    {
        public PluginDatabaseModelDataContext(string connection, int commandTimeoutSeconds) :
            base(connection, mappingSource)
        {
            CommandTimeout = commandTimeoutSeconds;
        }
    }
}
