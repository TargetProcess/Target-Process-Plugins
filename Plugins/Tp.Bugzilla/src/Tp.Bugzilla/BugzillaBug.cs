// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StructureMap;
using Tp.BugTracking;
using Tp.Bugzilla.BugFieldConverters;
using Tp.Bugzilla.Schemas;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Plugin.Core.Attachments;

namespace Tp.Bugzilla
{
    [Serializable]
    public class BugzillaBug
    {
        public string reporter { get; set; }

        public string assigned_to { get; set; }

        public string bug_id { get; set; }

        public string priority { get; set; }

        public string bug_severity { get; set; }

        public string bug_status { get; set; }

        public string op_sys { get; set; }

        public string component { get; set; }

        public string version { get; set; }

        public string classification { get; set; }

        public string rep_platform { get; set; }

        public string description { get; set; }

        public List<BugzillaComment> comments { get; set; }

        public List<custom_field> customFields { get; set; }

        public string creation_ts { get; set; }

        public string short_desc { get; set; }

        public List<LocalStoredAttachment> attachmentCollection { get; set; }

        public BugzillaBug()
        {
            comments = new List<BugzillaComment>();
        }

        public BugzillaBug(bug bug) : this()
        {
            bug_id = bug.bug_id;
            reporter = bug.reporter;
            assigned_to = bug.assigned_to;
            priority = bug.priority;
            bug_severity = bug.bug_severity;
            bug_status = bug.bug_status;

            op_sys = bug.op_sys;
            component = bug.component;
            version = bug.version;
            classification = bug.classification;
            rep_platform = bug.rep_platform;
            customFields = bug.custom_fieldCollection.Cast<custom_field>().ToList();

            var longDescCollection = GetDescriptionCollection(bug);
            if (longDescCollection.Count > 0)
            {
                description = longDescCollection[0].thetext;
            }
            if (longDescCollection.Count > 1)
            {
                comments = longDescCollection
                    .Cast<long_desc>()
                    .Skip(1)
                    .Select(x => new BugzillaComment(x))
                    .ToList();
            }

            creation_ts = bug.creation_ts;
            short_desc = DescriptionConverter.CleanUpContent(bug.short_desc);
            attachmentCollection = CreateAttachments(bug.attachmentCollection.Cast<attachment>());
        }

        private static long_descCollection GetDescriptionCollection(bug bug)
        {
            foreach (long_desc description in bug.long_descCollection)
            {
                description.thetext = DescriptionConverter.CleanUpContent(description.thetext);
            }

            return bug.long_descCollection;
        }

        private List<LocalStoredAttachment> CreateAttachments(IEnumerable<attachment> attachments)
        {
            var storedAttachments = new List<LocalStoredAttachment>();

            foreach (var attachment in attachments.Where(a => a.isobsolete == isobsolete._0 && a.data.Value != null))
            {
                var attachmentFileName = GetAttachmentFileName(attachment);
                FileId fileId = null;

                try
                {
                    fileId = SaveAttachmentFile(attachment);

                    var localAttachment = new LocalStoredAttachment
                    {
                        FileId = fileId,
                        FileName = attachmentFileName,
                        Description = attachment.desc,
                        OwnerId = ObjectFactory.GetInstance<IUserMapper>().GetTpIdBy(attachment.attacher),
                        CreateDate = CreateDateConverter.ParseFromBugzillaLocalTime(attachment.date)
                    };

                    storedAttachments.Add(localAttachment);
                }
                catch (Exception ex)
                {
                    ObjectFactory.GetInstance<IActivityLogger>().Error(
                        string.Format("Cannot import attachment. Bugzilla Bug ID: {1}; File name: {0}", attachmentFileName, bug_id), ex);

                    if (fileId != null && File.Exists(AttachmentFolder.GetAttachmentFileFullPath(fileId)))
                    {
                        AttachmentFolder.Delete(new[] { fileId });
                    }
                }
            }

            return storedAttachments;
        }

        private static FileId SaveAttachmentFile(attachment attachment)
        {
            byte[] data = Convert.FromBase64String(attachment.data.Value);

            return AttachmentFolder.Save(new MemoryStream(data));
        }

        private static string GetAttachmentFileName(attachment attachment)
        {
            var name = attachment.filename;

            if (string.IsNullOrEmpty(name))
                name = attachment.desc;

            return name;
        }

        public override string ToString()
        {
            return string.Format("Bugzilla ID: {1}; Bug Name: {0}", short_desc, bug_id);
        }
    }
}
