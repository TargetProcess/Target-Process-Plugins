namespace Tp.Integration.Messages.EntityLifecycle
{
    /// <summary>
    /// Marker interface to indicate that a class represents a command that can be sent to TargetProcess.
    /// </summary>
    public interface ITargetProcessCommand : ITargetProcessMessage
    {
    }
}
