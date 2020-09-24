using Tp.Core.Annotations;

namespace Tp.Core
{
    public class LazyFormatString
    {
        public string Format { get; }
        public object[] Args { get; }

        [StringFormatMethod("format")]
        public LazyFormatString(string format, params object[] args)
        {
            Format = format;
            Args = args;
        }

        [NotNull]
        [MustUseReturnValue]
        public string Formatted() => string.Format(Format, Args);
    }
}
