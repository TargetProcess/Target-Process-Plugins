// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

namespace Tp.Search.Model.Document
{
    public interface IDocumentIdFactory
    {
        string CreateEntityTypeId(int entityTypeId);
        string CreateSquadId(int squadId);
        string CreateEntityStateId(int entityStateId);
        string CreateProjectId(int projectId);
        int ParseProjectId(string projectId);
    }
}
