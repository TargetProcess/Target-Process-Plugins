namespace Tp.Search.Model.Document
{
	public interface IIndexDataFactory
	{
		ProjectIndexData CreateProjectData(params int?[] projectIds);
		string CreateEntityStateData(int entityStateId);
		string CreateSquadData(int? squadId);
		string CreateImpedimentData(bool? isPrivate, int? ownerId, int? responsibleId);
	}
}