using Tp.Core.Annotations;
using Tp.I18n;

// ReSharper disable once CheckNamespace

namespace System.Linq.Dynamic
{
    public class ParseException : Exception, IFormattedMessageContainer
    {
        public ParseException([NotNull] IFormattedMessage message, int position)
            : this(message, null, position)
        {
        }

        public ParseException([NotNull] Exception innerException, int position)
            : this(null, innerException, position)
        {
        }

        public ParseException(
            [CanBeNull] IFormattedMessage message,
            [CanBeNull] Exception innerException,
            int position)
            : this(position, message ?? innerException?.Message.AsLocalized(), innerException)
        {
        }

        protected ParseException(
            int position,
            [CanBeNull] IFormattedMessage finalMessage,
            [CanBeNull] Exception innerException)
            : base(finalMessage?.Value, innerException)
        {
            FormattedMessage = finalMessage;
            Position = position;
        }

        public int Position { get; }

        public IFormattedMessage FormattedMessage { get; }

        public override string ToString() =>
            $"{Message} (at index {Position})";
    }

    public class UnknownPropertyOrFieldParseException : ParseException
    {
        public UnknownPropertyOrFieldParseException(
            int position, [NotNull] string memberName, [NotNull] Type targetType,
            [CanBeNull] IFormattedMessage message = null, [CanBeNull] Exception inner = null)
            : base(position, message ?? Res.UnknownPropertyOrField(memberName, ExpressionParser.GetTypeName(targetType)), inner)
        {
            MemberName = memberName;
            TargetType = targetType;
        }

        public string MemberName { get; }

        public Type TargetType { get; }
    }

    public class UnknownMethodParseException : ParseException
    {
        public UnknownMethodParseException(
            int position, [NotNull] string methodName, [NotNull] Type targetType,
            [CanBeNull] IFormattedMessage message = null, [CanBeNull] Exception inner = null)
            : base(position, message ?? Res.NoApplicableMethod(methodName, ExpressionParser.GetTypeName(targetType)), inner)
        {
            MethodName = methodName;
            TargetType = targetType;
        }

        public string MethodName { get; }

        public Type TargetType { get; }
    }
}
