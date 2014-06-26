using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.PopEmailIntegration.Rules.Parsing;

namespace Tp.PopEmailIntegration.Rules.ThenClauses
{
	public class ThenCreatePublicRequestClause : ThenClause
	{
		private ThenCreatePublicRequestClause(ParseNode clauseNode, ITpBus bus, IStorageRepository storage)
			: base(clauseNode, bus, storage)
		{
		}

		public override void Execute(MessageDTO dto, AttachmentDTO[] attachments)
		{
			var command = new CreateRequestFromMessageCommand { MessageDto = dto, ProjectId = _projectId, Attachments = attachments, IsPrivate = false };
			_bus.SendLocal(command);
		}

		public static IThenClause Create(ParseNode clauseNode)
		{
			return new ThenCreatePublicRequestClause(clauseNode, ObjectFactory.GetInstance<ITpBus>(),
			                                         ObjectFactory.GetInstance<IStorageRepository>());
		}
	}
}