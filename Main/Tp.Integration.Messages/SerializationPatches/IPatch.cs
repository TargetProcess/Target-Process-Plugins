namespace Tp.Integration.Messages.SerializationPatches
{
    public interface IPatch
    {
        bool NeedToApply(string text);
        string Apply(string text);
    }
}
