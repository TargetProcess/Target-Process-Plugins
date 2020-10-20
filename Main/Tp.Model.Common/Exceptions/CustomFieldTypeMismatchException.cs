using System;
using Tp.I18n;

namespace Tp.Model.Common.Exceptions
{
    /// <summary>
    /// This exception is thrown when value of invalid type is written to a custom field.
    /// </summary>
    [Serializable]
    public class CustomFieldTypeMismatchException : CustomFieldException, IFormattedMessageContainer
    {
        public CustomFieldTypeMismatchException(IFormattedMessage message) : base(message.Value)
        {
            FormattedMessage = message;
        }

        public static Exception Create(Type expected, Type actual)
        {
            return Create(expected.Name, actual.Name);
        }

        public static Exception Create(string expected, string actual)
        {
            return new CustomFieldTypeMismatchException(
                "Invalid value type, expected {expected}, but got {actual}."
                    .Localize(new
                    {
                        expected,
                        actual,
                    }));
        }

        public IFormattedMessage FormattedMessage { get; }
    }
}
