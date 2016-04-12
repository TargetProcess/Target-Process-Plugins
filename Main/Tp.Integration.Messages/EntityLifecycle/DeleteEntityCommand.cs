using System;
using Tp.Integration.Common;

namespace Tp.Integration.Messages.EntityLifecycle
{
	/// <summary>
	/// Base command for deleting entity in TargetProcess.
	/// </summary>
	/// <typeparam name="TEntityDto">The type of entity to delete.</typeparam>
	[Serializable]
	public class DeleteEntityCommand<TEntityDto> : IDeleteEntityCommand
		where TEntityDto : DataTransferObject, new()
	{
		public DeleteEntityCommand(int id)
		{
			ID = id;
		}

		/// <summary>
		/// The id of entity to delete.
		/// </summary>
		public int ID { get; set; }

		Type IDeleteEntityCommand.DtoType
		{
			get { return typeof(TEntityDto); }
		}
	}

	/// <summary>
	/// Represents a command for entity deletion.
	/// </summary>
	public interface IDeleteEntityCommand
	{
		/// <summary>
		/// The type of entity to delete.
		/// </summary>
		Type DtoType { get; }

		/// <summary>
		/// The id of entity to delete.
		/// </summary>
		int ID { get; }
	}
}
