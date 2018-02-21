// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Search.Model.Document;
using hOOt;

namespace Tp.Search.Model.Entity
{
    public interface IEntityIndexer
    {
        IndexResult AddGeneralIndex(GeneralDTO general, DocumentIndexOptimizeSetup optimizeSetup = null);

        IndexResult UpdateGeneralIndex(GeneralDTO general, ICollection<GeneralField> changedFields,
            DocumentIndexOptimizeSetup optimizeSetup = null);

        IndexResult RemoveGeneralIndex(GeneralDTO general, DocumentIndexOptimizeSetup optimizeSetup = null);
        IndexResult AddAssignableIndex(AssignableDTO assignable, DocumentIndexOptimizeSetup optimizeSetup = null);

        IndexResult UpdateAssignableIndex(AssignableDTO assignable, ICollection<AssignableField> changedFields, bool isIndexing,
            DocumentIndexOptimizeSetup optimizeSetup = null);

        IndexResult RemoveAssignableIndex(AssignableDTO assignable, DocumentIndexOptimizeSetup optimizeSetup = null);
        IndexResult AddTestStepIndex(TestStepDTO testStep, DocumentIndexOptimizeSetup optimizeSetup = null);

        IndexResult UpdateTestStepIndex(TestStepDTO testStep, ICollection<TestStepField> changedFields, Maybe<int?> projectId,
            DocumentIndexOptimizeSetup optimizeSetup = null);

        IndexResult RemoveTestStepIndex(TestStepDTO testStep, DocumentIndexOptimizeSetup optimizeSetup = null);
        IndexResult AddCommentIndex(CommentDTO comment, DocumentIndexOptimizeSetup optimizeSetup = null);

        IndexResult UpdateCommentIndex(CommentDTO comment, ICollection<CommentField> changedFields, bool shouldIndexProjects,
            bool shouldIndexSquads, DocumentIndexOptimizeSetup optimizeSetup = null);

        IndexResult RemoveCommentIndex(CommentDTO comment, DocumentIndexOptimizeSetup optimizeSetup = null);

        IndexResult AddImpedimentIndex(ImpedimentDTO impediment, DocumentIndexOptimizeSetup optimizeSetup = null);

        IndexResult UpdateImpedimentIndex(ImpedimentDTO impediment, ICollection<ImpedimentField> changedFields, bool isIndexing,
            DocumentIndexOptimizeSetup optimizeSetup = null);

        IndexResult RemoveImpedimentIndex(ImpedimentDTO impediment, DocumentIndexOptimizeSetup optimizeSetup = null);

        IndexResult AddReleaseProjectIndex(ReleaseProjectDTO releaseProject, DocumentIndexOptimizeSetup optimizeSetup = null);
        IndexResult RemoveReleaseProjectIndex(ReleaseProjectDTO releaseProject, DocumentIndexOptimizeSetup optimizeSetup = null);
        IndexResult AddAssignableSquadIndex(AssignableSquadDTO assignableSquad, DocumentIndexOptimizeSetup optimizeSetup = null);

        IndexResult UpdateAssignableSquadIndex(AssignableSquadDTO assignableSquad, AssignableSquadDTO originalAssignableSquad,
            ICollection<AssignableSquadField> changedFields, DocumentIndexOptimizeSetup optimizeSetup = null);

        IndexResult RemoveAssignableSquadIndex(AssignableSquadDTO assignableSquad, DocumentIndexOptimizeSetup optimizeSetup = null);

        void OptimizeGeneralIndex(DocumentIndexOptimizeSetup optimizeSetup = null);
        void OptimizeAssignableIndex(DocumentIndexOptimizeSetup optimizeSetup = null);
        void OptimizeTestStepIndex(DocumentIndexOptimizeSetup optimizeSetup = null);
        void OptimizeCommentIndex(DocumentIndexOptimizeSetup optimizeSetup = null);
        void OptimizeImpedimentIndex(DocumentIndexOptimizeSetup optimizeSetup = null);

        void UpdateAssignablesForProjectProcessChange(ProjectDTO project);
    }
}
