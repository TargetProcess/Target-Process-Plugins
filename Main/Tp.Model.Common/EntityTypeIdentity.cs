using System.Collections.Generic;
using Tp.BusinessObjects;
using Tp.Core;

namespace Tp.Model.Common
{
    /// <summary>
    ///     Contains the values of entity type id
    /// </summary>
    public static class EntityTypeIdentity
    {
        private static readonly Dictionary<EntityKind, int> _kindToId = new Dictionary<EntityKind, int>();
        private static readonly Dictionary<int, EntityKind> _idToKind = new Dictionary<int, EntityKind>();

        private static readonly Dictionary<UserEntityKind, EntityKind> _userKindToKind = new Dictionary<UserEntityKind, EntityKind>();
        private static readonly Dictionary<EntityKind, UserEntityKind> _kindToUserKind = new Dictionary<EntityKind, UserEntityKind>();

        static EntityTypeIdentity()
        {
            Add(EntityKind.Program, PROGRAM_TYPE_ID);
            Add(EntityKind.Project, PROJECT_TYPE_ID);
            Add(EntityKind.Release, RELEASE_TYPE_ID);
            Add(EntityKind.Iteration, ITERATION_TYPE_ID);
            Add(EntityKind.UserStory, USERSTORY_TYPE_ID);
            Add(EntityKind.Task, TASK_TYPE_ID);
            Add(EntityKind.User, USER_TYPE_ID);
            Add(EntityKind.Time, TIME_TYPE_ID);
            Add(EntityKind.Bug, BUG_TYPE_ID);
            Add(EntityKind.Feature, FEATURE_TYPE_ID);
            Add(EntityKind.Build, BUILD_TYPE_ID);
            Add(EntityKind.TestCase, TESTCASE_TYPE_ID);
            Add(EntityKind.TestPlan, TESTPLAN_TYPE_ID);
            Add(EntityKind.TestPlanRun, TESTPLANRUN_TYPE_ID);
            Add(EntityKind.Impediment, IMPEDIMENT_TYPE_ID);
            Add(EntityKind.Request, REQUEST_TYPE_ID);
            Add(EntityKind.Requester, REQUESTER_TYPE_ID);
            Add(EntityKind.Attachment, ATTACHMENT_TYPE_ID);
            Add(EntityKind.Comment, COMMENT_TYPE_ID);
            Add(EntityKind.Company, COMPANY_TYPE_ID);
            Add(EntityKind.Squad, SQUAD_TYPE_ID);
            Add(EntityKind.SquadIteration, SQUAD_ITERATION_TYPE_ID);
            Add(EntityKind.TestStep, TESTSTEP_TYPE_ID);
            Add(EntityKind.TestStepRun, TESTSTEPRUN_TYPE_ID);
            Add(EntityKind.Epic, EPIC_TYPE_ID);
            Add(EntityKind.TestCaseRun, TESTCASERUN_TYPE_ID);
            Add(EntityKind.UserProjectAllocation, USERPROJECTALLOCATION_TYPE_ID);
            Add(EntityKind.SquadProjectAllocation, SQUADPROJECTALLOCATION_TYPE_ID);
            Add(EntityKind.PortfolioEpic, PORTFOLIO_EPIC_TYPE_ID);
            Add(EntityKind.Milestone, MILESTONE_TYPE_ID);

            Add(UserEntityKind.None, EntityKind.None);
            Add(UserEntityKind.User, EntityKind.User);
            Add(UserEntityKind.SystemUser, EntityKind.SystemUser);
            Add(UserEntityKind.Requester, EntityKind.Requester);
            Add(UserEntityKind.Contact, EntityKind.Contact);
            Add(UserEntityKind.RuleEngine, EntityKind.None);
            Add(UserEntityKind.MetricsUser, EntityKind.None);
        }

        private static void Add(EntityKind entityKind, int id)
        {
            _kindToId.Add(entityKind, id);
            _idToKind.Add(id, entityKind);
        }

        private static void Add(UserEntityKind userEntityKind, EntityKind entityKind)
        {
            _userKindToKind.Add(userEntityKind, entityKind);

            if (!_kindToUserKind.ContainsKey(entityKind))
            {
                _kindToUserKind.Add(entityKind, userEntityKind);
            }
        }

        public const int BUG_TYPE_ID = 8;
        public const int BUILD_TYPE_ID = 11;
        public const int FEATURE_TYPE_ID = 9;
        public const int IMPEDIMENT_TYPE_ID = 16;
        public const int ITERATION_TYPE_ID = 3;
        public const int PROGRAM_TYPE_ID = 10;
        public const int PROJECT_TYPE_ID = 1;
        public const int RELEASE_TYPE_ID = 2;
        public const int REQUEST_TYPE_ID = 17;
        public const int REQUESTER_TYPE_ID = 18;
        public const int TASK_TYPE_ID = 5;
        public const int TESTCASE_TYPE_ID = 12;
        public const int TESTPLAN_TYPE_ID = 13;
        public const int TESTPLANRUN_TYPE_ID = 14;
        public const int TIME_TYPE_ID = 7;
        public const int USER_TYPE_ID = 6;
        public const int USERSTORY_TYPE_ID = 4;
        public const int COMMENT_TYPE_ID = 19;
        public const int ATTACHMENT_TYPE_ID = 20;
        public const int COMPANY_TYPE_ID = 22;
        public const int SQUAD_TYPE_ID = 23;
        public const int SQUAD_ITERATION_TYPE_ID = 24;
        public const int TESTSTEP_TYPE_ID = 25;
        public const int TESTSTEPRUN_TYPE_ID = 26;
        public const int EPIC_TYPE_ID = 27;
        public const int TESTCASERUN_TYPE_ID = 28;
        public const int USERPROJECTALLOCATION_TYPE_ID = 29;
        public const int SQUADPROJECTALLOCATION_TYPE_ID = 30;
        public const int PORTFOLIO_EPIC_TYPE_ID = 31;
        public const int MILESTONE_TYPE_ID = 32;

        public static int? ToEntityTypeID(this EntityKind entityKind)
        {
            if (ExtendableDomain.IsExtendableDomainKind(entityKind))
            {
                return (int) entityKind;
            }

            return _kindToId.GetValue(entityKind).ToNullable();
        }

        public static EntityKind ToEntityKind(this int entityTypeID)
        {
            if (ExtendableDomain.IsExtendableDomainType(entityTypeID))
            {
                return (EntityKind) entityTypeID;
            }

            return _idToKind[entityTypeID];
        }

        public static Maybe<EntityKind> MaybeToEntityKind(this int entityTypeID)
        {
            if (ExtendableDomain.IsExtendableDomainType(entityTypeID))
            {
                return Maybe.Just((EntityKind) entityTypeID);
            }

            return _idToKind.GetValue(entityTypeID);
        }

        public static EntityKind ToEntityKind(this UserEntityKind userKind)
        {
            return _userKindToKind[userKind];
        }

        public static Maybe<UserEntityKind> ToUserEntityKind(this EntityKind entityKind)
        {
            return _kindToUserKind.GetValue(entityKind);
        }

        public static Maybe<UserEntityKind> ToUserEntityKind(this int entityTypeId)
        {
            return entityTypeId.ToEntityKind().ToUserEntityKind();
        }

        public static Maybe<UserEntityKind> ToUserEntityKind(this int? entityTypeId)
        {
            return entityTypeId.NothingIfNull().Bind(x => x.ToUserEntityKind());
        }
    }
}
