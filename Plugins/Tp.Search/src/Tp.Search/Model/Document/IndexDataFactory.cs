namespace Tp.Search.Model.Document
{
    class IndexDataFactory : IIndexDataFactory
    {
        public ProjectIndexData CreateProjectData(params int?[] projectIds)
        {
            return new ProjectIndexData(projectIds);
        }

        public string CreateEntityStateData(int entityStateId)
        {
            return IndexDataStringServices.EncodeStringId(entityStateId, "Entitystate");
        }

        public SquadIndexData CreateSquadData(params int?[] squadIds)
        {
            return new SquadIndexData(squadIds);
        }

        public string CreateImpedimentData(bool? isPrivate, int? ownerId, int? responsibleId)
        {
            var parts = new[]
            {
                isPrivate.GetValueOrDefault() ? string.Empty : "public",
                ownerId.HasValue ? IndexDataStringServices.EncodeStringId(ownerId.Value, "Owner") : string.Empty,
                responsibleId.HasValue ? IndexDataStringServices.EncodeStringId(responsibleId.Value, "Responsible") : string.Empty
            };
            return IndexDataStringServices.OfParts(parts);
        }
    }
}
