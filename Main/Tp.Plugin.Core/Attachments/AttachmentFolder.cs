// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using StructureMap;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Plugin.Core.Attachments
{
    [Serializable]
    public class FileId
    {
        public FileId()
        {
        }

        public FileId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; set; }

        public bool Equals(FileId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Value.Equals(Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(FileId)) return false;
            return Equals((FileId) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static implicit operator FileId(Guid fileId)
        {
            return new FileId(fileId);
        }

        public static bool operator ==(FileId left, FileId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FileId left, FileId right)
        {
            return !(left == right);
        }
    }

    public class AttachmentFolder
    {
        public static void Delete(IEnumerable<FileId> fileIds)
        {
            foreach (var fileId in fileIds)
            {
                File.Delete(GetAttachmentFileFullPath(fileId.Value));
            }
        }

        private static string GetProfileAwareFileFullPath(FileId fileId)
        {
            return Path.Combine(ObjectFactory.GetInstance<IProfile>().FileStorage.GetFolder(), fileId.Value.ToString());
        }

        public static string GetAttachmentFileFullPath(FileId fileId)
        {
            var path = GetProfileAwareFileFullPath(fileId);

            if (!File.Exists(path))
            {
                path = Path.Combine(ObjectFactory.GetInstance<PluginDataFolder>().Path, fileId.Value.ToString());
            }

            return path;
        }

        public static FileId Save(Stream contentStream)
        {
            var fileId = Guid.NewGuid();

            using (var fileStream = new FileStream(GetProfileAwareFileFullPath(fileId), FileMode.CreateNew,
                FileAccess.Write, FileShare.None))
            {
                CopyStream(contentStream, fileStream);
            }

            return fileId;
        }

        private static void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[32768];
            while (true)
            {
                var read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                {
                    return;
                }
                output.Write(buffer, 0, read);
            }
        }
    }
}
