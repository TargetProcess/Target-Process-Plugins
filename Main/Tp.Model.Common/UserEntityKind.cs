namespace Tp.Model.Common
{
    public enum UserEntityKind : byte
    {
        None = 0,
        User = 1,
        SystemUser = 3,
        Requester = 4,
        Contact = 5,
        RuleEngine = 6,
        MetricsUser = 7,
    }
}
