namespace Tp.Integration.Messages.PluginLifecycle
{
    public interface IMashupDirectoryIgnoreStrategy
    {
        bool ShouldIgnoreMashupDirectory(string directory);
    }
}
