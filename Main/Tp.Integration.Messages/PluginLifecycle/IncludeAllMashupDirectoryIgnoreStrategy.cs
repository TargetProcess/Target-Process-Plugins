namespace Tp.Integration.Messages.PluginLifecycle
{
    public class IncludeAllMashupDirectoryIgnoreStrategy : IMashupDirectoryIgnoreStrategy
    {
        public bool ShouldIgnoreMashupDirectory(string directory)
        {
            return false;
        }
    }
}
