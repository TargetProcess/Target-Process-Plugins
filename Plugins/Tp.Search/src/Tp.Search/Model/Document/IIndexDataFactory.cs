namespace Tp.Search.Model.Document
{
	public interface IIndexDataFactory
	{
		string CreateProjectData(int? projectId);
		string CreateEntityStateData(int entityStateId);
		string CreateSquadData(int? squadId);
		string CreateImpedimentData(bool? isPrivate, int? ownerId, int? responsibleId);
	}
}