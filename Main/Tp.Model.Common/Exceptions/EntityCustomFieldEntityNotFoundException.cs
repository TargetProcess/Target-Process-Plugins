using System;
using Tp.BusinessObjects;
using Tp.I18n;

namespace Tp.Model.Common.Exceptions
{
    /// <summary>
    /// This exception is thrown when entity custom field has an entity cannot be found in the database.
    /// </summary>
    [Serializable]
    public class EntityCustomFieldEntityNotFoundException : CustomFieldException, IFormattedMessageContainer
    {
        public EntityCustomFieldEntityNotFoundException(IFormattedMessage message) : base(message.Value)
        {
            FormattedMessage = message;
        }

        public static EntityCustomFieldEntityNotFoundException Create(string fieldName, EntityKind? entityKind, int? entityId)
        {
            return Create(fieldName, entityKind?.ToString(), entityId.ToString());
        }

        public static EntityCustomFieldEntityNotFoundException Create(string fieldName, string entityKind, string mayBeEntityId)
        {
            return new EntityCustomFieldEntityNotFoundException(
                "Unable to update custom field {fieldName}. {entityKind} #{entityId} not found.".Localize(new
                {
                    fieldName = fieldName ?? "Unknown".Localize().Value,
                    entityKind = entityKind ?? "Unknown entity".Localize().Value,
                    entityId = mayBeEntityId
                }));
        }

        public IFormattedMessage FormattedMessage { get; }
    }
}
