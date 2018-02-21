// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Tp.Tfs.WorkItemsIntegration.FieldsMapping
{
    public class WorkItemsFieldsMapper
    {
        [DataContract]
        public class NamesPair
        {
            public NamesPair()
            {
            }

            public NamesPair(string name, string visualName)
            {
                Name = name;
                VisualName = visualName;
            }

            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public string VisualName { get; set; }
        }

        private readonly Dictionary<string, Dictionary<string, NamesPair[]>> _mapping = new Dictionary
            <string, Dictionary<string, NamesPair[]>>
            {
                {
                    Constants.TfsBug,
                    new Dictionary<string, NamesPair[]>
                    {
                        { "Name", new[] { new NamesPair("Title", "Title") } },
                        {
                            "Description",
                            new[]
                            {
                                new NamesPair("Repro Steps", "Steps To Reproduce"),
                                new NamesPair("Acceptance Criteria", "Acceptance Criteria")
                            }
                        }
                    }
                },
                {
                    Constants.TfsUserStory,
                    new Dictionary<string, NamesPair[]>
                    {
                        { "Name", new[] { new NamesPair("Title", "Title") } },
                        { "Description", new[] { new NamesPair("Description", "Description") } }
                    }
                },
                {
                    Constants.TfsTask,
                    new Dictionary<string, NamesPair[]>
                    {
                        { "Name", new[] { new NamesPair("Title", "Title") } },
                        { "Description", new[] { new NamesPair("Description", "Description") } }
                    }
                },
                {
                    Constants.TfsIssue,
                    new Dictionary<string, NamesPair[]>
                    {
                        { "Name", new[] { new NamesPair("Title", "Title") } },
                        { "Description", new[] { new NamesPair("Description", "Description") } }
                    }
                },
                {
                    Constants.TfsBacklogItem,
                    new Dictionary<string, NamesPair[]>
                    {
                        { "Name", new[] { new NamesPair("Title", "Title") } },
                        {
                            "Description",
                            new[]
                                { new NamesPair("Description", "Description"), new NamesPair("Acceptance Criteria", "Acceptance Criteria") }
                        }
                    }
                },
                {
                    Constants.TfsImpediment,
                    new Dictionary<string, NamesPair[]>
                    {
                        { "Name", new[] { new NamesPair("Title", "Title") } },
                        { "Description", new[] { new NamesPair("Description", "Description") } }
                    }
                }
            };

        private static WorkItemsFieldsMapper _instance;
        private static readonly object LockObject = new object();

        private WorkItemsFieldsMapper()
        {
        }

        public static WorkItemsFieldsMapper Instance
        {
            get
            {
                lock (LockObject)
                {
                    if (_instance == null)
                        _instance = new WorkItemsFieldsMapper();
                }

                return _instance;
            }
        }

        public string GetMappedTpField(string workItemType, string field)
        {
            var mapping = _mapping[workItemType].Where(x => x.Value.FirstOrDefault(pair => pair.Name == field) != null).ToArray();

            return mapping.Any() ? mapping.Single().Key : null;
        }

        public Dictionary<string, NamesPair[]> GetMappingForWorkItemType(string workItemType)
        {
            return _mapping[workItemType];
        }
    }
}
