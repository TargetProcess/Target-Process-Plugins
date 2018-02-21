using Tp.Integration.Common;
using Tp.Integration.Messages.Commands;

namespace Tp.Integration.Messages.PluginLifecycle.PluginCommand
{
    /// <summary>
    /// Represents a plugin command which can be called from UI. To add a command to plugin just inherit from this class.
    /// </summary>
    public interface IPluginCommand
    {
        /// <summary>
        /// Executes the command and returns the result.
        /// </summary>
        /// <param name="args">Arguments passed to the command from UI.</param>
        /// <param name="user">User who requests command execution</param>
        /// <returns>Command result. Result status is always Fail if some unhandled exception occurs.</returns>
        PluginCommandResponseMessage Execute(string args, UserDTO user = null);

        /// <summary>
        /// Command name. Should be unique for plugin.
        /// </summary>
        string Name { get; }
    }
}
