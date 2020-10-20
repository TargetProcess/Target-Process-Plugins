using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Tp.BusinessObjects;
using Tp.Core;
using Tp.I18n;

namespace Tp.Model.Common
{
    public static class CommonMessages
    {
        public static IFormattedMessage FieldIsRequired(Enum field)
        {
            return "{fieldName} should be specified.".Localize(new { fieldName = ToFieldName(field) });
        }

        public static IFormattedMessage FieldIsRequired(string fieldName)
        {
            return "{fieldName} should be specified.".Localize(new { fieldName });
        }

        public static IFormattedMessage FieldIsRequired(IFormattedMessage fieldName)
        {
            return "{fieldName} should be specified.".Localize(new { fieldName });
        }

        public static IFormattedMessage FieldEnumValueIsOutOfBound(Enum field, Enum value)
        {
            return "Value {value} of field {fieldName} is out of bounds.".Localize(new { fieldName = ToFieldName(field), value });
        }

        public static IFormattedMessage FieldShouldBeEqualOrGreaterThanZero(Enum field)
        {
            return "{fieldName} should be equal to or greater than zero.".Localize(new { fieldName = ToFieldName(field) });
        }

        public static IFormattedMessage FieldShouldBeGreaterThanZero(Enum field)
        {
            return "{fieldName} should be greater than zero.".Localize(new { fieldName = ToFieldName(field) });
        }

        public static IFormattedMessage FieldMaxLength(Enum field, int maxLength)
        {
            return "Use {maxLength} characters or fewer for {fieldName}.".Localize(new { fieldName = ToFieldName(field), maxLength });
        }

        public static IFormattedMessage EntityCannotEndEarlierThanStarts(string entityKind)
        {
            return "{entityKind} cannot end earlier than it starts.".Localize(new { entityKind });
        }

        public static IFormattedMessage EntityIsDeleted(EntityKind entityKind, int? entityId)
        {
            return "{entityKind} with id {entityId} is deleted.".Localize(new { entityKind, entityId });
        }

        public static IFormattedMessage EntityStateShouldBeAssignedFromProjectProcess(string projTerm)
        {
            return "Invalid entity state is set. The entity state should be assigned from the same process as a {project}.".Localize(new { project = projTerm});
        }

        public static IFormattedMessage InvalidEntityStateForKind(string entityStateName, string entityKind)
        {
            return "Invalid entity state '{entityStateName}' for {entityKind}.".Localize(new { entityStateName, entityKind });
        }

        public static string ToCommaDelimitedList(IEnumerable<string> items)
        {
            return string.Join(", ", items.Select(x => string.Concat("'", x, "'")));
        }

        public static IFormattedMessage CannotEditSystemCustomField(string customFieldName)
        {
            return "Value of system custom field {customFieldName} can be set only by System User or system activities (e.g. metrics).".Localize(new { customFieldName });
        }

        public static IFormattedMessage MilestoneIsNotAllowed(string entityType)
        {
            return "Milestone cannot be assigned to {entityType} via Milestone property".Localize(new
            {
                entityType
            });
        }

        private static IFormattedMessage ToFieldName(Enum field)
        {
            return field
                .GetAttribute<DescriptionAttribute>()
                .Select(x => x.Description)
                .GetOrElse(field.ToString)
                .AsLocalizable();
        }
    }
}
