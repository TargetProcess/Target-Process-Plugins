namespace Tp.Model.Common.Interfaces
{
    public interface IEntityState
    {
        string Name { get; }
        bool? Planned { get; }
        bool? Initial { get; }
        bool? Final { get; }
        double? NumericPriority { get; }
    }
}
