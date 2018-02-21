namespace Tp.Search.Model.Document
{
    public interface IIndexDataFactory
    {
        ProjectIndexData CreateProjectData(params int?[] projectIds);
        string CreateEntityStateData(int entityStateId);
        SquadIndexData CreateSquadData(params int?[] squadIds);
        string CreateImpedimentData(bool? isPrivate, int? ownerId, int? responsibleId);
    }
}
