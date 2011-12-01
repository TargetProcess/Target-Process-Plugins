// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle.Commands
{
	[Serializable]
	public class CreateAttachmentFileCommand : CreateEntityCommand<AttachmentFileDTO>
	{
		public CreateAttachmentFileCommand(AttachmentFileDTO dto) : base(dto)
		{
		}
	}

	[Serializable]
	public class UpdateAttachmentFileCommand : UpdateEntityCommand<AttachmentFileDTO>
	{
		public UpdateAttachmentFileCommand(AttachmentFileDTO dto) : base(dto)
		{
		}

		public UpdateAttachmentFileCommand(AttachmentFileDTO dto, Enum[] changedFields) : base(dto, changedFields)
		{
		}
	}

	[Serializable]
	public class DeleteAttachmentFileCommand : DeleteEntityCommand<AttachmentFileDTO>
	{
		public DeleteAttachmentFileCommand(int id) : base(id)
		{
		}
	}
}